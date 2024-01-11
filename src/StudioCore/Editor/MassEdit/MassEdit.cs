#nullable enable
using Andre.Formats;
using StudioCore.Editor;
using StudioCore.ParamEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudioCore.Editor.MassEdit;

public enum MassEditResultType
{
    SUCCESS,
    PARSEERROR,
    OPERATIONERROR
}

public class MassEditResult
{
    public string Information;
    public MassEditResultType Type;

    public MassEditResult(MassEditResultType result, string info)
    {
        Type = result;
        Information = info;
    }
}

internal class MEParseException : Exception
{
    public MEParseException(string? message, int line) : base($@"{message} (line {line})")
    {
    }
}

internal struct MEFilterStage
{
    internal string command;
    // No arguments because this is handled separately in SearchEngine
    internal MEFilterStage(string toParse, int line, string stageName, TypelessSearchEngine expectedStage)
    {
        command = toParse.Trim();
        if (command.Equals(""))
        {
            throw new MEParseException($@"Could not find {stageName} filter. Add : and one of {string.Join(", ", expectedStage.AvailableCommandsForHelpText())}", line);
        }
    }
}
internal struct MEOperationStage
{
    internal string command;
    internal string arguments;
    internal MEOperationStage(string toParse, int line, string stageName, METypelessOperation expectedOperation)
    {
        var stage = toParse.TrimStart().Split(' ', 2);
        command = stage[0].Trim();
        arguments = stage[1];
        if (command.Equals(""))
        {
            throw new MEParseException($@"Could not find operation to perform. Add : and one of {string.Join(' ', expectedOperation.AllCommands().Keys)}", line);
        }
        if (!expectedOperation.AllCommands().ContainsKey(command))
        {
            throw new MEParseException($@"Unknown {stageName} operation {command}", line);
        }
    }
    internal string[] getArguments(int count)
    {
        return arguments.Split(':', count);
    }
}
internal class MECommand
{
    List<MEFilterStage> filters = new();
    MEOperationStage operation;
}

public static class MassParamEdit
{
    public static Dictionary<string, object> massEditVars = new();

    internal static object WithDynamicOf(object instance, Func<dynamic, object> dynamicFunc)
    {
        try
        {
            return Convert.ChangeType(dynamicFunc(instance), instance.GetType());
        }
        catch
        {
            // Second try, handle byte[], and casts from numerical values to string which need parsing.
            var ret = dynamicFunc(instance.ToParamEditorString());
            if (instance.GetType() == typeof(byte[]))
            {
                ret = ParamUtils.Dummy8Read((string)ret, ((byte[])instance).Length);
            }

            return Convert.ChangeType(ret, instance.GetType());
        }
    }

    public static void AppendParamEditAction(this List<EditorAction> actions, Param.Row row,
        (PseudoColumn, Param.Column) col, object newval)
    {
        if (col.Item1 == PseudoColumn.ID)
        {
            if (!row.ID.Equals(newval))
            {
                actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("ID"), -1, row, newval));
            }
        }
        else if (col.Item1 == PseudoColumn.Name)
        {
            if (row.Name == null || !row.Name.Equals(newval))
            {
                actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, newval));
            }
        }
        else
        {
            Param.Cell handle = row[col.Item2];
            if (!(handle.Value.Equals(newval)
                  || (handle.Value.GetType() == typeof(byte[])
                      && ParamUtils.ByteArrayEquals((byte[])handle.Value, (byte[])newval))))
            {
                actions.Add(new PropertiesChangedAction(handle.GetType().GetProperty("Value"), -1, handle, newval));
            }
        }
    }
}

public class MassParamEditRegex
{

    // Do we want these variables? shouldn't they be contained?
    private ParamBank bank;
    private ParamEditorSelectionState context;
    private Func<object, string[], object> genericFunc;

    private Func<int, Func<int, (PseudoColumn, Param.Column), string>[], string, Param.Row, List<EditorAction>,
        MassEditResult> rowOpOrCellStageFunc;
    private Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] paramArgFuncs;
    private string[] argNames;

    private MEOperationStage globalOperationInfo;
    private MEOperationStage varOperationInfo;
    private MEOperationStage rowOperationInfo;
    private MEOperationStage cellOperationInfo;
    private MEFilterStage varStageInfo;
    private MEFilterStage paramRowStageInfo;
    private MEFilterStage paramStageInfo;
    private MEFilterStage rowStageInfo;
    private MEFilterStage cellStageInfo;

    public static (MassEditResult, ActionManager child) PerformMassEdit(ParamBank bank, string commandsString,
        ParamEditorSelectionState context)
    {
        int currentLine = 0;
        try
        {
            var commands = commandsString.Split('\n');
            var changeCount = 0;
            ActionManager childManager = new();
            foreach (var cmd in commands)
            {
                currentLine++;
                var command = cmd;
                if (command.StartsWith("##") || string.IsNullOrWhiteSpace(command))
                {
                    continue;
                }

                if (command.EndsWith(';'))
                {
                    command = command.Substring(0, command.Length - 1);
                }

                (MassEditResult result, List<EditorAction> actions) = (null, null);

                MassParamEditRegex currentEditData = new();
                currentEditData.bank = bank;
                currentEditData.context = context;

                var primaryFilter = command.Split(':', 2)[0].Trim();

                if (MEGlobalOperation.globalOps.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = currentEditData.ParseGlobalOpStep(currentLine, command);
                }
                else if (VarSearchEngine.vse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = ParseFilterStep(currentLine, command, "variable", VarSearchEngine.vse, ref currentEditData.varStageInfo, "var operation", (currentLine, restOfStages) =>
                    {
                        return currentEditData.ParseVarOpStep(currentLine, restOfStages);
                    });
                }
                else if (ParamAndRowSearchEngine.parse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = ParseFilterStep(currentLine, command, "paramrow", ParamAndRowSearchEngine.parse, ref currentEditData.paramRowStageInfo, "cell filter or row operation", (currentLine, restOfStages) =>
                    {
                        if (MERowOperation.rowOps.HandlesCommand(restOfStages.Trim().Split(" ", 2)[0]))
                        {
                            return currentEditData.ParseRowOpStep(currentLine, restOfStages);
                        }
                        return ParseFilterStep(currentLine, restOfStages, "cell/property", CellSearchEngine.cse, ref currentEditData.cellStageInfo, "operation to perform", (currentLine, restOfStages) =>
                        {
                            currentEditData.rowOpOrCellStageFunc = currentEditData.ExecCellStage;
                            return currentEditData.ParseCellOpStep(currentLine, restOfStages);
                        });
                    });
                }
                else
                {
                    (result, actions) = ParseFilterStep(currentLine, command, "param", ParamSearchEngine.pse, ref currentEditData.paramStageInfo, "row filter", (currentLine, restOfStages) =>
                    {
                        return ParseFilterStep(currentLine, restOfStages, "row", RowSearchEngine.rse, ref currentEditData.rowStageInfo, "cell filter or row operation", (currentLine, restOfStages) =>
                        {
                            if (MERowOperation.rowOps.HandlesCommand(restOfStages.Trim().Split(" ", 2)[0]))
                            {
                                return currentEditData.ParseRowOpStep(currentLine, restOfStages);
                            }
                            return ParseFilterStep(currentLine, restOfStages, "cell/property", CellSearchEngine.cse, ref currentEditData.cellStageInfo, "operation to perform", (currentLine, restOfStages) =>
                            {
                                currentEditData.rowOpOrCellStageFunc = currentEditData.ExecCellStage;
                                return currentEditData.ParseCellOpStep(currentLine, restOfStages);
                            });
                        });
                    });
                }

                if (result.Type != MassEditResultType.SUCCESS)
                {
                    return (result, null);
                }

                changeCount += actions.Count;
                childManager.ExecuteAction(new CompoundAction(actions));
            }

            return (new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected"), childManager);
        }
        catch (Exception e)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, e.ToString()), null);
        }
    }

    private static (MassEditResult, List<EditorAction>) ParseFilterStep(int currentLine, string restOfStages, string stageName, TypelessSearchEngine expectedSearchEngine, ref MEFilterStage target, string expectedNextStageName, Func<int, string, (MassEditResult, List<EditorAction>)> nextStageFunc)
    {
        var stage = restOfStages.Split(":", 2);
        target = new MEFilterStage(stage[0], currentLine, stageName, expectedSearchEngine);

        if (stage.Length < 2)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {expectedNextStageName}. Check your colon placement. (line {currentLine})"), null);
        }

        return nextStageFunc(currentLine, stage[1]);
    }
    private (MassEditResult, List<EditorAction>) ParseOpStep(int currentLine, string restOfStages, string stageName, METypelessOperation operation, ref MEOperationStage target, ref string[] argNameTarget, ref Func<object, string[], object> funcTarget)
    {
        target = new MEOperationStage(restOfStages, currentLine, stageName, operation);

        var meOpDef = operation.AllCommands()[target.command];
        (argNameTarget, funcTarget) = (meOpDef.argNames, meOpDef.function);
        ExecParamOperationArguments(currentLine, globalOperationInfo.arguments);
        if (argNameTarget.Length != paramArgFuncs.Length)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {globalOperationInfo.command} (line {currentLine})"), null);
        }

        return SandboxMassEditExecution(currentLine, partials => ExecGlobalOp(currentLine));
    }

    private (MassEditResult, List<EditorAction>) ParseGlobalOpStep(int currentLine, string restOfStages)
    {
        globalOperationInfo = new MEOperationStage(restOfStages, currentLine, "global", MEGlobalOperation.globalOps);

        var meOpDef = MEGlobalOperation.globalOps.operations[globalOperationInfo.command];
        (argNames, genericFunc) = (meOpDef.argNames, meOpDef.function);
        ExecParamOperationArguments(currentLine, globalOperationInfo.arguments);
        if (argNames.Length != paramArgFuncs.Length)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {globalOperationInfo.command} (line {currentLine})"), null);
        }

        return SandboxMassEditExecution(currentLine, partials => ExecGlobalOp(currentLine));
    }

    private (MassEditResult, List<EditorAction>) ParseVarOpStep(int currentLine, string restOfStages)
    {
        varOperationInfo = new MEOperationStage(restOfStages, currentLine, "var", MEValueOperation.valueOps);

        var meOpDef = MEValueOperation.valueOps.operations[varOperationInfo.command];
        (argNames, genericFunc) = (meOpDef.argNames, meOpDef.function);
        ExecParamOperationArguments(currentLine, varOperationInfo.arguments);
        if (argNames.Length != paramArgFuncs.Length)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {varOperationInfo} (line {currentLine})"), null);
        }

        return SandboxMassEditExecution(currentLine, partials => ExecVarStage(currentLine));
    }

    private (MassEditResult, List<EditorAction>) ParseRowOpStep(int currentLine, string restOfStages)
    {
        var operationstage = restOfStages.TrimStart().Split(" ", 2);
        rowOperationInfo = new MEOperationStage(restOfStages, currentLine, "row", MERowOperation.rowOps);

        var meOpDef = MERowOperation.rowOps.operations[rowOperationInfo.command];
        (argNames, genericFunc) = (meOpDef.argNames, meOpDef.function);
        ExecParamOperationArguments(currentLine, operationstage.Length > 1 ? operationstage[1] : null);
        if (argNames.Length != paramArgFuncs.Length)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {rowOperationInfo} (line {currentLine})"), null);
        }

        rowOpOrCellStageFunc = ExecRowOp;
        return SandboxMassEditExecution(currentLine, partials =>
            paramRowStageInfo.command != null ? ExecParamRowStage(currentLine, partials) : ExecParamStage(currentLine, partials));
    }

    private (MassEditResult, List<EditorAction>) ParseCellOpStep(int currentLine, string restOfStages)
    {
        var operationstage = restOfStages.TrimStart().Split(" ", 2);
        cellOperationInfo = new MEOperationStage(restOfStages, currentLine, "cell/property", MEValueOperation.valueOps);

        var meOpDef = MEValueOperation.valueOps.operations[cellOperationInfo.command];
        (argNames, genericFunc) = (meOpDef.argNames, meOpDef.function);
        ExecParamOperationArguments(currentLine, operationstage.Length > 1 ? operationstage[1] : null);
        if (argNames.Length != paramArgFuncs.Length)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {cellOperationInfo} (line {currentLine})"), null);
        }

        return SandboxMassEditExecution(currentLine, partials =>
            paramRowStageInfo.command != null ? ExecParamRowStage(currentLine, partials) : ExecParamStage(currentLine, partials));
    }

    private void ExecParamOperationArguments(int currentLine, string opargs)
    {
        paramArgFuncs = MEOperationArgument.arg.getContextualArguments(argNames.Length, opargs);
    }

    private (MassEditResult, List<EditorAction>) SandboxMassEditExecution(int currentLine,
        Func<List<EditorAction>, MassEditResult> innerFunc)
    {
        List<EditorAction> partialActions = new();
        try
        {
            return (innerFunc(partialActions), partialActions);
        }
        catch (Exception e)
        {
            return (new MassEditResult(MassEditResultType.OPERATIONERROR, @$"Error on line {currentLine}" + '\n' + e.ToString()), null);
        }

        return (new MassEditResult(MassEditResultType.SUCCESS, $@"{partialActions.Count} cells affected"),
            partialActions);
    }

    private MassEditResult ExecGlobalOp(int currentLine)
    {
        var globalArgValues = paramArgFuncs.Select(f => f(-1, null)(-1, null)(-1, (PseudoColumn.None, null)))
            .ToArray();
        var result = (bool)genericFunc(context, globalArgValues);
        if (!result)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Error performing global operation {globalOperationInfo} (line {currentLine})");
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecVarStage(int currentLine)
    {
        ;
        var varArgs = paramArgFuncs
            .Select((func, i) => func(-1, null)(-1, null)(-1, (PseudoColumn.None, null))).ToArray();
        foreach (var varName in VarSearchEngine.vse.Search(false, varStageInfo.command, false, false))
        {
            MassEditResult res = ExecVarOpStage(currentLine, varName, varArgs);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecVarOpStage(int currentLine, string var, string[] args)
    {
        MassParamEdit.massEditVars[var] = genericFunc(MassParamEdit.massEditVars[var], args);
        var result = true; // Anything that practicably can go wrong 
        if (!result)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Error performing var operation {varOperationInfo} (line {currentLine})");
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecParamRowStage(int currentLine, List<EditorAction> partialActions)
    {
        Param activeParam = bank.Params[context.GetActiveParam()];
        IEnumerable<Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>> paramArgFunc =
            paramArgFuncs.Select((func, i) => func(0, activeParam)); // technically invalid for clipboard
        var rowEditCount = -1;
        foreach ((MassEditRowSource source, Param.Row row) in ParamAndRowSearchEngine.parse.Search(context,
                     paramRowStageInfo.command, false, false))
        {
            rowEditCount++;
            Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc =
                paramArgFunc.Select((rowFunc, i) => rowFunc(rowEditCount, row)).ToArray();
            var paramname = source == MassEditRowSource.Selection
                ? context.GetActiveParam()
                : ParamBank.ClipboardParam;
            MassEditResult res = rowOpOrCellStageFunc(currentLine, rowArgFunc, paramname, row, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecParamStage(int currentLine, List<EditorAction> partialActions)
    {
        var paramEditCount = -1;
        var operationForPrint = rowOperationInfo.command != null ? rowOperationInfo : cellOperationInfo;
        foreach ((ParamBank b, Param p) in ParamSearchEngine.pse.Search(false, paramStageInfo.command, false, false))
        {
            paramEditCount++;
            IEnumerable<Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>> paramArgFunc =
                paramArgFuncs.Select((func, i) => func(paramEditCount, p));
            if (argNames.Length != paramArgFuncs.Length)
            {
                return new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operationForPrint} (line {currentLine})");
            }

            var paramname = b.GetKeyForParam(p);
            MassEditResult res = ExecRowStage(currentLine, paramArgFunc, paramname, b, p, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecRowStage(int currentLine,
        IEnumerable<Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>> paramArgFunc,
        string paramname, ParamBank b, Param p, List<EditorAction> partialActions)
    {
        var rowEditCount = -1;
        foreach (Param.Row row in RowSearchEngine.rse.Search((b, p), rowStageInfo.command, false, false))
        {
            rowEditCount++;
            Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc =
                paramArgFunc.Select((rowFunc, i) => rowFunc(rowEditCount, row)).ToArray();
            MassEditResult res = rowOpOrCellStageFunc(currentLine, rowArgFunc, paramname, row, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecRowOp(int currentLine, Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc, string paramname,
        Param.Row row, List<EditorAction> partialActions)
    {
        var rowArgValues = rowArgFunc.Select((argV, i) => argV(-1, (PseudoColumn.None, null))).ToArray();
        (Param? p2, Param.Row? rs) = ((Param? p2, Param.Row? rs))genericFunc((paramname, row), rowArgValues);
        if (p2 == null)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {rowOperationInfo} {String.Join(' ', rowArgValues)} on row (line {currentLine})");
        }

        if (rs != null)
        {
            partialActions.Add(new AddParamsAction(p2, "FromMassEdit", new List<Param.Row> { rs }, false, true));
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecCellStage(int currentLine, Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc,
        string paramname, Param.Row row, List<EditorAction> partialActions)
    {
        var cellEditCount = -1;
        foreach ((PseudoColumn, Param.Column) col in CellSearchEngine.cse.Search((paramname, row), cellStageInfo.command,
                     false, false))
        {
            cellEditCount++;
            var cellArgValues = rowArgFunc.Select((argV, i) => argV(cellEditCount, col)).ToArray();
            MassEditResult res = ExecCellOp(currentLine, cellArgValues, paramname, row, col, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecCellOp(int currentLine, string[] cellArgValues, string paramname, Param.Row row,
        (PseudoColumn, Param.Column) col, List<EditorAction> partialActions)
    {
        object res = null;
        string errHelper = null;
        try
        {
            res = genericFunc(row.Get(col), cellArgValues);
        }
        catch (FormatException e)
        {
            errHelper = "Type is not correct";
        }
        catch (InvalidCastException e)
        {
            errHelper = "Cannot cast to correct type";
        }
        catch (Exception e)
        {
            errHelper = "Unknown error";
        }

        if (res == null && col.Item1 == PseudoColumn.ID)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo} {String.Join(' ', cellArgValues)} on ID ({errHelper}) (line {currentLine})");
        }

        if (res == null && col.Item1 == PseudoColumn.Name)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo} {String.Join(' ', cellArgValues)} on Name ({errHelper}) (line {currentLine})");
        }

        if (res == null)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo} {String.Join(' ', cellArgValues)} on field {col.Item2.Def.InternalName} ({errHelper}) (line {currentLine})");
        }

        partialActions.AppendParamEditAction(row, col, res);
        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }
}

public class MassParamEditOther
{
    public static AddParamsAction SortRows(ParamBank bank, string paramName)
    {
        Param param = bank.Params[paramName];
        List<Param.Row> newRows = new(param.Rows);
        newRows.Sort((a, b) => { return a.ID - b.ID; });
        return new AddParamsAction(param, paramName, newRows, true,
            true); //appending same params and allowing overwrite
    }
}
internal abstract class METypelessOperationDef
{
    internal string[] argNames;
    internal string wiki;
    internal Func<object, string[], object> function;
}
internal class MEOperationDef<T, O> : METypelessOperationDef
{
    internal MEOperationDef(string[] args, string tooltip, Func<T, string[], O> func)
    {
        argNames = args;
        wiki = tooltip;
        function = func as Func<object, string[], object>;
    }
}
public abstract class METypelessOperation
{
    internal abstract Dictionary<string, METypelessOperationDef> AllCommands();
}
public class MEOperation<T, O> : METypelessOperation
{
    internal Dictionary<string, METypelessOperationDef> operations = new();

    internal MEOperation()
    {
        Setup();
    }

    internal virtual void Setup()
    {
    }

    internal bool HandlesCommand(string command)
    {
        return operations.ContainsKey(command);
    }
    internal override Dictionary<string, METypelessOperationDef> AllCommands()
    {
        return operations;
    }

    public List<(string, string[], string)> AvailableCommands()
    {
        List<(string, string[], string)> options = new();
        foreach (var op in operations.Keys)
        {
            options.Add((op, operations[op].argNames, operations[op].wiki));
        }

        return options;
    }
    internal MEOperationDef<T, O> newCmd(string[] args, string wiki, Func<T, string[], O> func)
    {
        return new MEOperationDef<T, O>(args, wiki, func);
    }
    internal MEOperationDef<T, O> newCmd(string wiki, Func<T, string[], O> func)
    {
        return new MEOperationDef<T, O>(Array.Empty<string>(), wiki, func);
    }
}

public class MEGlobalOperation : MEOperation<ParamEditorSelectionState, bool>
{
    public static MEGlobalOperation globalOps = new();

    internal override void Setup()
    {
        operations.Add("clear", newCmd(new string[0], "Clears clipboard param and rows", (selectionState, args) =>
        {
            ParamBank.ClipboardParam = null;
            ParamBank.ClipboardRows.Clear();
            return true;
        }
        ));
        operations.Add("newvar", newCmd(new[] { "variable name", "value" },
            "Creates a variable with the given value, and the type of that value", (selectionState, args) =>
            {
                int asInt;
                double asDouble;
                if (int.TryParse(args[1], out asInt))
                {
                    MassParamEdit.massEditVars[args[0]] = asInt;
                }
                else if (double.TryParse(args[1], out asDouble))
                {
                    MassParamEdit.massEditVars[args[0]] = asDouble;
                }
                else
                {
                    MassParamEdit.massEditVars[args[0]] = args[1];
                }

                return true;
            }
        ));
        operations.Add("clearvars", newCmd(new string[0], "Deletes all variables", (selectionState, args) =>
        {
            MassParamEdit.massEditVars.Clear();
            return true;
        }
        ));
    }
}

public class MERowOperation : MEOperation<(string, Param.Row), (Param, Param.Row)>
{
    public static MERowOperation rowOps = new();

    internal override void Setup()
    {
        operations.Add("copy", newCmd(new string[0],
            "Adds the selected rows into clipboard. If the clipboard param is different, the clipboard is emptied first",
            (paramAndRow, args) =>
            {
                var paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                {
                    throw new Exception(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new Exception($@"Could not locate param {paramKey}");
                }

                Param p = ParamBank.PrimaryBank.Params[paramKey];
                // Only supporting single param in clipboard
                if (ParamBank.ClipboardParam != paramKey)
                {
                    ParamBank.ClipboardParam = paramKey;
                    ParamBank.ClipboardRows.Clear();
                }

                ParamBank.ClipboardRows.Add(new Param.Row(row, p));
                return (p, null);
            }
        ));
        operations.Add("copyN", newCmd(new[] { "count" },
            "Adds the selected rows into clipboard the given number of times. If the clipboard param is different, the clipboard is emptied first",
            (paramAndRow, args) =>
            {
                var paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                {
                    throw new Exception(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new Exception($@"Could not locate param {paramKey}");
                }

                var count = uint.Parse(args[0]);
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                // Only supporting single param in clipboard
                if (ParamBank.ClipboardParam != paramKey)
                {
                    ParamBank.ClipboardParam = paramKey;
                    ParamBank.ClipboardRows.Clear();
                }

                for (var i = 0; i < count; i++)
                {
                    ParamBank.ClipboardRows.Add(new Param.Row(row, p));
                }

                return (p, null);
            }
        ));
        operations.Add("paste", newCmd(new string[0],
            "Adds the selected rows to the primary regulation or parambnd in the selected param",
            (paramAndRow, args) =>
            {
                var paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                {
                    throw new Exception(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new Exception($@"Could not locate param {paramKey}");
                }

                Param p = ParamBank.PrimaryBank.Params[paramKey];
                return (p, new Param.Row(row, p));
            }
        ));
    }
}

public class MEValueOperation : MEOperation<object, object>
{
    public static MEValueOperation valueOps = new();

    internal override void Setup()
    {
        operations.Add("=",
            newCmd(new[] { "number or text" },
                "Assigns the given value to the selected values. Will attempt conversion to the value's data type",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => args[0])));
        operations.Add("+", newCmd(new[] { "number or text" },
            "Adds the number to the selected values, or appends text if that is the data type of the values",
            (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v =>
            {
                double val;
                if (double.TryParse(args[0], out val))
                {
                    return v + val;
                }

                return v + args[0];
            })));
        operations.Add("-",
            newCmd(new[] { "number" }, "Subtracts the number from the selected values",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v - double.Parse(args[0]))));
        operations.Add("*",
            newCmd(new[] { "number" }, "Multiplies selected values by the number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v * double.Parse(args[0]))));
        operations.Add("/",
            newCmd(new[] { "number" }, "Divides the selected values by the number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v / double.Parse(args[0]))));
        operations.Add("%",
            newCmd(new[] { "number" }, "Gives the remainder when the selected values are divided by the number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v % double.Parse(args[0]))));
        operations.Add("scale", newCmd(new[] { "factor number", "center number" },
            "Multiplies the difference between the selected values and the center number by the factor number",
            (ctx, args) =>
            {
                var opp1 = double.Parse(args[0]);
                var opp2 = double.Parse(args[1]);
                return MassParamEdit.WithDynamicOf(ctx, v =>
                {
                    return ((v - opp2) * opp1) + opp2;
                });
            }
        ));
        operations.Add("replace",
            newCmd(new[] { "text to replace", "new text" },
                "Interprets the selected values as text and replaces all occurances of the text to replace with the new text",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v.Replace(args[0], args[1]))));
        operations.Add("replacex", newCmd(new[] { "text to replace (regex)", "new text (w/ groups)" },
            "Interprets the selected values as text and replaces all occurances of the given regex with the replacement, supporting regex groups",
            (ctx, args) =>
            {
                Regex rx = new(args[0]);
                return MassParamEdit.WithDynamicOf(ctx, v => rx.Replace(v, args[1]));
            }
        ));
        operations.Add("max",
            newCmd(new[] { "number" }, "Returns the larger of the current value and number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => Math.Max(v, double.Parse(args[0])))));
        operations.Add("min",
            newCmd(new[] { "number" }, "Returns the smaller of the current value and number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => Math.Min(v, double.Parse(args[0])))));
    }
}

public class MEOperationArgument
{
    public static MEOperationArgument arg = new();
    private readonly Dictionary<string, OperationArgumentGetter> argumentGetters = new();
    private OperationArgumentGetter defaultGetter;

    private MEOperationArgument()
    {
        Setup();
    }

    private OperationArgumentGetter newGetter(string[] args, string wiki,
        Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }

    private void Setup()
    {
        defaultGetter = newGetter(new string[0], "Gives the specified value",
            value => (i, param) => (j, row) => (k, col) => value[0]);
        argumentGetters.Add("self", newGetter(new string[0], "Gives the value of the currently selected value",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return row.Get(col).ToParamEditorString();
            }));
        argumentGetters.Add("field", newGetter(new[] { "field internalName" },
            "Gives the value of the given cell/field for the currently selected row and param", field =>
                (i, param) =>
                {
                    (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                    if (!col.IsColumnValid())
                    {
                        throw new Exception($@"Could not locate field {field[0]}");
                    }

                    return (j, row) =>
                    {
                        var v = row.Get(col).ToParamEditorString();
                        return (k, c) => v;
                    };
                }));
        argumentGetters.Add("vanilla", newGetter(new string[0],
            "Gives the value of the equivalent cell/field in the vanilla regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added",
            empty =>
            {
                ParamBank bank = ParamBank.VanillaBank;
                return (i, param) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    return (j, row) =>
                    {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                        {
                            throw new Exception($@"Could not locate vanilla row {row.ID}");
                        }

                        return (k, col) =>
                        {
                            if (col.Item1 == PseudoColumn.None && col.Item2 == null)
                            {
                                throw new Exception(@"Could not locate given field or property");
                            }

                            return vRow.Get(col).ToParamEditorString();
                        };
                    };
                };
            }));
        argumentGetters.Add("aux", newGetter(new[] { "parambank name" },
            "Gives the value of the equivalent cell/field in the specified regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*",
            bankName =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(bankName[0]))
                {
                    throw new Exception($@"Could not locate paramBank {bankName[0]}");
                }

                ParamBank bank = ParamBank.AuxBanks[bankName[0]];
                return (i, param) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    return (j, row) =>
                    {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                        {
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        }

                        return (k, col) =>
                        {
                            if (!col.IsColumnValid())
                            {
                                throw new Exception(@"Could not locate given field or property");
                            }

                            return vRow.Get(col).ToParamEditorString();
                        };
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        argumentGetters.Add("vanillafield", newGetter(new[] { "field internalName" },
            "Gives the value of the specified cell/field in the vanilla regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added",
            field => (i, param) =>
            {
                var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                Param? vParam = ParamBank.VanillaBank.GetParamFromName(paramName);
                if (vParam == null)
                {
                    throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                }

                (PseudoColumn, Param.Column) col = vParam.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                return (j, row) =>
                {
                    Param.Row vRow = vParam?[row.ID];
                    if (vRow == null)
                    {
                        throw new Exception($@"Could not locate vanilla row {row.ID}");
                    }

                    var v = vRow.Get(col).ToParamEditorString();
                    return (k, c) => v;
                };
            }));
        argumentGetters.Add("auxfield", newGetter(new[] { "parambank name", "field internalName" },
            "Gives the value of the specified cell/field in the specified regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*",
            bankAndField =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(bankAndField[0]))
                {
                    throw new Exception($@"Could not locate paramBank {bankAndField[0]}");
                }

                ParamBank bank = ParamBank.AuxBanks[bankAndField[0]];
                return (i, param) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    (PseudoColumn, Param.Column) col = vParam.GetCol(bankAndField[1]);
                    if (!col.IsColumnValid())
                    {
                        throw new Exception($@"Could not locate field {bankAndField[1]}");
                    }

                    return (j, row) =>
                    {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                        {
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        }

                        var v = vRow.Get(col).ToParamEditorString();
                        return (k, c) => v;
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        argumentGetters.Add("paramlookup", newGetter(new[] { "param name", "row id", "field name" },
            "Returns the specific value specified by the exact param, row and field.", address =>
            {
                Param param = ParamBank.PrimaryBank.Params[address[0]];
                if (param == null)
                    throw new Exception($@"Could not find param {address[0]}");
                var id = int.Parse(address[1]);
                (PseudoColumn, Param.Column) field = param.GetCol(address[2]);
                if (!field.IsColumnValid())
                    throw new Exception($@"Could not find field {address[2]} in param {address[0]}");
                var row = param[id];
                if (row == null)
                    throw new Exception($@"Could not find row {id} in param {address[0]}");
                var value = row.Get(field).ToParamEditorString();
                return (i, param) => (j, row) => (k, col) => value;
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("average", newGetter(new[] { "field internalName", "row selector" },
            "Gives the mean value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                Type colType = col.GetColumnType();
                if (colType == typeof(string) || colType == typeof(byte[]))
                {
                    throw new Exception($@"Cannot average field {field[0]}");
                }

                List<Param.Row>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                IEnumerable<object> vals = rows.Select((row, i) => row.Get(col));
                var avg = vals.Average(val => Convert.ToDouble(val));
                return (j, row) => (k, c) => avg.ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("median", newGetter(new[] { "field internalName", "row selector" },
            "Gives the median value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<Param.Row>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                IEnumerable<object> vals = rows.Select((row, i) => row.Get(col));
                var avg = vals.OrderBy(val => Convert.ToDouble(val)).ElementAt(vals.Count() / 2);
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("mode", newGetter(new[] { "field internalName", "row selector" },
            "Gives the most common value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<Param.Row>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var avg = ParamUtils.GetParamValueDistribution(rows, col).OrderByDescending(g => g.Item2)
                    .First().Item1;
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("min", newGetter(new[] { "field internalName", "row selector" },
            "Gives the smallest value from the cells/fields found using the given param, row selector and field",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<Param.Row>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var min = rows.Min(r => r[field[0]].Value.Value);
                return (j, row) => (k, c) => min.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("max", newGetter(new[] { "field internalName", "row selector" },
            "Gives the largest value from the cells/fields found using the given param, row selector and field",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<Param.Row>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var max = rows.Max(r => r[field[0]].Value.Value);
                return (j, row) => (k, c) => max.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("random", newGetter(
            new[] { "minimum number (inclusive)", "maximum number (exclusive)" },
            "Gives a random decimal number between the given values for each selected value", minAndMax =>
            {
                double min;
                double max;
                if (!double.TryParse(minAndMax[0], out min) || !double.TryParse(minAndMax[1], out max))
                {
                    throw new Exception(@"Could not parse min and max random values");
                }

                if (max <= min)
                {
                    throw new Exception(@"Random max must be greater than min");
                }

                var range = max - min;
                return (i, param) => (j, row) => (k, c) => ((Random.Shared.NextDouble() * range) + min).ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("randint", newGetter(
            new[] { "minimum integer (inclusive)", "maximum integer (inclusive)" },
            "Gives a random integer between the given values for each selected value", minAndMax =>
            {
                int min;
                int max;
                if (!int.TryParse(minAndMax[0], out min) || !int.TryParse(minAndMax[1], out max))
                {
                    throw new Exception(@"Could not parse min and max randint values");
                }

                if (max <= min)
                {
                    throw new Exception(@"Random max must be greater than min");
                }

                return (i, param) => (j, row) => (k, c) => Random.Shared.NextInt64(min, max + 1).ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("randFrom", newGetter(new[] { "param name", "field internalName", "row selector" },
            "Gives a random value from the cells/fields found using the given param, row selector and field, for each selected value",
            paramFieldRowSelector =>
            {
                Param srcParam = ParamBank.PrimaryBank.Params[paramFieldRowSelector[0]];
                List<Param.Row> srcRows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, srcParam),
                    paramFieldRowSelector[2], false, false);
                var values = srcRows.Select((r, i) => r[paramFieldRowSelector[1]].Value.Value).ToArray();
                return (i, param) =>
                    (j, row) => (k, c) => values[Random.Shared.NextInt64(values.Length)].ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("paramIndex", newGetter(new string[0],
            "Gives an integer for the current selected param, beginning at 0 and increasing by 1 for each param selected",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return i.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("rowIndex", newGetter(new string[0],
            "Gives an integer for the current selected row, beginning at 0 and increasing by 1 for each row selected",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return j.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("fieldIndex", newGetter(new string[0],
            "Gives an integer for the current selected cell/field, beginning at 0 and increasing by 1 for each cell/field selected",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return k.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
    }

    public List<(string, string[])> AllArguments()
    {
        List<(string, string[])> options = new();
        foreach (var op in argumentGetters.Keys)
        {
            options.Add((op, argumentGetters[op].args));
        }

        return options;
    }

    public List<(string, string, string[])> VisibleArguments()
    {
        List<(string, string, string[])> options = new();
        foreach (var op in argumentGetters.Keys)
        {
            OperationArgumentGetter oag = argumentGetters[op];
            if (oag.shouldShow == null || oag.shouldShow())
            {
                options.Add((op, oag.wiki, oag.args));
            }
        }

        return options;
    }

    internal Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[]
        getContextualArguments(int argumentCount, string opData)
    {
        var opArgs = opData == null ? new string[0] : opData.Split(':', argumentCount);
        var contextualArgs =
            new Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[opArgs
                .Length];
        for (var i = 0; i < opArgs.Length; i++)
        {
            contextualArgs[i] = getContextualArgument(opArgs[i]);
        }

        return contextualArgs;
    }

    internal Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>
        getContextualArgument(string opArg)
    {
        if (opArg.StartsWith('"') && opArg.EndsWith('"'))
        {
            return (i, p) => (j, r) => (k, c) => opArg.Substring(1, opArg.Length - 2);
        }

        if (opArg.StartsWith('$'))
        {
            opArg = MassParamEdit.massEditVars[opArg.Substring(1)].ToString();
        }

        var arg = opArg.Split(" ", 2);
        if (argumentGetters.ContainsKey(arg[0].Trim()))
        {
            OperationArgumentGetter getter = argumentGetters[arg[0]];
            var opArgArgs = arg.Length > 1 ? arg[1].Split(" ", getter.args.Length) : new string[0];
            if (opArgArgs.Length != getter.args.Length)
            {
                throw new Exception(
                    @$"Contextual value {arg[0]} has wrong number of arguments. Expected {opArgArgs.Length}");
            }

            for (var i = 0; i < opArgArgs.Length; i++)
            {
                if (opArgArgs[i].StartsWith('$'))
                {
                    opArgArgs[i] = MassParamEdit.massEditVars[opArgArgs[i].Substring(1)].ToString();
                }
            }

            return getter.func(opArgArgs);
        }

        return defaultGetter.func(new[] { opArg });
    }
}

internal class OperationArgumentGetter
{
    public string[] args;

    internal Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>>
        func;

    internal Func<bool> shouldShow;
    public string wiki;

    internal OperationArgumentGetter(string[] args, string wiki,
        Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>>
            func, Func<bool> shouldShow)
    {
        this.args = args;
        this.wiki = wiki;
        this.func = func;
        this.shouldShow = shouldShow;
    }
}
