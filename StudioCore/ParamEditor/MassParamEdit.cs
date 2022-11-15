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

                    string[] paramstage = command.Split(":", 2);
                    string paramSelector = paramstage[0].Trim();
                    if (paramSelector.Equals(""))
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find param filter. Add : and one of "+String.Join(", ", ParamSearchEngine.pse.AvailableCommands())+" or "+String.Join(", ", ParamAndRowSearchEngine.parse.AvailableCommands())), null);
                    
                    string[] rowstage = null;
                    string rowSelector = null;
                    string[] cellstage = null;
                    string cellSelector = null;
                    if (ParamAndRowSearchEngine.parse.HandlesCommand(paramSelector))
                    {
                        cellstage = paramstage[1].Split(":", 2);
                    }
                    else
                    {
                        rowstage = paramstage[1].Split(":", 2);
                        rowSelector = rowstage[0].Trim();
                        if (rowSelector.Equals(""))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter. Add : and one of "+String.Join(", ", RowSearchEngine.rse.AvailableCommands())), null);
                        cellstage = rowstage[1].Split(":", 2);
                    }
                    cellSelector = cellstage[0].Trim();
                    if (cellSelector.Equals(""))
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell/property filter. Add : and one of "+String.Join(", ", CellSearchEngine.cse.AvailableCommands())+" or Name (0 args)"), null);
                    bool editName = cellSelector.Equals("Name");

                    string[] operationstage =  cellstage[1].TrimStart().Split(" ", 2);                
                    string operation = operationstage[0].Trim();

                    if (operation.Equals("") || (editName && !MERowOperation.rowOps.operations.ContainsKey(operation)) || (!editName && !MECellOperation.cellOps.operations.ContainsKey(operation)))
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
                    if (operationstage.Length == 1)
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation arguments. Add a value, or 'field' followed by the name of a field to take the value from"), null);

                    (var result, var actions) = PerformMassEditCommand(bank, rowSelector==null, paramSelector, rowSelector, cellSelector, editName, operation, operationstage[1], context);
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
        private static (MassEditResult, List<EditorAction>) PerformMassEditCommand(ParamBank bank, bool isParamRowSelector, string paramSelector, string rowSelector, string cellSelector, bool editName, string operation, string opargs, ParamEditorSelectionState context)
        {
            List<EditorAction> partialActions = new List<EditorAction>();
            try {

                int argc;
                Func<Param.Row, string[], object> rowFunc = null;
                Func<(Param.Row, Param.Column), string[], object> cellFunc = null; 
                if (editName)
                {
                    if (!MERowOperation.rowOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation "+operation), null);
                    (argc, rowFunc) = MERowOperation.rowOps.operations[operation];
                }
                else
                {
                    if (!MECellOperation.cellOps.operations.ContainsKey(operation))
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation "+operation), null);
                    (argc, cellFunc) = MECellOperation.cellOps.operations[operation];
                }
                string[] args = opargs.Split(":", argc);

                if (isParamRowSelector)
                {
                    var argFuncs = MEOperationArgument.arg.getContextualArguments(argc, opargs, bank.Params[context.getActiveParam()]);
                    if (argc != argFuncs.Length)
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operation}"), null);
                    foreach (Param.Row row in ParamAndRowSearchEngine.parse.Search(context, paramSelector, false, false))
                    {
                        var argValues = argFuncs.Select((rowFunc, i) => rowFunc.Invoke(row)).ToArray();
                        if (editName)
                        {
                            var res = rowFunc.Invoke(row, argValues);
                            if (res == null)
                                return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', argValues)} on Name"), null);
                            partialActions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, res));
                        }
                        else
                        {
                            foreach (Param.Column col in CellSearchEngine.cse.Search(row, cellSelector, false, false))
                            {
                                var res = cellFunc.Invoke((row, col), argValues);
                                if (res == null)
                                    return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', argValues)} on field {col.Def.InternalName}"), null);
                                addAction(row[col], res, partialActions);
                            }
                        }
                    }
                }
                else
                {
                    foreach (Param p in ParamSearchEngine.pse.Search(false, paramSelector, false, false))
                    {
                        var argFuncs = MEOperationArgument.arg.getContextualArguments(argc, opargs, p);
                        if (argc != argFuncs.Length)
                            return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Invalid number of arguments for operation {operation}"), null);
                        foreach (Param.Row row in RowSearchEngine.rse.Search(p, rowSelector, false, false))
                        {
                            var argValues = argFuncs.Select((rowFunc, i) => rowFunc.Invoke(row)).ToArray();
                            if (editName)
                            {
                                var res = rowFunc.Invoke(row, argValues);
                                if (res == null)
                                    return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', argValues)} on Name"), null);
                                partialActions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, res));
                            }
                            else
                            {
                                foreach (Param.Column col in CellSearchEngine.cse.Search(row, cellSelector, false, false))
                                {
                                    var res = cellFunc.Invoke((row, col), argValues);
                                    if (res == null)
                                        return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {operation} {String.Join(' ', argValues)} on field {col.Def.InternalName}"), null);
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
                actions.Add(new AddParamsAction(p, "legacystring", addedParams, appendOnly, replaceParams, false));
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
            return new AddParamsAction(param, paramName, newRows, true, true, false); //appending same params and allowing overwrite
        }
    }

    internal class MEOperation<T>
    {
        internal Dictionary<string, (int, Func<T, string[], object>)> operations = new Dictionary<string, (int, Func<T, string[], object>)>();
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
    internal class MECellOperation : MEOperation<(Param.Row, Param.Column)>
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
    internal class MERowOperation : MEOperation<Param.Row>
    {
        internal static MERowOperation rowOps = new MERowOperation();
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
        Dictionary<string, (int, Func<Param, string[], Func<Param.Row, string>>)> argumentGetters = new Dictionary<string, (int, Func<Param, string[], Func<Param.Row, string>>)>();
        (int, Func<Param, string, Func<Param.Row, string>>) defaultGetter;

        private MEOperationArgument()
        {
            Setup();
        }
        private void Setup()
        {
            defaultGetter = (0, (param, value) => ((row) => value));
            argumentGetters.Add("field", (1, (param, field) => {
                Param.Column? col = param[field[0]];
                if (col == null)
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }
                return (row) => {
                    object val = row[col].Value;
                    if (val.GetType() == typeof(byte[]))
                        return ParamUtils.Dummy8Write((byte[])val);
                    return val.ToString();
                };
            }));
            argumentGetters.Add("vanillafield", (1, (param, field) => {
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
                    if (val.GetType() == typeof(byte[]))
                        return ParamUtils.Dummy8Write((byte[])val);
                    return val.ToString();
                };
            }));
            argumentGetters.Add("auxfield", (2, (param, bankAndField) => {
                if (!ParamBank.AuxBanks.ContainsKey(bankAndField[0]))
                    throw new Exception($@"Could not locate paramBank {bankAndField[0]}");
                ParamBank bank = ParamBank.AuxBanks[bankAndField[0]];
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
                    if (val.GetType() == typeof(byte[]))
                        return ParamUtils.Dummy8Write((byte[])val);
                    return val.ToString();
                };
            }));
        }

        internal Func<Param.Row, string>[] getContextualArguments(int argumentCount, string opData, Param param)
        {
            string[] opArgs = opData.Split(':', argumentCount);
            Func<Param.Row, string>[] contextualArgs = new Func<Param.Row, string>[opArgs.Length];
            for (int i=0; i<opArgs.Length; i++)
            {
                string[] arg = opArgs[i].Split(" ", 2);
                if (argumentGetters.ContainsKey(arg[0].Trim()))
                {
                    var getter = argumentGetters[arg[0]];
                    string[] opArgArgs = arg[1].Split(" ", getter.Item1);
                    if (opArgArgs.Length != getter.Item1)
                        throw new Exception(@$"Contextual value {arg[0]} has wrong number of arguments. Expected {opArgArgs.Length}");
                    contextualArgs[i] = getter.Item2.Invoke(param, opArgArgs);
                }
                else
                {
                    contextualArgs[i] = defaultGetter.Item2.Invoke(param, opArgs[i]);
                }
            }
            return contextualArgs;
        }
    }
}