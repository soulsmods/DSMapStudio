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
internal class MEOperationException : Exception
{
    public MEOperationException(string? message) : base(message)
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

    private Func<int, object[], string, Param.Row, List<EditorAction>,
        MassEditResult> rowOpOrCellStageFunc;
    private object[] paramArgFuncs;
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
                    (result, actions) = ParseFilterStep(currentLine, command, VarSearchEngine.vse, ref currentEditData.varStageInfo, (currentLine, restOfStages) =>
                    {
                        return currentEditData.ParseVarOpStep(currentLine, restOfStages);
                    });
                }
                else if (ParamAndRowSearchEngine.parse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = ParseFilterStep(currentLine, command, ParamAndRowSearchEngine.parse, ref currentEditData.paramRowStageInfo, (currentLine, restOfStages) =>
                    {
                        if (MERowOperation.rowOps.HandlesCommand(restOfStages.Trim().Split(" ", 2)[0]))
                        {
                            return currentEditData.ParseRowOpStep(currentLine, restOfStages);
                        }
                        return ParseFilterStep(currentLine, restOfStages, CellSearchEngine.cse, ref currentEditData.cellStageInfo, (currentLine, restOfStages) =>
                        {
                            currentEditData.rowOpOrCellStageFunc = currentEditData.ExecCellStage;
                            return currentEditData.ParseCellOpStep(currentLine, restOfStages);
                        });
                    });
                }
                else
                {
                    (result, actions) = ParseFilterStep(currentLine, command, ParamSearchEngine.pse, ref currentEditData.paramStageInfo, (currentLine, restOfStages) =>
                    {
                        return ParseFilterStep(currentLine, restOfStages, RowSearchEngine.rse, ref currentEditData.rowStageInfo, (currentLine, restOfStages) =>
                        {
                            if (MERowOperation.rowOps.HandlesCommand(restOfStages.Trim().Split(" ", 2)[0]))
                            {
                                return currentEditData.ParseRowOpStep(currentLine, restOfStages);
                            }
                            return ParseFilterStep(currentLine, restOfStages, CellSearchEngine.cse, ref currentEditData.cellStageInfo, (currentLine, restOfStages) =>
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

    private static (MassEditResult, List<EditorAction>) ParseFilterStep(int currentLine, string restOfStages, TypelessSearchEngine expectedSearchEngine, ref MEFilterStage target, Func<int, string, (MassEditResult, List<EditorAction>)> nextStageFunc)
    {
        var stage = restOfStages.Split(":", 2);
        string stageName = expectedSearchEngine.NameForHelpTexts();
        target = new MEFilterStage(stage[0], currentLine, stageName, expectedSearchEngine);

        if (stage.Length < 2)
        {
            var esList = expectedSearchEngine.NextSearchEngines();
            var eo = expectedSearchEngine.NextOperation();
            if (esList.Any() && eo != null)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {esList.Last().Item1.NameForHelpTexts()} filter or {eo.NameForHelpTexts()} operation to perform. Check your colon placement. (line {currentLine})"), null);
            if (esList.Any())
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {esList.Last().Item1.NameForHelpTexts()} filter. Check your colon placement. (line {currentLine})"), null);
            if (eo != null)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {eo.NameForHelpTexts()} operation to perform. Check your colon placement. (line {currentLine})"), null);
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find next stage to perform (no suggestions found). Check your colon placement. (line {currentLine})"), null);
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
            return (new MassEditResult(MassEditResultType.OPERATIONERROR, @$"Error on line {currentLine}" + '\n' + e.GetBaseException().ToString()), null);
        }

        return (new MassEditResult(MassEditResultType.SUCCESS, $@"{partialActions.Count} cells affected"),
            partialActions);
    }

    private MassEditResult ExecGlobalOp(int currentLine)
    {
        var globalArgValues = paramArgFuncs.Select(f => f.assertCompleteContextOrThrow(0).ToParamEditorString()).ToArray();
        var result = (bool)genericFunc(context, globalArgValues);
        if (!result)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Error performing global operation {globalOperationInfo} (line {currentLine})");
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecVarStage(int currentLine)
    {
        var varArgs = paramArgFuncs;
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

    private MassEditResult ExecVarOpStage(int currentLine, string var, object[] args)
    {
        MassParamEdit.massEditVars[var] = genericFunc(MassParamEdit.massEditVars[var], args.Select((x, i) => x.assertCompleteContextOrThrow(i).ToParamEditorString()).ToArray());
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
        IEnumerable<object> paramArgFunc =
            paramArgFuncs.Select((func, i) => func.tryFoldAsFunc(0, activeParam)); // technically invalid for clipboard
        var rowEditCount = -1;
        foreach ((MassEditRowSource source, Param.Row row) in ParamAndRowSearchEngine.parse.Search(context,
                     paramRowStageInfo.command, false, false))
        {
            rowEditCount++;
            object[] rowArgFunc =
                paramArgFunc.Select((rowFunc, i) => rowFunc.tryFoldAsFunc(rowEditCount, row)).ToArray();
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
            IEnumerable<object> paramArgFunc =
                paramArgFuncs.Select((func, i) => func.tryFoldAsFunc(paramEditCount, p));
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
        IEnumerable<object> paramArgFunc,
        string paramname, ParamBank b, Param p, List<EditorAction> partialActions)
    {
        var rowEditCount = -1;
        foreach ((string param, Param.Row row) in RowSearchEngine.rse.Search((b, p), rowStageInfo.command, false, false))
        {
            rowEditCount++;
            object[] rowArgFunc =
                paramArgFunc.Select((rowFunc, i) => rowFunc.tryFoldAsFunc(rowEditCount, row)).ToArray();
            MassEditResult res = rowOpOrCellStageFunc(currentLine, rowArgFunc, paramname, row, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecRowOp(int currentLine, object[] rowArgFunc, string paramname,
        Param.Row row, List<EditorAction> partialActions)
    {
        var rowArgValues = rowArgFunc.Select((argV, i) => argV.assertCompleteContextOrThrow(i).ToParamEditorString()).ToArray();
        (Param? p2, Param.Row? rs) = ((Param? p2, Param.Row? rs))genericFunc((paramname, row), rowArgValues);
        if (p2 == null)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {rowOperationInfo.command} {String.Join(' ', rowArgValues)} on row (line {currentLine})");
        }

        if (rs != null)
        {
            partialActions.Add(new AddParamsAction(p2, "FromMassEdit", new List<Param.Row> { rs }, false, true));
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecCellStage(int currentLine, object[] rowArgFunc,
        string paramname, Param.Row row, List<EditorAction> partialActions)
    {
        var cellEditCount = -1;
        foreach ((PseudoColumn, Param.Column) col in CellSearchEngine.cse.Search((paramname, row), cellStageInfo.command,
                     false, false))
        {
            cellEditCount++;
            var cellArgValues = rowArgFunc.Select((argV, i) => argV.tryFoldAsFunc(cellEditCount, col)).ToArray();
            MassEditResult res = ExecCellOp(currentLine, cellArgValues, paramname, row, col, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecCellOp(int currentLine, object[] cellArgValues, string paramname, Param.Row row,
        (PseudoColumn, Param.Column) col, List<EditorAction> partialActions)
    {
        object res = null;
        string errHelper = null;
        try
        {
            res = genericFunc(row.Get(col), cellArgValues.Select((x, i) => x.assertCompleteContextOrThrow(i).ToParamEditorString()).ToArray());
        }
        catch (FormatException e)
        {
            errHelper = "Type is not correct. Check the operation arguments result in usable values.";
        }
        catch (InvalidCastException e)
        {
            errHelper = "Cannot cast to correct type. Check the operation arguments result in usable values.";
        }
        catch (MEOperationException e)
        {
            errHelper = e.Message;
        }
        catch (Exception e)
        {
            errHelper = $@"Unknown error ({e.Message})";
        }
        string errorValue = null;
        if (res == null)
        {
            errorValue = cellArgValues.Length > 0 && cellArgValues[0] is not Delegate ? String.Join(' ', cellArgValues) : "[Undetermined value]";
        }

        if (res == null && col.Item1 == PseudoColumn.ID)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo.command} {errorValue} on ID ({errHelper}) (line {currentLine})");
        }

        if (res == null && col.Item1 == PseudoColumn.Name)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo.command} {errorValue} on Name ({errHelper}) (line {currentLine})");
        }

        if (res == null)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo.command} {errorValue} on field {col.Item2.Def.InternalName} ({errHelper}) (line {currentLine})");
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
