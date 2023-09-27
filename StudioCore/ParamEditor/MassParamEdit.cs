#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;
using FSParam;
using ImGuiNET;
using SoulsFormats;
using StudioCore.Editor;

namespace StudioCore.ParamEditor
{
    public enum MassEditResultType
    {
        SUCCESS,
        PARSEERROR,
        OPERATIONERROR
    }

    public class MassEditResult
    {
        public MassEditResultType Type;
        public string Information;
        public MassEditResult(MassEditResultType result, string info)
        {
            Type = result;
            Information = info;
        }
    }
    
    public static class MassParamEdit
    {
        public static Dictionary<string, object> massEditVars = new Dictionary<string, object>();

        internal static object WithDynamicOf(object instance, Func<dynamic, object> dynamicFunc)
        {
            try
            {
                return Convert.ChangeType(dynamicFunc(instance), instance.GetType());
            }
            catch
            {
                // Second try, handle byte[], and casts from numerical values to string which need parsing.
                object ret = dynamicFunc(instance.ToParamEditorString());
                if (instance.GetType() == typeof(byte[]))
                    ret = ParamUtils.Dummy8Read((string)ret, ((byte[])instance).Length);
                return Convert.ChangeType(ret, instance.GetType());
            }
        }

        public static void AppendParamEditAction(this List<EditorAction> actions, Param.Row row, (PseudoColumn, Param.Column) col, object newval)
        {
            if (col.Item1 == PseudoColumn.ID)
            {
                if (!row.ID.Equals(newval))
                    actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("ID"), -1, row, newval));
            }
            else if (col.Item1 == PseudoColumn.Name)
            {
                if (row.Name == null || !row.Name.Equals(newval))
                    actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, newval));
            }
            else
            {
                Param.Cell handle = row[col.Item2];
                if (!(handle.Value.Equals(newval) 
                || (handle.Value.GetType()==typeof(byte[]) 
                && ParamUtils.ByteArrayEquals((byte[])handle.Value, (byte[])newval))))
                    actions.Add(new PropertiesChangedAction(handle.GetType().GetProperty("Value"), -1, handle, newval));
            }
        }
    }

    public class MassParamEditRegex
    {
        public static (MassEditResult, ActionManager child) PerformMassEdit(ParamBank bank, string commandsString, ParamEditorSelectionState context)
        {
            try
            {
                string[] commands = commandsString.Split('\n');
                int changeCount = 0;
                ActionManager childManager = new ActionManager();
                foreach (string cmd in commands)
                {
                    string command = cmd;
                    if (command.StartsWith("##") || string.IsNullOrWhiteSpace(command))
                        continue;
                    if (command.EndsWith(';'))
                        command = command.Substring(0, command.Length-1);
                    (MassEditResult result, List<EditorAction> actions) = (null, null);

                    MassParamEditRegex currentEditData = new MassParamEditRegex();
                    currentEditData.bank = bank;
                    currentEditData.context = context;

                    string primaryFilter = command.Split(':', 2)[0].Trim();

                    if (MEGlobalOperation.globalOps.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                    {
                        (result, actions) = currentEditData.ParseGlobalOpStep(command);
                    }
                    else if (VarSearchEngine.vse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                    {
                        (result, actions) = currentEditData.ParseVarStep(command);
                    }
                    else if (ParamAndRowSearchEngine.parse.HandlesCommand(primaryFilter.Split(" ", 2)[0]))
                    {
                        (result, actions) = currentEditData.ParseParamRowStep(command);
                    }
                    else
                    {
                        (result, actions) = currentEditData.ParseParamStep(command);
                    }
                    if (result.Type != MassEditResultType.SUCCESS)
                        return (result, null);
                    changeCount += actions.Count;
                    childManager.ExecuteAction(new CompoundAction(actions));
                }
                return (new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected"), childManager);
            }
            catch (Exception e)
            {
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown parsing error: "+e.ToString()), null);
            }
        }

        // Do we want these variables? shouldn't they be contained?
        ParamBank bank;
        ParamEditorSelectionState context;

        // Parsing data
        string varSelector = null;

        string paramSelector = null;
        string rowSelector = null;
        string paramRowSelector = null;
        string cellSelector = null;

        string globalOperation = null;
        string rowOperation = null;
        string cellOperation = null;
        string varOperation = null;

        // Run data        
        string[] argNames;
        int argc;
        Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] paramArgFuncs;
        
        Func<ParamEditorSelectionState, string[], bool> globalFunc = null;
        Func<(string, Param.Row), string[], (Param, Param.Row)> rowFunc = null;
        Func<object, string[], object> genericFunc = null; 


        Func<Func<int, (PseudoColumn, Param.Column), string>[], string, Param.Row, List<EditorAction>, MassEditResult> rowOpOrCellStageFunc = null;

        private (MassEditResult, List<EditorAction>) ParseGlobalOpStep(string restOfStages)
        {
            string[] opStage = restOfStages.Split(" ", 2);
            globalOperation = opStage[0].Trim();
            if (!MEGlobalOperation.globalOps.operations.ContainsKey(globalOperation))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown global operation "+globalOperation), null);
            string wiki;
            (argNames, wiki, globalFunc) = MEGlobalOperation.globalOps.operations[globalOperation];
            ExecParamOperationArguments(opStage.Length > 1 ? opStage[1] : null);
            if (argc != paramArgFuncs.Length)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {globalOperation}"), null);
            return SandboxMassEditExecution((partials) => ExecGlobalOp());
        }

        private (MassEditResult, List<EditorAction>) ParseVarStep(string restOfStages)
        {
            string[] varstage = restOfStages.Split(":", 2);
            varSelector = varstage[0].Trim();
            if (varSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find variable filter. Add : and one of "+String.Join(", ", VarSearchEngine.vse.AvailableCommandsForHelpText())), null);
            if (varstage.Length < 2)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find var operation. Check your colon placement."), null);
            return ParseVarOpStep(varstage[1]);
        }
        private (MassEditResult, List<EditorAction>) ParseVarOpStep(string restOfStages)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            varOperation = operationstage[0].Trim();
            if (varOperation.Equals("") || !MEValueOperation.valueOps.operations.ContainsKey(varOperation))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
            string wiki;
            (argNames, wiki, genericFunc) = MEValueOperation.valueOps.operations[varOperation];
            ExecParamOperationArguments(operationstage.Length > 1 ? operationstage[1] : null);
            if (argc != paramArgFuncs.Length)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {varOperation}"), null);
            return SandboxMassEditExecution((partials) => ExecVarStage());
        }
        private (MassEditResult, List<EditorAction>) ParseParamRowStep(string restOfStages)
        {
            string[] paramrowstage = restOfStages.Split(":", 2);
            paramRowSelector = paramrowstage[0].Trim();
            if (paramRowSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find paramrow filter. Add : and one of "+String.Join(", ", ParamAndRowSearchEngine.parse.AvailableCommandsForHelpText())), null);
            if (paramrowstage.Length < 2)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell filter or row operation. Check your colon placement."), null);
            if (MERowOperation.rowOps.HandlesCommand(paramrowstage[1].Trim().Split(" ", 2)[0]))
                return ParseRowOpStep(paramrowstage[1]);
            else
                return ParseCellStep(paramrowstage[1]);
        }
        private (MassEditResult, List<EditorAction>) ParseParamStep(string restOfStages)
        {
            string[] paramstage = restOfStages.Split(":", 2);
            paramSelector = paramstage[0].Trim();
            if (paramSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find param filter. Add : and one of "+String.Join(", ", ParamSearchEngine.pse.AvailableCommandsForHelpText())), null);
            if (paramstage.Length < 2)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter. Check your colon placement."), null);
            return ParseRowStep(paramstage[1]);
        }
        private (MassEditResult, List<EditorAction>) ParseRowStep(string restOfStages)
        {
            string[] rowstage = restOfStages.Split(":", 2);
            rowSelector = rowstage[0].Trim();
            if (rowSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter. Add : and one of "+String.Join(", ", RowSearchEngine.rse.AvailableCommandsForHelpText())), null);
            if (rowstage.Length < 2)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell filter or row operation to perform. Check your colon placement."), null);
            if (MERowOperation.rowOps.HandlesCommand(rowstage[1].Trim().Split(" ", 2)[0]))
                return ParseRowOpStep(rowstage[1]);
            else
                return ParseCellStep(rowstage[1]);
        }
        private (MassEditResult, List<EditorAction>) ParseCellStep(string restOfStages)
        {
            string[] cellstage = restOfStages.Split(":", 2);
            cellSelector = cellstage[0].Trim();
            if (cellSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell/property filter. Add : and one of "+String.Join(", ", CellSearchEngine.cse.AvailableCommandsForHelpText())+" or Name (0 args)"), null);
            if (cellstage.Length < 2)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Check your colon placement."), null);
            rowOpOrCellStageFunc = ExecCellStage;
            return ParseCellOpStep(cellstage[1]);
        }
        private (MassEditResult, List<EditorAction>) ParseRowOpStep(string restOfStages)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            rowOperation = operationstage[0].Trim();
            if (!MERowOperation.rowOps.operations.ContainsKey(rowOperation))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown row operation "+rowOperation), null);
            string wiki;
            (argNames, wiki, rowFunc) = MERowOperation.rowOps.operations[rowOperation];
            ExecParamOperationArguments(operationstage.Length > 1 ? operationstage[1] : null);
            if (argc != paramArgFuncs.Length)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {rowOperation}"), null);
            rowOpOrCellStageFunc = ExecRowOp;
            return SandboxMassEditExecution((partials) => paramRowSelector != null ? ExecParamRowStage(partials) : ExecParamStage(partials));
        }
        private (MassEditResult, List<EditorAction>) ParseCellOpStep(string restOfStages)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            cellOperation = operationstage[0].Trim();

            if (cellOperation.Equals("") || !MEValueOperation.valueOps.operations.ContainsKey(cellOperation))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
            if (!MEValueOperation.valueOps.operations.ContainsKey(cellOperation))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown cell operation "+cellOperation), null);
            string wiki;
            (argNames, wiki, genericFunc) = MEValueOperation.valueOps.operations[cellOperation];
            ExecParamOperationArguments(operationstage.Length > 1 ? operationstage[1] : null);
            if (argc != paramArgFuncs.Length)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {cellOperation}"), null);
            return SandboxMassEditExecution((partials) => paramRowSelector != null ? ExecParamRowStage(partials) : ExecParamStage(partials));
        }
        private void ExecParamOperationArguments(string opargs)
        {
            argc = argNames.Length;
            paramArgFuncs = MEOperationArgument.arg.getContextualArguments(argc, opargs);
        }
        private (MassEditResult, List<EditorAction>) SandboxMassEditExecution(Func<List<EditorAction>, MassEditResult> innerFunc)
        {
            List<EditorAction> partialActions = new List<EditorAction>();
            try
            {
                return (innerFunc(partialActions), partialActions);
            }
            catch (Exception e)
            {
                return (new MassEditResult(MassEditResultType.OPERATIONERROR, e.ToString()), null);
            }
            return (new MassEditResult(MassEditResultType.SUCCESS, $@"{partialActions.Count} cells affected"), partialActions);
        }
        private MassEditResult ExecGlobalOp()
        {
            string[] globalArgValues = paramArgFuncs.Select((f) => f(-1, null)(-1, null)(-1, (PseudoColumn.None, null))).ToArray();
            bool result = globalFunc(context, globalArgValues);
            if (!result)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, "performing global operation "+globalOperation);
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecVarStage()
        {;
            var varArgs = paramArgFuncs.Select((func, i) => func(-1, null)(-1, null)(-1, (PseudoColumn.None, null))).ToArray();
            foreach (string varName in VarSearchEngine.vse.Search(false, varSelector, false, false))
            {
                var res = ExecVarOpStage(varName, varArgs);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecVarOpStage(string var, string[] args)
        {
            MassParamEdit.massEditVars[var] = genericFunc(MassParamEdit.massEditVars[var], args);
            bool result = true; // Anything that practicably can go wrong 
            if (!result)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, "performing var operation "+varOperation);
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecParamRowStage(List<EditorAction> partialActions)
        {
            Param activeParam = bank.Params[context.getActiveParam()];
            var paramArgFunc = paramArgFuncs.Select((func, i) => func(0, activeParam)); // technically invalid for clipboard
            int rowEditCount = -1;
            foreach ((MassEditRowSource source, Param.Row row) in ParamAndRowSearchEngine.parse.Search(context, paramRowSelector, false, false))
            {
                rowEditCount++;
                var rowArgFunc = paramArgFunc.Select((rowFunc, i) => rowFunc(rowEditCount, row)).ToArray();
                string paramname = source == MassEditRowSource.Selection ? context.getActiveParam() : ParamBank.ClipboardParam;
                var res = rowOpOrCellStageFunc(rowArgFunc, paramname, row, partialActions);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecParamStage(List<EditorAction> partialActions)
        {
            int paramEditCount = -1;
            string operationForPrint = rowOperation != null ? rowOperation : cellOperation;
            foreach ((ParamBank b, Param p) in ParamSearchEngine.pse.Search(false, paramSelector, false, false))
            {
                paramEditCount++;
                var paramArgFunc = paramArgFuncs.Select((func, i) => func(paramEditCount, p));
                if (argc != paramArgFuncs.Length)
                    return new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operationForPrint}");
                string paramname = b.GetKeyForParam(p);
                var res = ExecRowStage(paramArgFunc, paramname, b, p, partialActions);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecRowStage(IEnumerable<Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>> paramArgFunc, string paramname, ParamBank b, Param p, List<EditorAction> partialActions)
        {
            int rowEditCount = -1;
            foreach (Param.Row row in RowSearchEngine.rse.Search((b, p), rowSelector, false, false))
            {
                rowEditCount++;
                var rowArgFunc = paramArgFunc.Select((rowFunc, i) => rowFunc(rowEditCount, row)).ToArray();
                var res = rowOpOrCellStageFunc(rowArgFunc, paramname, row, partialActions);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecRowOp(Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc, string paramname, Param.Row row, List<EditorAction> partialActions)
        {
            var rowArgValues = rowArgFunc.Select((argV, i) => argV(-1, (PseudoColumn.None, null))).ToArray();
            var (p2, rs) = rowFunc((paramname, row), rowArgValues);
            if (p2 == null)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {rowOperation} {String.Join(' ', rowArgValues)} on row");
            if (rs != null)
                partialActions.Add(new AddParamsAction(p2, "FromMassEdit", new List<Param.Row>{rs}, false, true));
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecCellStage(Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc, string paramname, Param.Row row, List<EditorAction> partialActions)
        {
            int cellEditCount = -1;
            foreach ((PseudoColumn, Param.Column) col in CellSearchEngine.cse.Search((paramname, row), cellSelector, false, false))
            {
                cellEditCount++;
                var cellArgValues = rowArgFunc.Select((argV, i) => argV(cellEditCount, col)).ToArray();
                var res = ExecCellOp(cellArgValues, paramname, row, col, partialActions);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private MassEditResult ExecCellOp(string[] cellArgValues, string paramname, Param.Row row, (PseudoColumn, Param.Column) col, List<EditorAction> partialActions)
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
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperation} {String.Join(' ', cellArgValues)} on ID ({errHelper})");
            else if (res == null && col.Item1 == PseudoColumn.Name)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperation} {String.Join(' ', cellArgValues)} on Name ({errHelper})");
            else if (res == null)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {cellOperation} {String.Join(' ', cellArgValues)} on field {col.Item2.Def.InternalName} ({errHelper})");
            partialActions.AppendParamEditAction(row, col, res);
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
    }

    public class MassParamEditOther
    {
        public static AddParamsAction SortRows(ParamBank bank, string paramName)
        {
            Param param = bank.Params[paramName];
            List<Param.Row> newRows = new List<Param.Row>(param.Rows);
            newRows.Sort((Param.Row a, Param.Row b)=>{return a.ID - b.ID;});
            return new AddParamsAction(param, paramName, newRows, true, true); //appending same params and allowing overwrite
        }
    }

    public class MEOperation<T, O>
    {
        internal Dictionary<string, (string[], string, Func<T, string[], O>)> operations = new Dictionary<string, (string[], string, Func<T, string[], O>)>();
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
        public List<(string, string[], string)> AvailableCommands()
        {
            List<(string, string[], string)> options = new List<(string, string[], string)>();
            foreach (string op in operations.Keys)
            {
                options.Add((op, operations[op].Item1, operations[op].Item2));
            }
            return options;
        }

    }
    public class MEGlobalOperation : MEOperation<ParamEditorSelectionState, bool>
    {
        public static MEGlobalOperation globalOps = new MEGlobalOperation();
        internal override void Setup()
        {
            operations.Add("clear", (new string[0], "Clears clipboard param and rows", (selectionState, args) => {
                ParamBank.ClipboardParam = null;
                ParamBank.ClipboardRows.Clear();
                return true;
            }));
            operations.Add("newvar", (new string[]{"variable name", "value"}, "Creates a variable with the given value, and the type of that value", (selectionState, args) => {
                int asInt;
                double asDouble;
                if (int.TryParse(args[1], out asInt))
                    MassParamEdit.massEditVars[args[0]] = asInt;
                else if (double.TryParse(args[1], out asDouble))
                    MassParamEdit.massEditVars[args[0]] = asDouble;
                else
                    MassParamEdit.massEditVars[args[0]] = args[1];
                return true;
            }));
            operations.Add("clearvars", (new string[0], "Deletes all variables", (selectionState, args) => {
                MassParamEdit.massEditVars.Clear();
                return true;
            }));
        }
    }
    public class MERowOperation : MEOperation<(string, Param.Row), (Param, Param.Row)>
    {
        public static MERowOperation rowOps = new MERowOperation();
        internal override void Setup()
        {
            operations.Add("copy", (new string[0], "Adds the selected rows into clipboard. If the clipboard param is different, the clipboard is emptied first", (paramAndRow, args) => {
                string paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                    throw new Exception($@"Could not locate param");
                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                    throw new Exception($@"Could not locate param {paramKey}");
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                // Only supporting single param in clipboard
                if (ParamBank.ClipboardParam != paramKey)
                {
                    ParamBank.ClipboardParam = paramKey;
                    ParamBank.ClipboardRows.Clear();
                }
                ParamBank.ClipboardRows.Add(new Param.Row(row, p));
                return (p, null);
            }));
            operations.Add("copyN", (new string[]{"count"}, "Adds the selected rows into clipboard the given number of times. If the clipboard param is different, the clipboard is emptied first", (paramAndRow, args) => {
                string paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                    throw new Exception($@"Could not locate param");
                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                    throw new Exception($@"Could not locate param {paramKey}");
                uint count = uint.Parse(args[0]);
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                // Only supporting single param in clipboard
                if (ParamBank.ClipboardParam != paramKey)
                {
                    ParamBank.ClipboardParam = paramKey;
                    ParamBank.ClipboardRows.Clear();
                }
                for (int i=0; i<count; i++)
                    ParamBank.ClipboardRows.Add(new Param.Row(row, p));
                return (p, null);
            }));
            operations.Add("paste", (new string[0], "Adds the selected rows to the primary regulation or parambnd in the selected param", (paramAndRow, args) => {
                string paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                    throw new Exception($@"Could not locate param");
                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                    throw new Exception($@"Could not locate param {paramKey}");
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                return (p, new Param.Row(row, p));
            }));         
        }
    }
    public class MEValueOperation : MEOperation<object, object>
    {
        public static MEValueOperation valueOps = new MEValueOperation();
        internal override void Setup()
        {
            operations.Add("=", (new string[]{"number or text"}, "Assigns the given value to the selected values. Will attempt conversion to the value's data type", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => args[0])));
            operations.Add("+", (new string[]{"number or text"}, "Adds the number to the selected values, or appends text if that is the data type of the values", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => {
                double val;
                if (double.TryParse(args[0], out val))
                    return v + val;
                return v + args[0];
                })));
            operations.Add("-", (new string[]{"number"}, "Subtracts the number from the selected values", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v - double.Parse(args[0]))));
            operations.Add("*", (new string[]{"number"}, "Multiplies selected values by the number", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v * double.Parse(args[0]))));
            operations.Add("/", (new string[]{"number"}, "Divides the selected values by the number", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v / double.Parse(args[0]))));
            operations.Add("%", (new string[]{"number"}, "Gives the remainder when the selected values are divided by the number", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v % double.Parse(args[0]))));
            operations.Add("scale", (new string[]{"factor number", "center number"}, "Multiplies the difference between the selected values and the center number by the factor number", (ctx, args) => {
                double opp1 = double.Parse(args[0]);
                double opp2 = double.Parse(args[1]);
                return MassParamEdit.WithDynamicOf(ctx, (v) => {
                    return (v - opp2) * opp1 + opp2;
                });
            }));
            operations.Add("replace", (new string[]{"text to replace", "new text"}, "Interprets the selected values as text and replaces all occurances of the text to replace with the new text", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v.Replace(args[0], args[1]))));
            operations.Add("replacex", (new string[]{"text to replace (regex)", "new text (w/ groups)"}, "Interprets the selected values as text and replaces all occurances of the given regex with the replacement, supporting regex groups", (ctx, args) => {
                Regex rx = new Regex(args[0]);
                return MassParamEdit.WithDynamicOf(ctx, (v) => rx.Replace(v, args[1]));
            }));
            operations.Add("max", (new string[]{"number"}, "Returns the larger of the current value and number", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => Math.Max(v, double.Parse(args[0])))));
            operations.Add("min", (new string[]{"number"}, "Returns the smaller of the current value and number", (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => Math.Min(v, double.Parse(args[0])))));
        }
    }
    public class MEOperationArgument
    {
        public static MEOperationArgument arg = new MEOperationArgument();
        Dictionary<string, OperationArgumentGetter> argumentGetters = new Dictionary<string, OperationArgumentGetter>();
        OperationArgumentGetter defaultGetter;

        private MEOperationArgument()
        {
            Setup();
        }
        private OperationArgumentGetter newGetter(string[] args, string wiki, Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>> func, Func<bool> shouldShow = null)
        {
            return new OperationArgumentGetter(args, wiki, func, shouldShow);
        }
        private void Setup()
        {
            defaultGetter = newGetter(new string[0], "Gives the specified value", (value) => (i, param) => (j, row) => (k, col) => value[0]);
            argumentGetters.Add("self", newGetter(new string[0], "Gives the value of the currently selected value", (empty) => (i, param) => (j, row) => (k, col) => {
                return row.Get(col).ToParamEditorString();
            }));
            argumentGetters.Add("field", newGetter(new string[]{"field internalName"}, "Gives the value of the given cell/field for the currently selected row and param", (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                return (j, row) => {
                    string v = row.Get(col).ToParamEditorString();
                    return (k, c) => v;
                };
            }));
            argumentGetters.Add("vanilla", newGetter(new string[0], "Gives the value of the equivalent cell/field in the vanilla regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added", (empty) => {
                ParamBank bank = ParamBank.VanillaBank;
                return (i, param) => {
                    string paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                        throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                    Param vParam = bank.Params[paramName];
                    return (j, row) => {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                            throw new Exception($@"Could not locate vanilla row {row.ID}");
                        return (k, col) => {
                            if (col.Item1 == PseudoColumn.None && col.Item2 == null)
                                throw new Exception($@"Could not locate given field or property");
                            return vRow.Get(col).ToParamEditorString();
                        };
                    };
                };
            }));
            argumentGetters.Add("aux", newGetter(new string[]{"parambank name"}, "Gives the value of the equivalent cell/field in the specified regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*", (bankName) => {
                if (!ParamBank.AuxBanks.ContainsKey(bankName[0]))
                    throw new Exception($@"Could not locate paramBank {bankName[0]}");
                ParamBank bank = ParamBank.AuxBanks[bankName[0]];
                return (i, param) => {
                    string paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    Param vParam = bank.Params[paramName];
                    return (j, row) => {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        return (k, col) => {
                            if (!col.IsColumnValid())
                                throw new Exception($@"Could not locate given field or property");
                            return vRow.Get(col).ToParamEditorString();
                        };
                    };
                };
            }, ()=>ParamBank.AuxBanks.Count > 0));
            argumentGetters.Add("vanillafield", newGetter(new string[]{"field internalName"}, "Gives the value of the specified cell/field in the vanilla regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added", (field) => (i, param) => {
                var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                var vParam = ParamBank.VanillaBank.GetParamFromName(paramName);
                if (vParam == null)
                    throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                var col = vParam.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                return (j, row) => {
                    Param.Row vRow = vParam?[row.ID];
                    if (vRow == null)
                        throw new Exception($@"Could not locate vanilla row {row.ID}");
                    string v = vRow.Get(col).ToParamEditorString();
                    return (k, c) => v;
                };
            }));
            argumentGetters.Add("auxfield", newGetter(new string[]{"parambank name", "field internalName"}, "Gives the value of the specified cell/field in the specified regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*", (bankAndField) => {
                if (!ParamBank.AuxBanks.ContainsKey(bankAndField[0]))
                    throw new Exception($@"Could not locate paramBank {bankAndField[0]}");
                ParamBank bank = ParamBank.AuxBanks[bankAndField[0]];
                return (i, param) => {
                    string paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    Param vParam = bank.Params[paramName];
                    var col = vParam.GetCol(bankAndField[1]);
                    if (!col.IsColumnValid())
                        throw new Exception($@"Could not locate field {bankAndField[1]}");
                    return (j, row) => {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        string v = vRow.Get(col).ToParamEditorString();
                        return (k, c) => v;
                    };
                };
            }, ()=>ParamBank.AuxBanks.Count > 0));
            argumentGetters.Add("paramlookup", newGetter(new string[]{"param name", "row id", "field name"}, "Returns the specific value specified by the exact param, row and field.", (address) => {
                Param param = ParamBank.PrimaryBank.Params[address[0]];
                int id = int.Parse(address[1]);
                var field = param.GetCol(address[2]);
                var value = param[id].Get(field).ToParamEditorString();
                return (i, param) => (j, row) => (k, col) => value;
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("average", newGetter(new string[]{"field internalName", "row selector"}, "Gives the mean value of the cells/fields found using the given selector, for the currently selected param", (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                Type colType = col.GetColumnType();
                if (colType == typeof(string) || colType == typeof(byte[]))
                    throw new Exception($@"Cannot average field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var vals = rows.Select((row, i) =>row.Get(col));
                double avg = vals.Average((val) => Convert.ToDouble(val));
                return (j, row) => (k, c) => avg.ToString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("median", newGetter(new string[]{"field internalName", "row selector"}, "Gives the median value of the cells/fields found using the given selector, for the currently selected param", (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var vals = rows.Select((row, i) => row.Get(col));
                var avg = vals.OrderBy((val) => Convert.ToDouble(val)).ElementAt(vals.Count()/2);
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("mode", newGetter(new string[]{"field internalName", "row selector"}, "Gives the most common value of the cells/fields found using the given selector, for the currently selected param", (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var avg = ParamUtils.GetParamValueDistribution(rows, col).OrderByDescending((g) => g.Item2).First().Item1;
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("min", newGetter(new string[]{"field internalName", "row selector"}, "Gives the smallest value from the cells/fields found using the given param, row selector and field", (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var min = rows.Min((r) => r[field[0]].Value.Value);
                return (j, row) => (k, c) => min.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("max", newGetter(new string[]{"field internalName", "row selector"}, "Gives the largest value from the cells/fields found using the given param, row selector and field", (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var max = rows.Max((r) => r[field[0]].Value.Value);
                return (j, row) => (k, c) => max.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("random", newGetter(new string[]{"minimum number (inclusive)", "maximum number (exclusive)"}, "Gives a random decimal number between the given values for each selected value", (minAndMax) => {
                double min;
                double max;
                if (!double.TryParse(minAndMax[0], out min) || !double.TryParse(minAndMax[1], out max))
                    throw new Exception($@"Could not parse min and max random values");
                if (max <= min)
                    throw new Exception($@"Random max must be greater than min");
                double range = max - min;
                return (i, param) => (j, row) => (k, c) => (Random.Shared.NextDouble() * range + min).ToString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("randint", newGetter(new string[]{"minimum integer (inclusive)", "maximum integer (inclusive)"}, "Gives a random integer between the given values for each selected value", (minAndMax) => {
                int min;
                int max;
                if (!int.TryParse(minAndMax[0], out min) || !int.TryParse(minAndMax[1], out max))
                    throw new Exception($@"Could not parse min and max randint values");
                if (max <= min)
                    throw new Exception($@"Random max must be greater than min");
                return (i, param) => (j, row) => (k, c) => Random.Shared.NextInt64(min, max + 1).ToString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("randFrom", newGetter(new string[]{"param name", "field internalName", "row selector"}, "Gives a random value from the cells/fields found using the given param, row selector and field, for each selected value", (paramFieldRowSelector) => {
                Param srcParam = ParamBank.PrimaryBank.Params[paramFieldRowSelector[0]];
                List<Param.Row> srcRows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, srcParam), paramFieldRowSelector[2], false, false);
                object[] values = srcRows.Select((r, i) => r[paramFieldRowSelector[1]].Value.Value).ToArray();
                return (i, param) => (j, row) => (k, c) => values[Random.Shared.NextInt64(values.Length)].ToString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("paramIndex", newGetter(new string[0], "Gives an integer for the current selected param, beginning at 0 and increasing by 1 for each param selected", (empty) => (i, param) => (j, row) => (k, col) => {
                return i.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("rowIndex", newGetter(new string[0], "Gives an integer for the current selected row, beginning at 0 and increasing by 1 for each row selected", (empty) => (i, param) => (j, row) => (k, col) => {
                return j.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            argumentGetters.Add("fieldIndex", newGetter(new string[0], "Gives an integer for the current selected cell/field, beginning at 0 and increasing by 1 for each cell/field selected", (empty) => (i, param) => (j, row) => (k, col) => {
                return k.ToParamEditorString();
            }, ()=>CFG.Current.Param_AdvancedMassedit));
        }

        public List<(string, string[])> AllArguments()
        {
            List<(string, string[])> options = new List<(string, string[])>();
            foreach (string op in argumentGetters.Keys)
            {
                options.Add((op, argumentGetters[op].args));
            }
            return options;
        }
        public List<(string, string, string[])> VisibleArguments()
        {
            List<(string, string, string[])> options = new List<(string, string, string[])>();
            foreach (string op in argumentGetters.Keys)
            {
                OperationArgumentGetter oag = argumentGetters[op];
                if (oag.shouldShow == null || oag.shouldShow())
                    options.Add((op, oag.wiki, oag.args));
            }
            return options;
        }
        internal Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] getContextualArguments(int argumentCount, string opData)
        {
            string[] opArgs = opData == null ? new string[0] : opData.Split(':', argumentCount);
            Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] contextualArgs = new Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[opArgs.Length];
            for (int i=0; i<opArgs.Length; i++)
            {
                contextualArgs[i] = getContextualArgument(opArgs[i]);
            }
            return contextualArgs;
        }
        internal Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>> getContextualArgument(string opArg)
        {
            if (opArg.StartsWith('"') && opArg.EndsWith('"'))
                return (i, p) => (j, r) => (k, c) => opArg.Substring(1, opArg.Length-2);
            if (opArg.StartsWith('$'))
                opArg = MassParamEdit.massEditVars[opArg.Substring(1)].ToString();
            string[] arg = opArg.Split(" ", 2);
            if (argumentGetters.ContainsKey(arg[0].Trim()))
            {
                var getter = argumentGetters[arg[0]];
                string[] opArgArgs = arg.Length > 1 ? arg[1].Split(" ", getter.args.Length) : new string[0];
                if (opArgArgs.Length != getter.args.Length)
                    throw new Exception(@$"Contextual value {arg[0]} has wrong number of arguments. Expected {opArgArgs.Length}");
                for (int i=0; i<opArgArgs.Length; i++)
                {
                    if (opArgArgs[i].StartsWith('$'))
                        opArgArgs[i] = MassParamEdit.massEditVars[opArgArgs[i].Substring(1)].ToString();
                }
                return getter.func(opArgArgs);
            }
            else
            {
                return defaultGetter.func(new string[]{opArg});
            }
        }
    }
    class OperationArgumentGetter
    {
        public string[] args;
        public string wiki;
        internal Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>> func;
        internal Func<bool> shouldShow;
        internal OperationArgumentGetter(string[] args, string wiki, Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>> func, Func<bool> shouldShow)
        {
            this.args = args;
            this.wiki = wiki;
            this.func = func;
            this.shouldShow = shouldShow;
        }
    }
}