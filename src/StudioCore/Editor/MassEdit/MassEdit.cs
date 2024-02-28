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
    internal MEParseException(string? message, int line) : base($@"{message} (line {line})")
    {
    }
}
internal class MEOperationException : Exception
{
    internal MEOperationException(string? message) : base(message)
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
        if (stage.Length > 1)
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
    internal static Dictionary<string, object> massEditVars = new();

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

    internal static void AppendParamEditAction(this List<EditorAction> actions, Param.Row row,
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
    /* Line number associated with this edit command, for error reporting. */
    private int _currentLine;

    private ParamBank bank;
    private ParamEditorSelectionState context;
    private Func<object, string[], object> genericFunc;
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
                currentEditData._currentLine = currentLine;
                currentEditData.bank = bank;
                currentEditData.context = context;

                var primaryFilter = command.Split(':', 2)[0].Trim();

                if (MEGlobalOperation.globalOps.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = currentEditData.ParseOpStep(command, "global", MEGlobalOperation.globalOps, ref currentEditData.globalOperationInfo, ref currentEditData.argNames, ref currentEditData.genericFunc);
                }
                else if (VarSearchEngine.vse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = currentEditData.ParseFilterStep(command, VarSearchEngine.vse, ref currentEditData.varStageInfo, (restOfStages) =>
                    {
                        return currentEditData.ParseOpStep(restOfStages, "var", MEValueOperation.valueOps, ref currentEditData.varOperationInfo, ref currentEditData.argNames, ref currentEditData.genericFunc);
                    });
                }
                else if (ParamAndRowSearchEngine.parse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                {
                    (result, actions) = currentEditData.ParseFilterStep(command, ParamAndRowSearchEngine.parse, ref currentEditData.paramRowStageInfo, (restOfStages) =>
                    {
                        if (MERowOperation.rowOps.HandlesCommand(restOfStages.Trim().Split(" ", 2)[0]))
                        {
                            return currentEditData.ParseOpStep(restOfStages, "row", MERowOperation.rowOps, ref currentEditData.rowOperationInfo, ref currentEditData.argNames, ref currentEditData.genericFunc);
                        }
                        return currentEditData.ParseFilterStep(restOfStages, CellSearchEngine.cse, ref currentEditData.cellStageInfo, (restOfStages) =>
                        {
                            return currentEditData.ParseOpStep(restOfStages, "cell/property", MEValueOperation.valueOps, ref currentEditData.cellOperationInfo, ref currentEditData.argNames, ref currentEditData.genericFunc);
                        });
                    });
                }
                else
                {
                    (result, actions) = currentEditData.ParseFilterStep(command, ParamSearchEngine.pse, ref currentEditData.paramStageInfo, (restOfStages) =>
                    {
                        return currentEditData.ParseFilterStep(restOfStages, RowSearchEngine.rse, ref currentEditData.rowStageInfo, (restOfStages) =>
                        {
                            if (MERowOperation.rowOps.HandlesCommand(restOfStages.Trim().Split(" ", 2)[0]))
                            {
                                return currentEditData.ParseOpStep(restOfStages, "row", MERowOperation.rowOps, ref currentEditData.rowOperationInfo, ref currentEditData.argNames, ref currentEditData.genericFunc);
                            }
                            return currentEditData.ParseFilterStep(restOfStages, CellSearchEngine.cse, ref currentEditData.cellStageInfo, (restOfStages) =>
                            {
                                return currentEditData.ParseOpStep(restOfStages, "cell/property", MEValueOperation.valueOps, ref currentEditData.cellOperationInfo, ref currentEditData.argNames, ref currentEditData.genericFunc);
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

    private (MassEditResult, List<EditorAction>) ParseFilterStep(string restOfStages, TypelessSearchEngine expectedSearchEngine, ref MEFilterStage target, Func<string, (MassEditResult, List<EditorAction>)> nextStageFunc)
    {
        var stage = restOfStages.Split(":", 2);
        string stageName = expectedSearchEngine.NameForHelpTexts();
        target = new MEFilterStage(stage[0], _currentLine, stageName, expectedSearchEngine);

        if (stage.Length < 2)
        {
            var esList = expectedSearchEngine.NextSearchEngines();
            var eo = expectedSearchEngine.NextOperation();
            if (esList.Any() && eo != null)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {esList.Last().Item1.NameForHelpTexts()} filter or {eo.NameForHelpTexts()} operation to perform. Check your colon placement. (line {_currentLine})"), null);
            if (esList.Any())
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {esList.Last().Item1.NameForHelpTexts()} filter. Check your colon placement. (line {_currentLine})"), null);
            if (eo != null)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {eo.NameForHelpTexts()} operation to perform. Check your colon placement. (line {_currentLine})"), null);
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find next stage to perform (no suggestions found). Check your colon placement. (line {_currentLine})"), null);
        }

        return nextStageFunc(stage[1]);
    }
    private (MassEditResult, List<EditorAction>) ParseOpStep(string restOfStages, string stageName, METypelessOperation operation, ref MEOperationStage target, ref string[] argNameTarget, ref Func<object, string[], object> funcTarget)
    {
        target = new MEOperationStage(restOfStages, _currentLine, stageName, operation);

        var meOpDef = operation.AllCommands()[target.command];
        (argNameTarget, funcTarget) = (meOpDef.argNames, meOpDef.function);
        paramArgFuncs = MEOperationArgument.arg.getContextualArguments(argNames.Length, target.arguments);
        if (argNameTarget.Length != paramArgFuncs.Length)
        {
            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {globalOperationInfo.command} (line {_currentLine})"), null);
        }


        //hacks for now
        if (globalOperationInfo.command != null)
            return SandboxMassEditExecution(_currentLine, partials => ExecGlobalOp(_currentLine));
        if (varOperationInfo.command != null)
            return SandboxMassEditExecution(_currentLine, partials => ExecVarStage(_currentLine));

        if (paramRowStageInfo.command != null)
            return SandboxMassEditExecution(_currentLine, partials => ExecParamRowStage(_currentLine, partials));
        if (paramStageInfo.command != null)
            return SandboxMassEditExecution(_currentLine, partials => ExecParamStage(_currentLine, partials));
        throw new MEParseException("No initial stage or op was parsed", _currentLine);
    }

    private (MassEditResult, List<EditorAction>) SandboxMassEditExecution(int _currentLine,
        Func<List<EditorAction>, MassEditResult> innerFunc)
    {
        List<EditorAction> partialActions = new();
        try
        {
            return (innerFunc(partialActions), partialActions);
        }
        catch (Exception e)
        {
            return (new MassEditResult(MassEditResultType.OPERATIONERROR, @$"Error on line {_currentLine}" + '\n' + e.GetBaseException().ToString()), null);
        }

        return (new MassEditResult(MassEditResultType.SUCCESS, $@"{partialActions.Count} cells affected"),
            partialActions);
    }

    private MassEditResult ExecGlobalOp(int _currentLine)
    {
        var globalArgValues = paramArgFuncs.Select(f => f.assertCompleteContextOrThrow(0).ToParamEditorString()).ToArray();
        var result = (bool)genericFunc(context, globalArgValues);
        if (!result)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Error performing global operation {globalOperationInfo} (line {_currentLine})");
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecVarStage(int _currentLine)
    {
        var varArgs = paramArgFuncs;
        foreach (var varName in VarSearchEngine.vse.Search(false, varStageInfo.command, false, false))
        {
            MassEditResult res = ExecVarOpStage(_currentLine, varName, varArgs);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecVarOpStage(int _currentLine, string var, object[] args)
    {
        MassParamEdit.massEditVars[var] = genericFunc(MassParamEdit.massEditVars[var], args.Select((x, i) => x.assertCompleteContextOrThrow(i).ToParamEditorString()).ToArray());
        var result = true; // Anything that practicably can go wrong 
        if (!result)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Error performing var operation {varOperationInfo} (line {_currentLine})");
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecParamRowStage(int _currentLine, List<EditorAction> partialActions)
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

            MassEditResult res;
            if (rowOperationInfo.command != null)
                res = ExecRowOp(_currentLine, rowArgFunc, paramname, row, partialActions);
            else if (cellStageInfo.command != null)
                res = ExecCellStage(_currentLine, rowArgFunc, paramname, row, partialActions);
            else
                throw new MEParseException("No row op or cell stage was parsed", _currentLine);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecParamStage(int _currentLine, List<EditorAction> partialActions)
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
                return new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operationForPrint} (line {_currentLine})");
            }

            var paramname = b.GetKeyForParam(p);
            MassEditResult res = ExecRowStage(_currentLine, paramArgFunc, paramname, b, p, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecRowStage(int _currentLine,
        IEnumerable<object> paramArgFunc,
        string paramname, ParamBank b, Param p, List<EditorAction> partialActions)
    {
        var rowEditCount = -1;
        foreach ((string param, Param.Row row) in RowSearchEngine.rse.Search((b, p), rowStageInfo.command, false, false))
        {
            rowEditCount++;
            object[] rowArgFunc =
                paramArgFunc.Select((rowFunc, i) => rowFunc.tryFoldAsFunc(rowEditCount, row)).ToArray();
            MassEditResult res;
            if (rowOperationInfo.command != null)
                res = ExecRowOp(_currentLine, rowArgFunc, paramname, row, partialActions);
            else if (cellStageInfo.command != null)
                res = ExecCellStage(_currentLine, rowArgFunc, paramname, row, partialActions);
            else
                throw new MEParseException("No row op or cell stage was parsed", _currentLine);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecRowOp(int _currentLine, object[] rowArgFunc, string paramname,
        Param.Row row, List<EditorAction> partialActions)
    {
        var rowArgValues = rowArgFunc.Select((argV, i) => argV.assertCompleteContextOrThrow(i).ToParamEditorString()).ToArray();
        (Param? p2, Param.Row? rs) = ((Param? p2, Param.Row? rs))genericFunc((paramname, row), rowArgValues);
        if (p2 == null)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {rowOperationInfo.command} {String.Join(' ', rowArgValues)} on row (line {_currentLine})");
        }

        if (rs != null)
        {
            partialActions.Add(new AddParamsAction(p2, "FromMassEdit", new List<Param.Row> { rs }, false, true));
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecCellStage(int _currentLine, object[] rowArgFunc,
        string paramname, Param.Row row, List<EditorAction> partialActions)
    {
        var cellEditCount = -1;
        foreach ((PseudoColumn, Param.Column) col in CellSearchEngine.cse.Search((paramname, row), cellStageInfo.command,
                     false, false))
        {
            cellEditCount++;
            var cellArgValues = rowArgFunc.Select((argV, i) => argV.tryFoldAsFunc(cellEditCount, col)).ToArray();
            MassEditResult res = ExecCellOp(_currentLine, cellArgValues, paramname, row, col, partialActions);
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }

    private MassEditResult ExecCellOp(int _currentLine, object[] cellArgValues, string paramname, Param.Row row,
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
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo.command} {errorValue} on ID ({errHelper}) (line {_currentLine})");
        }

        if (res == null && col.Item1 == PseudoColumn.Name)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo.command} {errorValue} on Name ({errHelper}) (line {_currentLine})");
        }

        if (res == null)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperationInfo.command} {errorValue} on field {col.Item2.Def.InternalName} ({errHelper}) (line {_currentLine})");
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
