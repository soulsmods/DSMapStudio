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
        protected static object PerformOperation(Param.Row row, Param.Cell cell, string op, string opparam)
        {
            try
            {
                if (op.Equals("ref"))
                {
                    if (cell.ValueType == typeof(int))
                    {
                        foreach (string reftype in FieldMetaData.Get(cell.Def).RefTypes)
                        {
                            var p = ParamBank.Params[reftype];
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
                    if (cell.ValueType == typeof(bool))
                        return bool.Parse(opparam);
                    else if (cell.ValueType == typeof(string))
                        return opparam;
                    else if (cell.ValueType == typeof(byte[]))
                        return ParamUtils.Dummy8Read(opparam, ((byte[])cell.GetValue(row)).Length);
                }
                
                if (cell.ValueType == typeof(long))
                    return PerformBasicOperation<long>(row, cell, op, double.Parse(opparam));
                if (cell.ValueType == typeof(ulong))
                    return PerformBasicOperation<ulong>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(int))
                    return PerformBasicOperation<int>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(uint))
                    return PerformBasicOperation<uint>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(short))
                    return PerformBasicOperation<short>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(ushort))
                    return PerformBasicOperation<ushort>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(sbyte))
                    return PerformBasicOperation<sbyte>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(byte))
                    return PerformBasicOperation<byte>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(float))
                    return PerformBasicOperation<float>(row, cell, op, double.Parse(opparam));
                else if (cell.ValueType == typeof(double))
                    return PerformBasicOperation<double>(row,cell, op, double.Parse(opparam));
            }
            catch
            {
            }
            return null;
        }
        public static string PerformNameOperation(string name, string op, string opparam)
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

        public static T PerformBasicOperation<T>(Param.Row row, Param.Cell c, string op, double opparam) where T : struct, IFormattable
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
    }

    public class MassParamEditRegex : MassParamEdit
    {
        public static (MassEditResult, ActionManager child) PerformMassEdit(string commandsString, ParamEditorSelectionState context)
        {
            string[] commands = commandsString.Split('\n');
            int changeCount = 0;
            ActionManager childManager = new ActionManager();
            foreach (string cmd in commands)
            {
                string command = cmd.Trim();
                if (command.EndsWith(';'))
                    command = command.Substring(0, command.Length-1);

                List<EditorAction> partialActions = new List<EditorAction>();

                string[] stages = command.Split(":", StringSplitOptions.TrimEntries);


                List<Param> affectedParams = new List<Param>();
                List<Param.Row> affectedRows = new List<Param.Row>();
                List<Param.Cell> affectedCells = new List<Param.Cell>();
                int stage = 0;
                if (stages[stage].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find param filter. Add : and one of "+String.Join(", ", ParamSearchEngine.pse.AvailableCommands())+" or "+String.Join(", ", ParamAndRowSearchEngine.parse.AvailableCommands())), null);
                if (ParamAndRowSearchEngine.parse.HandlesCommand(stages[stage]))
                {
                    affectedRows = ParamAndRowSearchEngine.parse.Search(context, stages[stage], false, false);
                    stage++;
                }
                else
                {
                    affectedParams = ParamSearchEngine.pse.Search(false, stages[stage], false, false);
                    stage++;
                    if (stage>=stages.Length || stages[stage].Equals(""))
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter. Add : and one of "+String.Join(", ", RowSearchEngine.rse.AvailableCommands())), null);
                    affectedRows = RowSearchEngine.rse.Search(affectedParams, stages[stage], false, false);
                    stage++;
                }

                if (stage>=stages.Length || stages[stage].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell/property filter. Add : and one of "+String.Join(", ", CellSearchEngine.cse.AvailableCommands())+" or Name (0 args)"), null);
                int cellStage = stage;
                //hack
                string[] cellStageSplit = stages[cellStage].Split(" ", StringSplitOptions.TrimEntries);
                bool editName = cellStageSplit.Length == 1 && cellStageSplit[0].Equals("Name");
                //hack
                stage++;

                //skip ahead to op stage
                if (stage>=stages.Length || stages[stage].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform. Add : and one of + - * / replace"), null);
                string[] operation = stages[stage].Split(" ", 2, StringSplitOptions.TrimEntries);
                string op = operation[0];
                if(!"= + - * / replace".Contains(op))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation "+op), null);
                
                if (operation.Length<2 || operation[1].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation argument. Add a number, or 'field' followed by a field to read a number from."), null);
                bool readField = false;
                string[] opArgs = operation[1].Split(" ", 2, StringSplitOptions.TrimEntries);
                if (opArgs.Length > 1 && opArgs[0].Equals("field"))
                    readField = true;
                //back to cell stage - this is because we want row context ["cell" as a stage is inaccurate, do (row, cell)? (row,propertyinfo)?]
                foreach(var row in affectedRows)
                {
                    string valueToUse = opArgs[0];
                    if (readField)
                    {
                        Param.CellHandle? reffed = row[opArgs[1]];
                        if (reffed == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not read field {opArgs[1]}"), null);
                        valueToUse = reffed.Value.Value.ToString();
                    }

                    affectedCells = CellSearchEngine.cse.Search(row, stages[cellStage], false, false);

                    changeCount += affectedCells.Count;
                    foreach (Param.Cell cell in affectedCells)
                    {
                        object newval = PerformOperation(row, cell, op, valueToUse);
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {op} {valueToUse} on field {cell.Def.InternalName}"), null);
                        partialActions.Add(new PropertiesChangedAction(cell.GetType().GetProperty("Value"), -1, cell, newval));
                    }
                    if (editName)
                    {
                        string newval = PerformNameOperation(row.Name, op, valueToUse);
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {op} {valueToUse} on name"), null);
                        partialActions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, newval));
                    
                    }
                }
                childManager.ExecuteAction(new CompoundAction(partialActions));
            }
            return (new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected"), childManager);
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
                foreach (Param.Cell cell in row.Cells)
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
            string gen = "";
            gen += GenerateColumnLabels(param, separator);
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
                    rowgen = $@"{row.ID}{separator}{row[field].Value}";
                }
                gen += rowgen + "\n";
            }
            return gen;
        }

        public static bool IsIanACuck()
        {
            bool yes = true;
            return yes && true;
        }

        public static bool IsIanASimp()
        {
            return IsIanACuck() && true;
        }
        
        public static MassEditResult PerformMassEdit(string csvString, ActionManager actionManager, string param, bool appendOnly, bool replaceParams, char separator)
        {
            try
            {
                Param p = ParamBank.Params[param];
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
                    if (row.Name != null && !row.Name.Equals(name))
                        actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, name));
                    int index = 2;
                    foreach (Param.Cell c in row.Cells)
                    {
                        string v = csvs[index];
                        index++;
                        object newval = PerformOperation(row, c, "=", v);
                        if (newval == null)
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {v} to field {c.Def.InternalName}");
                        var handle = row[c];
                        if (!(handle.Value.Equals(newval) || (handle.Value.GetType()==typeof(byte[]) && ParamUtils.ByteArrayEquals((byte[])handle.Value, (byte[])newval))))
                            actions.Add(new PropertiesChangedAction(c.GetType().GetProperty("Value"), -1, c, newval));
                    }
                }
                changeCount = actions.Count;
                addedCount = addedParams.Count;
                actions.Add(new AddParamsAction(p, "legacystring", addedParams, appendOnly, replaceParams, false));
                if (changeCount != 0 || addedCount != 0)
                    actionManager.ExecuteAction(new CompoundAction(actions));
                return new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected, {addedCount} rows added");
            }
            catch
            {
                return new MassEditResult(MassEditResultType.PARSEERROR, "Unable to parse CSV into correct data types");
            }
        }
        public static (MassEditResult, CompoundAction) PerformSingleMassEdit(string csvString, string param, string field, char separator)
        {
            try
            {
                Param p = ParamBank.Params[param];
                if (p == null)
                    return (new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected"), null);
                string[] csvLines = csvString.Split("\n");
                if (csvLines[0].Contains($@"ID{separator}Name"))
                    csvLines[0] = ""; //skip column label row
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
                        return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate row {id}"), null);
                    if (field.Equals("Name"))
                    {
                        if (row.Name != null && !row.Name.Equals(value))
                            actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, value));
                    }
                    else
                    {
                        Param.Cell? cell = p[field];
                        if (cell == null)
                        {
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate field {field}"), null);
                        }
                        object newval = PerformOperation(row, cell, "=", value);
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {value} to field {cell.Def.InternalName}"), null);
                        if (!(cell.GetValue(row).Equals(newval) || (cell.GetValue(row).GetType()==typeof(byte[]) && ParamUtils.ByteArrayEquals((byte[])cell.GetValue(row), (byte[])newval))))
                            actions.Add(new PropertiesChangedAction(cell.GetType().GetProperty("Value"), -1, cell, newval));
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
        public static AddParamsAction SortRows(string paramName)
        {
            Param param = ParamBank.Params[paramName];
            List<Param.Row> newRows = new List<Param.Row>(param.Rows);
            newRows.Sort((Param.Row a, Param.Row b)=>{return a.ID - b.ID;});
            return new AddParamsAction(param, paramName, newRows, true, true, false); //appending same params and allowing overwrite
        }
    }
}