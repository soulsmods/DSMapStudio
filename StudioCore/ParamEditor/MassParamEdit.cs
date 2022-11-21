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
    
    public class MassParamEdit
    {
        internal static object PerformOperation(ParamBank bank, Param.Row row, Param.Column column, string op, string opparam)
        {
            try
            {
                if (op.Equals("ref"))
                {
                    if (column.ValueType == typeof(int))
                    {
                        foreach (string reftype in FieldMetaData.Get(column.Def).RefTypes)
                        {
                            var p = bank.Params[reftype];
                            if (p == null)
                                continue;
                            foreach (var r in p.Rows)
                            {
                                if (r.Name == null)
                                    continue;
                                if (r.Name.Equals(opparam))
                                    return r.ID;
                            }
                        }
                    }
                }
                if (op.Equals("="))
                {
                    if (column.ValueType == typeof(bool))
                        return bool.Parse(opparam);
                    else if (column.ValueType == typeof(string))
                        return opparam;
                    else if (column.ValueType == typeof(byte[]))
                        return ParamUtils.Dummy8Read(opparam, ((byte[])column.GetValue(row)).Length);
                }
                
                if (column.ValueType == typeof(long))
                    return PerformBasicOperation<long>(row, column, op, double.Parse(opparam));
                if (column.ValueType == typeof(ulong))
                    return PerformBasicOperation<ulong>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(int))
                    return PerformBasicOperation<int>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(uint))
                    return PerformBasicOperation<uint>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(short))
                    return PerformBasicOperation<short>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(ushort))
                    return PerformBasicOperation<ushort>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(sbyte))
                    return PerformBasicOperation<sbyte>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(byte))
                    return PerformBasicOperation<byte>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(float))
                    return PerformBasicOperation<float>(row, column, op, double.Parse(opparam));
                else if (column.ValueType == typeof(double))
                    return PerformBasicOperation<double>(row,column, op, double.Parse(opparam));
            }
            catch
            {
            }
            return null;
        }
        internal static string PerformNameOperation(string name, string op, string opparam)
        {
            try
            {
                if (op.Equals("="))
                {
                    return opparam;
                }
                if (op.Equals("+"))
                {
                    return name + opparam;
                }
                if (op.Equals("replace"))
                {
                    string[] split = opparam.Split(":");
                    if (split.Length!=2)
                        return null;
                    return name.Replace(split[0], split[1]);
                }
            }
            catch
            {
            }
            return null;
        }

        public static T PerformBasicOperation<T>(Param.Row row, Param.Column c, string op, double opparam) where T : struct, IFormattable
        {
            try
            {
                dynamic val = c.GetValue(row);
                dynamic opp = opparam;
                if (op.Equals("="))
                    return (T) (opp);
                else if (op.Equals("+"))
                    return (T) (val + opp);
                else if (op.Equals("-"))
                    return (T) (val - opp);
                else if (op.Equals("*"))
                    return (T) (val * opp);
                else if (op.Equals("/"))
                    return (T) (val / opp);
            }
            catch
            {
                // Operation error
            }
            return default(T);
        }

        internal static void addAction(Param.Cell handle, object newval, List<EditorAction> actions)
        {
            if (!(handle.Value.Equals(newval) 
            || (handle.Value.GetType()==typeof(byte[]) 
            && ParamUtils.ByteArrayEquals((byte[])handle.Value, (byte[])newval))))
                actions.Add(new PropertiesChangedAction(handle.GetType().GetProperty("Value"), -1, handle, newval));
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

                    (var result, var actions) = PerformMassEditCommandParamStep(bank, command, context);
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
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandParamStep(ParamBank bank, string restOfStages, ParamEditorSelectionState context)
        {
            string[] paramstage = restOfStages.Split(":", 2);
            string paramSelector = paramstage[0].Trim();
            if (paramSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find param filter. Add : and one of "+String.Join(", ", ParamSearchEngine.pse.AvailableCommands())+" or "+String.Join(", ", ParamAndRowSearchEngine.parse.AvailableCommands())), null);
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
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter. Add : and one of "+String.Join(", ", RowSearchEngine.rse.AvailableCommands())), null);
            return PerformMassEditCommandCellStep(bank, false, paramStage, rowSelector, rowstage[1], context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandCellStep(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string restOfStages, ParamEditorSelectionState context)
        {
            string[] cellstage = restOfStages.Split(":", 2);
            string cellSelector = cellstage[0].Trim();
            if (cellSelector.Equals(""))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell/property filter. Add : and one of "+String.Join(", ", CellSearchEngine.cse.AvailableCommands())+" or Name (0 args)"), null);
            
            if (MERowOperation.rowOps.HandlesCommand(cellSelector.Split(" ", 2)[0]))
            {
                return PerformMassEditCommandRowOpStep(bank, isParamRowSelector, paramSelector, rowSelector, restOfStages, context);
            }
            else
            {
                PseudoColumn pseudoCol = cellSelector.Equals("Name") ? PseudoColumn.Name : PseudoColumn.None;
                return PerformMassEditCommandCellOpStep(bank, isParamRowSelector, paramSelector, rowSelector, cellSelector, pseudoCol, cellstage[1], context);
            }
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandRowOpStep(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string restOfStages, ParamEditorSelectionState context)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            string operation = operationstage[0].Trim();
            if (operationstage.Length == 1)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation arguments."), null);
            return PerformMassEditCommand(bank, isParamRowSelector, paramSelector, rowSelector, null, PseudoColumn.None, operation, operationstage[1], context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommandCellOpStep(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string cellSelector, PseudoColumn pseudoCol, string restOfStages, ParamEditorSelectionState context)
        {
            string[] operationstage =  restOfStages.TrimStart().Split(" ", 2);                
            string operation = operationstage[0].Trim();

            if (operation.Equals("") || (pseudoCol == PseudoColumn.Name && !MEPseudoCellOperation.pseudoCellOps.operations.ContainsKey(operation)) || (pseudoCol != PseudoColumn.Name && !MECellOperation.cellOps.operations.ContainsKey(operation)))
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
            if (operationstage.Length == 1)
                return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation arguments. Add a value, or 'field' followed by the name of a field to take the value from"), null);
            return PerformMassEditCommand(bank, isParamRowSelector, paramSelector, rowSelector, cellSelector, pseudoCol, operation, operationstage[1], context);
        }
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommand(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string cellSelector, PseudoColumn pseudoCol, string operation, string opargs, ParamEditorSelectionState context)
        {
            List<EditorAction> partialActions = new List<EditorAction>();
            try {

                int argc;
                Func<(ParamBank, Param, Param.Row), string[], (Param, Param.Row[])> rowFunc = null;
                Func<Param.Row, string[], object> pseudoCellFunc = null;
                Func<(Param.Row, Param.Column), string[], object> cellFunc = null; 
                if (cellSelector == null)
                {
                    if (!MERowOperation.rowOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation "+operation), null);
                    (argc, rowFunc) = MERowOperation.rowOps.operations[operation];
                }
                else if (pseudoCol == PseudoColumn.Name)
                {
                    if (!MEPseudoCellOperation.pseudoCellOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation "+operation), null);
                    (argc, pseudoCellFunc) = MEPseudoCellOperation.pseudoCellOps.operations[operation];
                }
                else
                {
                    if (!MECellOperation.cellOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation "+operation), null);
                    (argc, cellFunc) = MECellOperation.cellOps.operations[operation];
                }
                string[] args = opargs.Split(":", argc);
                var argFuncs = MEOperationArgument.arg.getContextualArguments(argc, opargs);
                if (isParamRowSelector)
                {
                    Param activeParam = bank.Params[context.getActiveParam()];
                    var paramArgFunc = argFuncs.Select((func, i) => func(activeParam));
                    if (argc != argFuncs.Length)
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operation}"), null);
                    foreach (Param.Row row in ParamAndRowSearchEngine.parse.Search(context, paramSelector, false, false))
                    {
                        var rowArgFunc = paramArgFunc.Select((rowFunc, i) => rowFunc(row)).ToArray();
                        if (cellSelector == null)
                        {
                            var rowArgValues = rowArgFunc.Select((argV, i) => argV(PseudoColumn.None, null)).ToArray();
                            var (p, rs) = rowFunc((bank, activeParam, row), rowArgValues);
                            if (p == null || rs == null)
                                return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', rowArgValues)} on row"), null);
                            partialActions.Add(new AddParamsAction(p, "FromMassEdit", rs.ToList(), false, true, false));
                        }
                        else if (pseudoCol == PseudoColumn.Name)
                        {
                            var cellArgValues = rowArgFunc.Select((argV, i) => argV(PseudoColumn.Name, null)).ToArray();
                            var res = pseudoCellFunc(row, cellArgValues);
                            if (res == null)
                                return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on Name"), null);
                            partialActions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, res));
                        }
                        else
                        {
                            foreach (Param.Column col in CellSearchEngine.cse.Search(row, cellSelector, false, false))
                            {
                                var cellArgValues = rowArgFunc.Select((argV, i) => argV(PseudoColumn.None, col)).ToArray();
                                var res = cellFunc((row, col), cellArgValues);
                                if (res == null)
                                    return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on field {col.Def.InternalName}"), null);
                                addAction(row[col], res, partialActions);
                            }
                        }
                    }
                }
                else
                {
                    foreach ((ParamBank b, Param p) in ParamSearchEngine.pse.Search(false, paramSelector, false, false))
                    {
                        var paramArgFunc = argFuncs.Select((func, i) => func(p));
                        if (argc != argFuncs.Length)
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operation}"), null);
                        foreach (Param.Row row in RowSearchEngine.rse.Search((b, p), rowSelector, false, false))
                        {
                            var rowArgFunc = paramArgFunc.Select((rowFunc, i) => rowFunc(row)).ToArray();
                            if (cellSelector == null)
                            {
                                var rowArgValues = rowArgFunc.Select((argV, i) => argV(PseudoColumn.None, null)).ToArray();
                                var (p2, rs) = rowFunc((b, p, row), rowArgValues);
                                if (p2 == null || rs == null)
                                    return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', rowArgValues)} on row"), null);
                                partialActions.Add(new AddParamsAction(p2, "FromMassEdit", rs.ToList(), false, true, false));
                                }
                            else if (pseudoCol == PseudoColumn.Name)
                            {
                                var cellArgValues = rowArgFunc.Select((argV, i) => argV(PseudoColumn.Name, null)).ToArray();
                                var res = pseudoCellFunc(row, cellArgValues);
                                if (res == null)
                                    return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on Name"), null);
                                partialActions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, res));
                            }
                            else
                            {
                                foreach (Param.Column col in CellSearchEngine.cse.Search(row, cellSelector, false, false))
                                {
                                    var cellArgValues = rowArgFunc.Select((argV, i) => argV(PseudoColumn.None, col)).ToArray();
                                    var res = cellFunc((row, col), cellArgValues);
                                    if (res == null)
                                        return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', cellArgValues)} on field {col.Def.InternalName}"), null);
                                    addAction(row[col], res, partialActions);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                return (new MassEditResult(MassEditResultType.OPERATIONERROR, e.ToString()), null);
            }
            return (new MassEditResult(MassEditResultType.SUCCESS, $@"{partialActions.Count} cells affected"), partialActions);
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
                {
                    if (row[cell].Value.GetType() == typeof(byte[]))
                        rowgen += $@"{separator}{ParamUtils.Dummy8Write((byte[])row[cell].Value)}";
                    else
                        rowgen += $@"{separator}{row[cell].Value}";
                }
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
                    rowgen = $@"{row.ID}{separator}{row[field].Value.Value}";
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
                        object newval = PerformOperation(bank, row, col, "=", v);
                        if (newval == null)
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {v} to field {col.Def.InternalName}");
                        var handle = row[col];
                        addAction(handle, newval, actions);
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
                        object newval = PerformOperation(bank, row, col, "=", value);
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {value} to field {col.Def.InternalName}"), null);
                        var handle = row[col];
                        addAction(handle, newval, actions);
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

    internal class MEOperation<T, O>
    {
        internal Dictionary<string, (int, Func<T, string[], O>)> operations = new Dictionary<string, (int, Func<T, string[], O>)>();
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

    }
    internal class MERowOperation : MEOperation<(ParamBank, Param, Param.Row), (Param, Param.Row[])>
    {
        internal static MERowOperation rowOps = new MERowOperation();
        internal override void Setup()
        {
            operations.Add("migrate", (1, (paramBankAndRow, target) => {
                if (!target[0].Trim().ToLower().Equals("primary"))
                    throw new Exception($@"Only migrating to primary is supported");
                ParamBank bank = paramBankAndRow.Item1;
                Param param = paramBankAndRow.Item2;
                Param.Row row = paramBankAndRow.Item3;
                string paramKey = bank.GetKeyForParam(param);
                if (paramKey == null)
                    throw new Exception($@"Could not locate param");
                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                    throw new Exception($@"Could not locate param {paramKey}");
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                return (p, new Param.Row[]{new Param.Row(row, p)});
            }));            
        }
    }
    internal class MECellOperation : MEOperation<(Param.Row, Param.Column), object>
    {
        internal static MECellOperation cellOps = new MECellOperation();
        internal override void Setup()
        {
            operations.Add("=", (1, (ctx, args) => MassParamEdit.PerformOperation(ParamBank.PrimaryBank, ctx.Item1, ctx.Item2, "=", args[0])));
            operations.Add("+", (1, (ctx, args) => MassParamEdit.PerformOperation(ParamBank.PrimaryBank, ctx.Item1, ctx.Item2, "+", args[0])));
            operations.Add("-", (1, (ctx, args) => MassParamEdit.PerformOperation(ParamBank.PrimaryBank, ctx.Item1, ctx.Item2, "-", args[0])));
            operations.Add("*", (1, (ctx, args) => MassParamEdit.PerformOperation(ParamBank.PrimaryBank, ctx.Item1, ctx.Item2, "*", args[0])));
            operations.Add("/", (1, (ctx, args) => MassParamEdit.PerformOperation(ParamBank.PrimaryBank, ctx.Item1, ctx.Item2, "/", args[0])));
            operations.Add("%", (1, (ctx, args) => MassParamEdit.PerformOperation(ParamBank.PrimaryBank, ctx.Item1, ctx.Item2, "%", args[0])));
        }
    }
    internal class MEPseudoCellOperation : MEOperation<Param.Row, object>
    {
        internal static MEPseudoCellOperation pseudoCellOps = new MEPseudoCellOperation();
        internal override void Setup()
        {
            operations.Add("=", (1, (row, args) => MassParamEdit.PerformNameOperation(row.Name, "=", args[0])));
            operations.Add("+", (1, (row, args) => MassParamEdit.PerformNameOperation(row.Name, "+", args[0])));
            operations.Add("replace", (2, (row, args) => MassParamEdit.PerformNameOperation(row.Name, "replace", args[0]+":"+args[1])));
        }
    }
    internal class MEOperationArgument
    {
        static internal MEOperationArgument arg = new MEOperationArgument();
        Dictionary<string, (int, Func<string[], Func<Param, Func<Param.Row, Func<PseudoColumn, Param.Column, string>>>>)> argumentGetters = new Dictionary<string, (int, Func<string[], Func<Param, Func<Param.Row, Func<PseudoColumn, Param.Column, string>>>>)>();
        (int, Func<string, Func<Param, Func<Param.Row, Func<PseudoColumn, Param.Column, string>>>>) defaultGetter;

        private MEOperationArgument()
        {
            Setup();
        }
        private void Setup()
        {
            defaultGetter = (0, (value) => (param) => (row) => (pc, col) => value);
            argumentGetters.Add("self", (0, (empty) => (param) => (row) => (pc, col) => {
                if (col != null)
                {
                    object val = row[col].Value;
                    return val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
                }
                else
                {
                    return row.Name;
                }
            }));
            argumentGetters.Add("field", (1, (field) => (param) => {
                Param.Column? col = param[field[0]];
                if (col == null)
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }
                return (row) => {
                    object val = row[col].Value;
                    string v = val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
                    return (pc,c) => v;
                };
            }));
            argumentGetters.Add("vanilla", (0, (empty) => {
                ParamBank bank = ParamBank.VanillaBank;
                return (param) => {
                    string paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                        throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                    Param vParam = bank.Params[paramName];
                    return (row) => {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                            throw new Exception($@"Could not locate vanilla row {row.ID}");
                        return (pc, col) => {
                            if (col != null)
                            {
                                object val = vRow[col].Value;
                                return val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
                            }
                            else
                            {
                                return vRow.Name;
                            }
                        };
                    };
                };
            }));
            argumentGetters.Add("vanillafield", (1, (field) => (param) => {
                var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                var vParam = ParamBank.VanillaBank.GetParamFromName(paramName);
                if (vParam == null)
                    throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                Param.Column? col = vParam?[field[0]];
                if (col == null)
                    throw new Exception($@"Could not locate field {field[0]}");
                return (row) => {
                    Param.Row vRow = vParam?[row.ID];
                    if (vRow == null)
                        throw new Exception($@"Could not locate vanilla row {row.ID}");
                    object val = vRow[col].Value;
                    string v = val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
                    return (pc,c) => v;
                };
            }));
            argumentGetters.Add("aux", (1, (bankName) => {
                if (!ParamBank.AuxBanks.ContainsKey(bankName[0]))
                    throw new Exception($@"Could not locate paramBank {bankName[0]}");
                ParamBank bank = ParamBank.AuxBanks[bankName[0]];
                return (param) => {
                    string paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    Param vParam = bank.Params[paramName];
                    return (row) => {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        return (pc, col) => {
                            if (col != null)
                            {
                                object val = vRow[col].Value;
                                return val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
                            }
                            else
                            {
                                return vRow.Name;
                            }
                        };
                    };
                };
            }));
            argumentGetters.Add("auxfield", (2, (bankAndField) => {
                if (!ParamBank.AuxBanks.ContainsKey(bankAndField[0]))
                    throw new Exception($@"Could not locate paramBank {bankAndField[0]}");
                ParamBank bank = ParamBank.AuxBanks[bankAndField[0]];
                return (param) => {
                    string paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    Param vParam = bank.Params[paramName];
                    Param.Column? col = vParam?[bankAndField[1]];
                    if (col == null)
                        throw new Exception($@"Could not locate field {bankAndField[1]}");
                    return (row) => {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        object val = vRow[col].Value;
                        string v = val.GetType() == typeof(byte[]) ? ParamUtils.Dummy8Write((byte[])val) : val.ToString();
                        return (pc, col) => v;
                    };
                };
            }));
        }

        internal Func<Param, Func<Param.Row, Func<PseudoColumn, Param.Column, string>>>[] getContextualArguments(int argumentCount, string opData)
        {
            string[] opArgs = opData.Split(':', argumentCount);
            Func<Param, Func<Param.Row, Func<PseudoColumn, Param.Column, string>>>[] contextualArgs = new Func<Param, Func<Param.Row, Func<PseudoColumn, Param.Column, string>>>[opArgs.Length];
            for (int i=0; i<opArgs.Length; i++)
            {
                string[] arg = opArgs[i].Split(" ", 2);
                if (argumentGetters.ContainsKey(arg[0].Trim()))
                {
                    var getter = argumentGetters[arg[0]];
                    string[] opArgArgs = arg[1].Split(" ", getter.Item1);
                    if (opArgArgs.Length != getter.Item1)
                        throw new Exception(@$"Contextual value {arg[0]} has wrong number of arguments. Expected {opArgArgs.Length}");
                    contextualArgs[i] = getter.Item2(opArgArgs);
                }
                else
                {
                    contextualArgs[i] = defaultGetter.Item2(opArgs[i]);
                }
            }
            return contextualArgs;
        }
    }

    internal enum PseudoColumn
    {
        None,
        // ID,
        Name
    }
}