#nullable enable
using Andre.Formats;
using DotNext.Collections.Generic;
using Org.BouncyCastle.Crypto.Engines;
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
    internal SearchEngine engine;
    // No arguments because this is handled separately in SearchEngine
    internal MEFilterStage(string toParse, int line, string stageName, SearchEngine stageEngine)
    {
        command = toParse.Trim();
        if (command.Equals(""))
        {
            throw new MEParseException($@"Could not find {stageName} filter. Add : and one of {string.Join(", ", stageEngine.AvailableCommandsForHelpText())}", line);
        }
        engine = stageEngine;
    }
}
internal struct MEOperationStage
{
    internal string command;
    internal string arguments;
    internal OperationCategory operation;
    internal MEOperationStage(string toParse, int line, string stageName, OperationCategory operationType)
    {
        var stage = toParse.TrimStart().Split(' ', 2);
        command = stage[0].Trim();
        if (stage.Length > 1)
            arguments = stage[1];
        if (command.Equals(""))
        {
            throw new MEParseException($@"Could not find operation to perform. Add : and one of {string.Join(' ', operationType.AllCommands().Keys)}", line);
        }
        if (!operationType.AllCommands().ContainsKey(command))
        {
            throw new MEParseException($@"Unknown {stageName} operation {command}", line);
        }
        operation = operationType;
    }
    internal string[] getArguments(int count)
    {
        return arguments.Split(':', count);
    }
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
    /* Current actions from execution of this edit command. */
    private List<EditorAction> _partialActions = new();

    private ParamBank bank;
    private ParamEditorSelectionState context;
    private object[] argFuncs;
    METypelessOperationDef parsedOp;

    List<MEFilterStage> filters = new();
    MEOperationStage operation;

    internal static ParamEditorSelectionState totalHackPleaseKillme = null;
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

                MassParamEditRegex currentEditData = new();
                currentEditData._currentLine = currentLine;
                currentEditData.bank = bank;
                currentEditData.context = context;
                totalHackPleaseKillme = context;

                MassEditResult result = currentEditData.ParseCommand(command);

                if (result.Type != MassEditResultType.SUCCESS)
                {
                    return (result, null);
                }

                List<EditorAction> actions = currentEditData._partialActions;

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
    private MassEditResult ParseCommand(string command)
    {
        var stage = command.Split(":", 2);

        Type currentType = typeof((bool, bool)); // Always start at boolbool type as basis
        string firstStage = stage[0];
        string firstStageKeyword = firstStage.Trim().Split(" ", 2)[0];

        var op = OperationCategory.GetEditOperation(currentType);
        // Try run an operation
        if (op != null && op.HandlesCommand(firstStageKeyword))
            return ParseOpStep(command, op.NameForHelpTexts(), op);

        var nextStage = SearchEngine.GetSearchEngines(currentType);
        // Try out each defined search engine for the current type
        foreach ((SearchEngine engine, Type t) in nextStage)
        {
            if (engine.HandlesCommand(firstStageKeyword))
            {
                return ParseFilterStep(command, engine);
            }
        }
        //Assume it's default search of last search option
        return ParseFilterStep(command, nextStage.Last().Item1);
    }

    private MassEditResult ParseFilterStep(string stageText, SearchEngine expectedSearchEngine)
    {
        var stage = stageText.Split(":", 2);
        string stageName = expectedSearchEngine.NameForHelpTexts();
        filters.Add(new MEFilterStage(stage[0], _currentLine, stageName, expectedSearchEngine));

        if (stage.Length < 2)
        {
            var esList = expectedSearchEngine.NextSearchEngines();
            var eo = expectedSearchEngine.NextOperation();
            if (esList.Any() && eo != null)
                return new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {esList.Last().Item1.NameForHelpTexts()} filter or {eo.NameForHelpTexts()} operation to perform. Check your colon placement. (line {_currentLine})");
            if (esList.Any())
                return new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {esList.Last().Item1.NameForHelpTexts()} filter. Check your colon placement. (line {_currentLine})");
            if (eo != null)
                return new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find {eo.NameForHelpTexts()} operation to perform. Check your colon placement. (line {_currentLine})");
            return new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find next stage to perform (no suggestions found). Check your colon placement. (line {_currentLine})");
        }

        Type currentType = expectedSearchEngine.getElementType();
        string restOfStages = stage[1];
        string nextStageKeyword = restOfStages.Trim().Split(" ", 2)[0];

        var op = OperationCategory.GetEditOperation(currentType);
        // Try run an operation
        if (op != null && op.HandlesCommand(nextStageKeyword))
            return ParseOpStep(stage[1], op.NameForHelpTexts(), op);

        var nextStage = SearchEngine.GetSearchEngines(currentType);
        // Try out each defined search engine for the current type
        foreach ((SearchEngine engine, Type t) in nextStage)
        {
            if (engine.HandlesCommand(nextStageKeyword))
                return ParseFilterStep(restOfStages, engine);
        }
        //Assume it's default search of last search option
        return ParseFilterStep(restOfStages, nextStage.Last().Item1);
    }
    private MassEditResult ParseOpStep(string stageText, string stageName, OperationCategory operation)
    {
        this.operation = new MEOperationStage(stageText, _currentLine, stageName, operation);

        parsedOp = operation.AllCommands()[this.operation.command];
        argFuncs = OperationArguments.arg.getContextualArguments(parsedOp.argNames.Length, this.operation.arguments);
        if (parsedOp.argNames.Length != argFuncs.Length)
        {
            return new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {this.operation.command} (line {_currentLine})");
        }
        var currentObj = (true, true);
        var contextObjects = new Dictionary<Type, (object, object)>() { { typeof(bool), currentObj} };

        if (filters.Count == 0)
            return SandboxMassEditExecution(() => ExecOp(this.operation, this.operation.command, argFuncs, currentObj, contextObjects, operation));
        else
        {
            int filterDepth = 0;
            MEFilterStage baseFilter = filters[filterDepth];
            return SandboxMassEditExecution(() => ExecStage(baseFilter, baseFilter.engine, filterDepth, currentObj, contextObjects, argFuncs));
        }
        throw new MEParseException("No initial stage or op was parsed", _currentLine);
    }

    private MassEditResult SandboxMassEditExecution(Func<MassEditResult> innerFunc)
    {
        try
        {
            return innerFunc();
        }
        catch (Exception e)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, @$"Error on line {_currentLine}" + '\n' + e.GetBaseException().ToString());
        }
    }

    private MassEditResult ExecStage(MEFilterStage info, SearchEngine engine, int filterDepth, (object, object) contextObject, Dictionary<Type, (object, object)> contextObjects, IEnumerable<object> argFuncs)
    {
        var editCount = -1;
        filterDepth++;
        var contexts = engine.GetStaticContextItems();
        foreach (var context in contexts)
        {
            argFuncs = argFuncs.Select((func, i) => func.tryFoldAsFunc(editCount, context.Item2));
        }
        foreach ((object, object) currentObject in engine.SearchNoType(contextObject, info.command, false, false))
        {
            editCount++;
            //add context
            contextObjects[engine.getElementType()] = currentObject;
            //update argGetters
            IEnumerable<object> newArgFuncs = argFuncs.Select((func, i) => func.tryFoldAsFunc(editCount, currentObject));
            //exec it
            MassEditResult res;

            if (filterDepth == filters.Count)
                res = ExecOp(operation, operation.command, argFuncs, currentObject, contextObjects, operation.operation);
            else
            {
                MEFilterStage nextFilter = filters[filterDepth];
                res = ExecStage(nextFilter, nextFilter.engine, filterDepth, currentObject, contextObjects, newArgFuncs);
            }
            if (res.Type != MassEditResultType.SUCCESS)
            {
                return res;
            }
        }

        return new MassEditResult(MassEditResultType.SUCCESS, "");
    }
    private MassEditResult ExecOp(MEOperationStage opInfo, string opName, IEnumerable<object> argFuncs, (object, object) currentObject, Dictionary<Type, (object, object)> contextObjects, OperationCategory opType)
    {
        var argValues = argFuncs.Select(f => f.assertCompleteContextOrThrow(_currentLine).ToParamEditorString()).ToArray();
        var opResult = parsedOp.function(currentObject.Item1, opType.GetElementValue(currentObject, contextObjects), argValues);
        ResultValidity res = opType.ValidateResult(opResult);
        if (res == ResultValidity.ERROR)
        {
            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Error performing {opName} operation {opInfo.command} (line {_currentLine})");
        }
        if (res == ResultValidity.OK)
        {
            opType.UseResult(_partialActions, currentObject, contextObjects, opResult);
        }
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
