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
using StudioCore.ParamEditor;

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
    
    public class MassParamEdit
    {
        public static Dictionary<string, string> massEditVars = new Dictionary<string, string>();

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

        internal static void addAction(Param.Row row, (PseudoColumn, Param.Column) col, object newval, List<EditorAction> actions)
        {
            if (col.Item1 == PseudoColumn.ID)
            {
                if (!row.ID.Equals(newval))
                    actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("ID"), -1, row, newval));
            }
            else if (col.Item1 == PseudoColumn.Name)
            {
                if (!row.Name.Equals(newval))
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

    public class MassParamEditRegex : MassParamEdit
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
                    if (command.EndsWith(';'))
                        command = command.Substring(0, command.Length-1);
                    (MassEditResult result, List<EditorAction> actions) = (null, null);
                    if (MEGlobalOperation.globalOps.HandlesCommand(command.Split(" ", 2)[0]))
                    {
                        string[] opStage = command.Split(" ", 2);
                        string op = opStage[0].Trim();
                        (result, actions) = PerformMassEditCommand(bank, false, null, null, null, op, opStage.Length > 1 ? opStage[1] : null, context);
                    }
                    else if (VarSearchEngine.vse.HandlesCommand(command.Split(" ", 2)[0]))
                    {

                    }
                    else
                    {
                        (result, actions) = PerformMassEditCommandParamStep(bank, command, context);
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
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandVarStep(ParamBank bank, string restOfStages, ParamEditorSelectionState context)
        {
            string[] varstage = restOfStages.Split(":", 2);
            string varSelector = varstage[0].Trim();
            if (varSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find variable filter. Add : and one of "+String.Join(", ", VarSearchEngine.vse.AvailableCommandsForHelpText())), null);
            return PerformMassEditCommandVarOpStep(bank, varSelector, varstage[1], context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandVarOpStep(ParamBank bank, string varSelector, string restOfStages, ParamEditorSelectionState context)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            string operation = operationstage[0].Trim();

            if (operation.Equals("") || !MEValueOperation.valueOps.operations.ContainsKey(operation))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
            // TODO: add var selector to big command, implement usage
            return PerformMassEditCommand(bank, false, null, null, null, operation, operationstage.Length > 1 ? operationstage[1] : null, context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandParamStep(ParamBank bank, string restOfStages, ParamEditorSelectionState context)
        {
            string[] paramstage = restOfStages.Split(":", 2);
            string paramSelector = paramstage[0].Trim();
            if (paramSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find param filter. Add : and one of "+String.Join(", ", ParamSearchEngine.pse.AvailableCommandsForHelpText())+" or "+String.Join(", ", ParamAndRowSearchEngine.parse.AvailableCommandsForHelpText())), null);
            if (!ParamAndRowSearchEngine.parse.HandlesCommand(paramSelector))
            {
                return PerformMassEditCommandRowStep(bank, paramSelector, paramstage[1], context);
            }
            else
            {
                return PerformMassEditCommandCellStep(bank, true, paramSelector, null, paramstage[1], context);
            }
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandRowStep(ParamBank bank, string paramStage, string restOfStages, ParamEditorSelectionState context)
        {
            string[] rowstage = restOfStages.Split(":", 2);
            string rowSelector = rowstage[0].Trim();
            if (rowSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter. Add : and one of "+String.Join(", ", RowSearchEngine.rse.AvailableCommandsForHelpText())), null);
            return PerformMassEditCommandCellStep(bank, false, paramStage, rowSelector, rowstage[1], context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandCellStep(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string restOfStages, ParamEditorSelectionState context)
        {
            string[] cellstage = restOfStages.Split(":", 2);
            string cellSelector = cellstage[0].Trim();
            if (cellSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell/property filter. Add : and one of "+String.Join(", ", CellSearchEngine.cse.AvailableCommandsForHelpText())+" or Name (0 args)"), null);
            
            if (MERowOperation.rowOps.HandlesCommand(cellSelector.Split(" ", 2)[0]))
            {
                return PerformMassEditCommandRowOpStep(bank, isParamRowSelector, paramSelector, rowSelector, restOfStages, context);
            }
            else
            {
                return PerformMassEditCommandCellOpStep(bank, isParamRowSelector, paramSelector, rowSelector, cellSelector, cellstage[1], context);
            }
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandRowOpStep(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string restOfStages, ParamEditorSelectionState context)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            string operation = operationstage[0].Trim();
            //if (operationstage.Length == 1)
            //    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation arguments."), null);
            return PerformMassEditCommand(bank, isParamRowSelector, paramSelector, rowSelector, null, operation, operationstage.Length > 1 ? operationstage[1] : null, context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandCellOpStep(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string cellSelector, string restOfStages, ParamEditorSelectionState context)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            string operation = operationstage[0].Trim();

            if (operation.Equals("") || !MEValueOperation.valueOps.operations.ContainsKey(operation))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
            //if (operationstage.Length == 1)
            //    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation arguments. Add a value, or 'field' followed by the name of a field to take the value from"), null);
            return PerformMassEditCommand(bank, isParamRowSelector, paramSelector, rowSelector, cellSelector, operation, operationstage.Length > 1 ? operationstage[1] : null, context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommand(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string cellSelector, string operation, string opargs, ParamEditorSelectionState context)
        {
            List<EditorAction> partialActions = new List<EditorAction>();
            try {
                string[] argNames;
                int argc;
                Func<ParamEditorSelectionState, string[], bool> globalFunc = null;
                Func<(string, Param.Row), string[], (Param, Param.Row)> rowFunc = null;
                Func<object, string[], object> cellFunc = null; 

                if (paramSelector == null)
                {
                    if (!MEGlobalOperation.globalOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown global operation "+operation), null);
                    (argNames, globalFunc) = MEGlobalOperation.globalOps.operations[operation];
                }
                else if (cellSelector == null)
                {
                    if (!MERowOperation.rowOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown row operation "+operation), null);
                    (argNames, rowFunc) = MERowOperation.rowOps.operations[operation];
                }
                else
                {
                    if (!MEValueOperation.valueOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown cell operation "+operation), null);
                    (argNames, cellFunc) = MEValueOperation.valueOps.operations[operation];
                }
                argc = argNames.Length;
                var argFuncs = MEOperationArgument.arg.getContextualArguments(argc, opargs);

                if (paramSelector == null)
                {
                    return (ExecGlobalOp(argFuncs, globalFunc, context, operation), new List<EditorAction>());
                }
                if (isParamRowSelector)
                {
                    return (ExecParamRowStage(argFuncs, rowFunc, cellFunc, argc, bank, context, paramSelector, cellSelector, operation, partialActions), partialActions);
                }
                else
                {
                    return (ExecParamStage(argFuncs, rowFunc, cellFunc, argc, paramSelector, rowSelector, cellSelector, operation, partialActions), partialActions);
                }
            }
            catch (Exception e)
            {
                return (new MassEditResult(MassEditResultType.OPERATIONERROR, e.ToString()), null);
            }
            return (new MassEditResult(MassEditResultType.SUCCESS, $@"{partialActions.Count} cells affected"), partialActions);
        }
        private static MassEditResult ExecGlobalOp(Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] argFuncs, Func<ParamEditorSelectionState, string[], bool> globalFunc, ParamEditorSelectionState context, string operation)
        {
            string[] globalArgValues = argFuncs.Select((f) => f(-1, null)(-1, null)(-1, (PseudoColumn.None, null))).ToArray();
            bool result = globalFunc(context, globalArgValues);
            if (!result)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, "performing global operation "+operation);
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private static MassEditResult ExecParamRowStage(Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] argFuncs, Func<(string, Param.Row), string[], (Param, Param.Row)> rowFunc, Func<object, string[], object> cellFunc, int argc, ParamBank bank, ParamEditorSelectionState context, string paramSelector, string cellSelector, string operation, List<EditorAction> partialActions)
        {
            Param activeParam = bank.Params[context.getActiveParam()];
            var paramArgFunc = argFuncs.Select((func, i) => func(0, activeParam)); // technically invalid for clipboard
            if (argc != argFuncs.Length)
                return new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operation}");
            int rowEditCount = -1;
            foreach ((MassEditRowSource source, Param.Row row) in ParamAndRowSearchEngine.parse.Search(context, paramSelector, false, false))
            {
                rowEditCount++;
                var rowArgFunc = paramArgFunc.Select((rowFunc, i) => rowFunc(rowEditCount, row)).ToArray();
                string paramname = source == MassEditRowSource.Selection ? context.getActiveParam() : ParamBank.ClipboardParam;
                if (cellSelector == null)
                {
                    var res = ExecRowOp(rowArgFunc, rowFunc, paramname, row, operation, partialActions);
                    if (res.Type != MassEditResultType.SUCCESS)
                        return res;
                }
                else
                {
                    var res = ExecCellStage(rowArgFunc, cellFunc, paramname, row, cellSelector, operation, partialActions);
                    if (res.Type != MassEditResultType.SUCCESS)
                        return res;
                }
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private static MassEditResult ExecParamStage(Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>[] argFuncs, Func<(string, Param.Row), string[], (Param, Param.Row)> rowFunc, Func<object, string[], object> cellFunc, int argc, string paramSelector, string rowSelector, string cellSelector, string operation, List<EditorAction> partialActions)
        {
            int paramEditCount = -1;
            foreach ((ParamBank b, Param p) in ParamSearchEngine.pse.Search(false, paramSelector, false, false))
            {
                paramEditCount++;
                var paramArgFunc = argFuncs.Select((func, i) => func(paramEditCount, p));
                if (argc != argFuncs.Length)
                    return new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operation}");
                string paramname = b.GetKeyForParam(p);
                var res = ExecRowStage(paramArgFunc, rowFunc, cellFunc, paramname, b, p, rowSelector, cellSelector, operation, partialActions);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private static MassEditResult ExecRowStage(IEnumerable<Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>> paramArgFunc, Func<(string, Param.Row), string[], (Param, Param.Row)> rowFunc, Func<object, string[], object> cellFunc, string paramname, ParamBank b, Param p, string rowSelector, string cellSelector, string operation, List<EditorAction> partialActions)
        {
            int rowEditCount = -1;
            foreach (Param.Row row in RowSearchEngine.rse.Search((b, p), rowSelector, false, false))
            {
                rowEditCount++;
                var rowArgFunc = paramArgFunc.Select((rowFunc, i) => rowFunc(rowEditCount, row)).ToArray();
                if (cellSelector == null)
                {
                    var res = ExecRowOp(rowArgFunc, rowFunc, paramname, row, operation, partialActions);
                    if (res.Type != MassEditResultType.SUCCESS)
                        return res;
                }
                else
                {
                    var res = ExecCellStage(rowArgFunc, cellFunc, paramname, row, cellSelector, operation, partialActions);
                    if (res.Type != MassEditResultType.SUCCESS)
                        return res;
                }
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private static MassEditResult ExecRowOp(Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc, Func<(string, Param.Row), string[], (Param, Param.Row)> rowFunc, string paramname, Param.Row row, string operation, List<EditorAction> partialActions)
        {
            var rowArgValues = rowArgFunc.Select((argV, i) => argV(-1, (PseudoColumn.None, null))).ToArray();
            var (p2, rs) = rowFunc((paramname, row), rowArgValues);
            if (p2 == null)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', rowArgValues)} on row");
            if (rs != null)
                partialActions.Add(new AddParamsAction(p2, "FromMassEdit", new List<Param.Row>{rs}, false, true));
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private static MassEditResult ExecCellStage(Func<int, (PseudoColumn, Param.Column), string>[] rowArgFunc, Func<object, string[], object> cellFunc, string paramname, Param.Row row, string cellSelector, string operation, List<EditorAction> partialActions)
        {
            int cellEditCount = -1;
            foreach ((PseudoColumn, Param.Column) col in CellSearchEngine.cse.Search((paramname, row), cellSelector, false, false))
            {
                cellEditCount++;
                var cellArgValues = rowArgFunc.Select((argV, i) => argV(cellEditCount, col)).ToArray();
                var res = ExecCellOp(cellFunc, cellArgValues, paramname, row, col, operation, partialActions);
                if (res.Type != MassEditResultType.SUCCESS)
                    return res;
            }
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
        private static MassEditResult ExecCellOp(Func<object, string[], object> cellFunc, string[] cellArgValues, string paramname, Param.Row row, (PseudoColumn, Param.Column) col, string operation, List<EditorAction> partialActions)
        {
            var res = cellFunc(row.Get(col), cellArgValues);
            if (res == null && col.Item1 == PseudoColumn.ID)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on ID");
            else if (res == null && col.Item1 == PseudoColumn.Name)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on Name");
            else if (res == null)
                return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on field {col.Item2.Def.InternalName}");
            addAction(row, col, res, partialActions);
            return new MassEditResult(MassEditResultType.SUCCESS, "");
        }
    }
    public class MassParamEditCSV : MassParamEdit
    {
        public static string GenerateColumnLabels(Param param, char separator)
        {
            string str = "";
            str += $@"ID{separator}Name{separator}";
            foreach (var f in param.AppliedParamdef.Fields)
            {
                 str += $@"{f.InternalName}{separator}";
            }
            return str+"\n";
        }
        
        public static string GenerateCSV(List<Param.Row> rows, Param param, char separator)
        {
            string gen = "";
            gen += GenerateColumnLabels(param, separator);

            foreach (Param.Row row in rows)
            {
                string name = row.Name==null ? "null" : row.Name.Replace(separator, '-');
                string rowgen = $@"{row.ID}{separator}{name}";
                foreach (Param.Column cell in row.Cells)
                    rowgen += $@"{separator}{row[cell].ToParamEditorString()}";
                gen += rowgen + "\n";
            }
            return gen;
        }
        public static string GenerateSingleCSV(List<Param.Row> rows, Param param, string field, char separator)
        {
            string gen = $@"ID{separator}{field}"+"\n";
            foreach (Param.Row row in rows)
            {
                string rowgen;
                if (field.Equals("Name"))
                {
                    string name = row.Name==null ? "null" : row.Name.Replace(separator, '-');
                    rowgen = $@"{row.ID}{separator}{name}";
                }
                else
                {
                    rowgen = $@"{row.ID}{separator}{row[field].Value.ToParamEditorString()}";
                }
                gen += rowgen + "\n";
            }
            return gen;
        }
        
        public static MassEditResult PerformMassEdit(ParamBank bank, string csvString, ActionManager actionManager, string param, bool appendOnly, bool replaceParams, char separator)
        {
            #if !DEBUG
            try
            {
            #endif
                Param p = bank.Params[param];
                if (p == null)
                    return new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected");
                int csvLength = p.AppliedParamdef.Fields.Count + 2;// Include ID and name
                string[] csvLines = csvString.Split("\n");
                if (csvLines[0].Contains($@"ID{separator}Name"))
                    csvLines[0] = ""; //skip column label row
                int changeCount = 0;
                int addedCount = 0;
                List<EditorAction> actions = new List<EditorAction>();
                List<Param.Row> addedParams = new List<Param.Row>();
                foreach (string csvLine in csvLines)
                {
                    if (csvLine.Trim().Equals(""))
                        continue;
                    string[] csvs = csvLine.Trim().Split(separator);
                    if (csvs.Length != csvLength && !(csvs.Length==csvLength+1 && csvs[csvLength].Trim().Equals("")))
                    {
                        return new MassEditResult(MassEditResultType.PARSEERROR, "CSV has wrong number of values");
                    }
                    int id = int.Parse(csvs[0]);
                    string name = csvs[1];
                    var row = p[id];
                    if (row == null || replaceParams)
                    {
                        row = new Param.Row(id, name, p);
                        addedParams.Add(row);
                    }
                    if (!name.Equals(row.Name))
                        actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, name));
                    int index = 2;
                    foreach (Param.Column col in row.Cells)
                    {
                        string v = csvs[index];
                        index++;
                        object newval = WithDynamicOf(row.Get((PseudoColumn.None, col)), (v) => v);
                        if (newval == null)
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {v} to field {col.Def.InternalName}");
                        addAction(row, (PseudoColumn.None, col), newval, actions);
                    }
                }
                changeCount = actions.Count;
                addedCount = addedParams.Count;
                actions.Add(new AddParamsAction(p, "legacystring", addedParams, appendOnly, replaceParams));
                if (changeCount != 0 || addedCount != 0)
                    actionManager.ExecuteAction(new CompoundAction(actions));
                return new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected, {addedCount} rows added");
            #if !DEBUG
            }
            catch
            {
                return new MassEditResult(MassEditResultType.PARSEERROR, "Unable to parse CSV into correct data types");
            }
            #else
                return new MassEditResult(MassEditResultType.PARSEERROR, "Unable to parse CSV into correct data types");
            #endif
        }
        public static (MassEditResult, CompoundAction) PerformSingleMassEdit(ParamBank bank, string csvString, string param, string field, char separator, bool ignoreMissingRows)
        {
            try
            {
                Param p = bank.Params[param];
                if (p == null)
                    return (new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected"), null);
                string[] csvLines = csvString.Split("\n");
                if (csvLines[0].Contains($@"ID{separator}"))
                {
                    if (!csvLines[0].Contains($@"ID{separator}{field}"))
                    {
                        return (new MassEditResult(MassEditResultType.PARSEERROR, "CSV has wrong field name"), null);
                    }
                    csvLines[0] = ""; //skip column label row
                }
                int changeCount = 0;
                List<EditorAction> actions = new List<EditorAction>();
                foreach (string csvLine in csvLines)
                {
                    if (csvLine.Trim().Equals(""))
                        continue;
                    string[] csvs = csvLine.Trim().Split(separator, 2);
                    if (csvs.Length != 2 && !(csvs.Length==3 && csvs[2].Trim().Equals("")))
                        return (new MassEditResult(MassEditResultType.PARSEERROR, "CSV has wrong number of values"), null);
                    int id = int.Parse(csvs[0]);
                    string value = csvs[1];
                    Param.Row? row = p[id];
                    if (row == null)
                    {
                        if (ignoreMissingRows)
                            continue;
                        return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate row {id}"), null);
                    }
                    if (field.Equals("Name"))
                    {
                        if (!value.Equals(row.Name))
                            actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, value));
                    }
                    else
                    {
                        Param.Column? col = p[field];
                        if (col == null)
                        {
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate field {field}"), null);
                        }
                        object newval = WithDynamicOf(row.Get((PseudoColumn.None, col)), (v) => v);
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {value} to field {col.Def.InternalName}"), null);
                        addAction(row, (PseudoColumn.None, col), newval, actions);
                    }
                }
                changeCount = actions.Count;
                return (new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} rows affected"), new CompoundAction(actions));
            }
            catch
            {
                return (new MassEditResult(MassEditResultType.PARSEERROR, "Unable to parse CSV into correct data types"), null);
            }
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
        internal Dictionary<string, (string[], Func<T, string[], O>)> operations = new Dictionary<string, (string[], Func<T, string[], O>)>();
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
        public List<(string, string[])> AvailableCommands()
        {
            List<(string, string[])> options = new List<(string, string[])>();
            foreach (string op in operations.Keys)
            {
                options.Add((op, operations[op].Item1));
            }
            return options;
        }

    }
    public class MEGlobalOperation : MEOperation<ParamEditorSelectionState, bool>
    {
        public static MEGlobalOperation globalOps = new MEGlobalOperation();
        internal override void Setup()
        {
            operations.Add("clear", (new string[0], (selectionState, args) => {
                ParamBank.ClipboardParam = null;
                ParamBank.ClipboardRows.Clear();
                return true;
            }));
            operations.Add("set", (new string[]{"variable", "value"}, (selectionState, args) => {
                MassParamEdit.massEditVars[args[0]] = args[1];
                return true;
            }));
        }
    }
    public class MERowOperation : MEOperation<(string, Param.Row), (Param, Param.Row)>
    {
        public static MERowOperation rowOps = new MERowOperation();
        internal override void Setup()
        {
            operations.Add("copy", (new string[0], (paramAndRow, args) => {
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
            operations.Add("copyN", (new string[]{"count"}, (paramAndRow, args) => {
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
            operations.Add("paste", (new string[0], (paramAndRow, args) => {
                string paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                    throw new Exception($@"Could not locate param");
                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                    throw new Exception($@"Could not locate param {paramKey}");
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                return (p, new Param.Row(row, p));
            }));  
            operations.Add("migrate", (new string[]{"parambank name (only primary supported)"}, (paramAndRow, target) => {
                if (!target[0].Trim().ToLower().Equals("primary"))
                    throw new Exception($@"Only migrating to primary is supported");
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
            operations.Add("=", (new string[]{"number or text"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => args[0])));
            operations.Add("+", (new string[]{"number or text"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => {
                double val;
                if (double.TryParse(args[0], out val))
                    return v + val;
                return v + args[0];
                })));
            operations.Add("-", (new string[]{"number"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v - double.Parse(args[0]))));
            operations.Add("*", (new string[]{"number"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v * double.Parse(args[0]))));
            operations.Add("/", (new string[]{"number"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v / double.Parse(args[0]))));
            operations.Add("%", (new string[]{"number"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v % double.Parse(args[0]))));
            operations.Add("scale", (new string[]{"factor number", "center number"}, (ctx, args) =>  MassParamEdit.WithDynamicOf(ctx, (v) => {
                double opp2 = double.Parse(args[1]);
                return (v - opp2) * double.Parse(args[0]) + opp2;
            })));
            operations.Add("replace", (new string[]{"text to replace", "new text"}, (ctx, args) => MassParamEdit.WithDynamicOf(ctx, (v) => v.Replace(args[0], args[1]))));
        }
    }
    public class MEOperationArgument
    {
        public static MEOperationArgument arg = new MEOperationArgument();
        Dictionary<string, (string[], Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>>)> argumentGetters = new Dictionary<string, (string[], Func<string[], Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>>)>();
        (string[], Func<string, Func<int, Param, Func<int, Param.Row, Func<int, (PseudoColumn, Param.Column), string>>>>) defaultGetter;

        private MEOperationArgument()
        {
            Setup();
        }
        private void Setup()
        {
            defaultGetter = (new string[0], (value) => (i, param) => (j, row) => (k, col) => value);
            argumentGetters.Add("self", (new string[0], (empty) => (i, param) => (j, row) => (k, col) => {
                return row.Get(col).ToParamEditorString();
            }));
            argumentGetters.Add("field", (new string[]{"field internalName"}, (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                return (j, row) => {
                    string v = row.Get(col).ToParamEditorString();
                    return (k, c) => v;
                };
            }));
            argumentGetters.Add("vanilla", (new string[0], (empty) => {
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
            argumentGetters.Add("aux", (new string[]{"parambank name"}, (bankName) => {
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
            }));
            argumentGetters.Add("vanillafield", (new string[]{"field internalName"}, (field) => (i, param) => {
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
            argumentGetters.Add("auxfield", (new string[]{"parambank name", "field internalName"}, (bankAndField) => {
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
            }));
            argumentGetters.Add("average", (new string[]{"field internalName", "row selector"}, (field) => (i, param) => {
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
            }));
            argumentGetters.Add("median", (new string[]{"field internalName", "row selector"}, (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var vals = rows.Select((row, i) => row.Get(col));
                var avg = vals.OrderBy((val) => Convert.ToDouble(val)).ElementAt(vals.Count()/2);
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }));
            argumentGetters.Add("mode", (new string[]{"field internalName", "row selector"}, (field) => (i, param) => {
                var col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                    throw new Exception($@"Could not locate field {field[0]}");
                var rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var vals = rows.Select((row, i) => row.Get(col));
                var avg = vals.GroupBy((val) => val).OrderByDescending((g) => g.Count()).Select((g) => g.Key).First();
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }));
            argumentGetters.Add("random", (new string[]{"minimum number (inclusive)", "maximum number (exclusive)"}, (minAndMax) => {
                double min;
                double max;
                if (!double.TryParse(minAndMax[0], out min) || !double.TryParse(minAndMax[1], out max))
                    throw new Exception($@"Could not parse min and max random values");
                if (max <= min)
                    throw new Exception($@"Random max must be greater than min");
                double range = max - min;
                return (i, param) => (j, row) => (k, c) => (Random.Shared.NextDouble() * range + min).ToString();
            }));
            argumentGetters.Add("randint", (new string[]{"minimum integer (inclusive)", "maximum integer (inclusive)"}, (minAndMax) => {
                int min;
                int max;
                if (!int.TryParse(minAndMax[0], out min) || !int.TryParse(minAndMax[1], out max))
                    throw new Exception($@"Could not parse min and max randint values");
                if (max <= min)
                    throw new Exception($@"Random max must be greater than min");
                return (i, param) => (j, row) => (k, c) => Random.Shared.NextInt64(min, max + 1).ToString();
            }));
            argumentGetters.Add("randFrom", (new string[]{"param name", "field internalName", "row selector"}, (paramFieldRowSelector) => {
                Param srcParam = ParamBank.PrimaryBank.Params[paramFieldRowSelector[0]];
                List<Param.Row> srcRows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, srcParam), paramFieldRowSelector[2], false, false);
                object[] values = srcRows.Select((r, i) => r[paramFieldRowSelector[1]].Value.Value).ToArray();
                return (i, param) => (j, row) => (k, c) => values[Random.Shared.NextInt64(values.Length)].ToString();
            }));
            argumentGetters.Add("paramIndex", (new string[0], (empty) => (i, param) => (j, row) => (k, col) => {
                return i.ToParamEditorString();
            }));
            argumentGetters.Add("rowIndex", (new string[0], (empty) => (i, param) => (j, row) => (k, col) => {
                return j.ToParamEditorString();
            }));
            argumentGetters.Add("fieldIndex", (new string[0], (empty) => (i, param) => (j, row) => (k, col) => {
                return k.ToParamEditorString();
            }));
            /*argumentGetters.Add("variable", (new string[]{"variableName"}, (name) => {
                string val = MassParamEdit.massEditVars[name[0]];
                return (i, param) => (j, row) => (k, cell) => val;
            }));*/
        }

        public List<(string, string[])> AvailableArguments()
        {
            List<(string, string[])> options = new List<(string, string[])>();
            foreach (string op in argumentGetters.Keys)
            {
                options.Add((op, argumentGetters[op].Item1));
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
                opArg = MassParamEdit.massEditVars[opArg.Substring(1)];
            string[] arg = opArg.Split(" ", 2);
            if (argumentGetters.ContainsKey(arg[0].Trim()))
            {
                var getter = argumentGetters[arg[0]];
                string[] opArgArgs = arg.Length > 1 ? arg[1].Split(" ", getter.Item1.Length) : new string[0];
                if (opArgArgs.Length != getter.Item1.Length)
                    throw new Exception(@$"Contextual value {arg[0]} has wrong number of arguments. Expected {opArgArgs.Length}");
                return getter.Item2(opArgArgs);
            }
            else
            {
                return defaultGetter.Item2(opArg);
            }
        }
    }
}