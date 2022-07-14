using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;
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
        public static object PerformOperation(PARAM.Cell cell, string op, string opparam)
        {
            try
            {
                if (op.Equals("ref"))
                {
                    if (cell.Value.GetType() == typeof(int))
                    {
                        foreach (string reftype in FieldMetaData.Get(cell.Def).RefTypes)
                        {
                            PARAM p = ParamBank.Params[reftype];
                            if (p == null)
                                continue;
                            foreach (PARAM.Row r in p.Rows)
                            {
                                if (r.Name == null)
                                    continue;
                                if (r.Name.Equals(opparam))
                                    return (int) r.ID;
                            }
                        }
                    }
                }
                if (op.Equals("="))
                {
                    if (cell.Value.GetType() == typeof(bool))
                        return bool.Parse(opparam);
                    else if (cell.Value.GetType() == typeof(string))
                        return opparam;
                }
                
                if (cell.Value.GetType() == typeof(long))
                    return PerformBasicOperation<long>(cell, op, double.Parse(opparam));
                if (cell.Value.GetType() == typeof(ulong))
                    return PerformBasicOperation<ulong>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(int))
                    return PerformBasicOperation<int>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(uint))
                    return PerformBasicOperation<uint>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(short))
                    return PerformBasicOperation<short>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(ushort))
                    return PerformBasicOperation<ushort>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(sbyte))
                    return PerformBasicOperation<sbyte>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(byte))
                    return PerformBasicOperation<byte>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(float))
                    return PerformBasicOperation<float>(cell, op, double.Parse(opparam));
                else if (cell.Value.GetType() == typeof(double))
                    return PerformBasicOperation<double>(cell, op, double.Parse(opparam));
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

        public static T PerformBasicOperation<T>(PARAM.Cell c, string op, double opparam) where T : struct, IFormattable
        {
            try
            {
                dynamic val = c.Value;
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
            foreach (string command in commands)
            {
                List<EditorAction> partialActions = new List<EditorAction>();

                string[] stages = command.Split(":", StringSplitOptions.TrimEntries);


                List<PARAM> affectedParams = new List<PARAM>();
                List<PARAM.Row> affectedRows = new List<PARAM.Row>();
                List<PARAM.Cell> affectedCells = new List<PARAM.Cell>();
                int stage = 0;
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
                        return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find row filter"), null);
                    affectedRows = RowSearchEngine.rse.Search(affectedParams, stages[stage], false, false);
                    stage++;
                }

                if (stage>=stages.Length || stages[stage].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find cell/property filter"), null);
                int cellStage = stage;
                //hack
                string[] cellStageSplit = stages[cellStage].Split(" ", StringSplitOptions.TrimEntries);
                bool editName = cellStageSplit.Length == 1 && cellStageSplit[0].Equals("Name");
                //hack
                stage++;

                //skip ahead to op stage
                if (stage>=stages.Length || stages[stage].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation to perform"), null);
                string[] operation = stages[stage].Split(" ", 2, StringSplitOptions.TrimEntries);
                string op = operation[0];
                if(!"= + - * / replace".Contains(op))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unknown operation"), null);
                
                if (operation.Length<2 || operation[1].Equals(""))
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Could not find operation argument"), null);
                bool readField = false;
                string[] opArgs = operation[1].Split(" ", 2, StringSplitOptions.TrimEntries);
                if (opArgs.Length > 1 && opArgs[0].Equals("field"))
                    readField = true;
                //back to cell stage - this is because we want row context ["cell" as a stage is inaccurate, do (row, cell)? (row,propertyinfo)?]
                foreach(PARAM.Row row in affectedRows)
                {
                    string valueToUse = opArgs[0];
                    if (readField)
                        valueToUse = row[opArgs[0]].Value.ToString();

                    affectedCells = CellSearchEngine.cse.Search(row, stages[cellStage], false, false);

                    changeCount += affectedCells.Count;
                    foreach (PARAM.Cell cell in affectedCells)
                    {
                        object newval = PerformOperation(cell, op, valueToUse);
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
        public static string GenerateColumnLabels(PARAM param)
        {
            string str = "";
            str += "ID,Name,";
            foreach (var f in param.AppliedParamdef.Fields)
            {
                string rowgen = $@"{f.InternalName}";
                str += rowgen + ",";
            }
            return str+"\n";
        }
        
        public static string GenerateCSV(List<PARAM.Row> rows, PARAM param)
        {
            string gen = "";
            gen += GenerateColumnLabels(param);

            foreach (PARAM.Row row in rows)
            {
                string rowgen = $@"{row.ID},{row.Name}";
                foreach (PARAM.Cell cell in row.Cells)
                {
                    rowgen += $@",{cell.Value}";
                }
                gen += rowgen + "\n";
            }
            return gen;
        }
        public static string GenerateSingleCSV(List<PARAM.Row> rows, PARAM param, string field)
        {
            string gen = "";
            gen += GenerateColumnLabels(param);
            foreach (PARAM.Row row in rows)
            {
                string rowgen;
                if (field.Equals("Name"))
                    rowgen = $@"{row.ID},{row.Name}";
                else
                {
                    rowgen = $@"{row.ID},{row[field].Value}";
                }
                gen += rowgen + "\n";
            }
            return gen;
        }
        
        public static MassEditResult PerformMassEdit(string csvString, ActionManager actionManager, string param, bool appendOnly, bool replaceParams)
        {
            try
            {
                PARAM p = ParamBank.Params[param];
                if (p == null)
                    return new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected");
                int csvLength = p.AppliedParamdef.Fields.Count + 2;// Include ID and name
                string[] csvLines = csvString.Split("\n");
                if (csvLines[0].Contains("ID,Name"))
                    csvLines[0] = ""; //skip column label row
                int changeCount = 0;
                int addedCount = 0;
                List<EditorAction> actions = new List<EditorAction>();
                List<PARAM.Row> addedParams = new List<PARAM.Row>();
                foreach (string csvLine in csvLines)
                {
                    if (csvLine.Trim().Equals(""))
                        continue;
                    string[] csvs = csvLine.Trim().Split(',');
                    if (csvs.Length != csvLength || csvs.Length < 2)
                    {
                        return new MassEditResult(MassEditResultType.PARSEERROR, "CSV has wrong number of values");
                    }
                    int id = int.Parse(csvs[0]);
                    string name = csvs[1];
                    PARAM.Row row = p[id];
                    if (row == null || replaceParams)
                    {
                        row = new PARAM.Row(id, name, p.AppliedParamdef);
                        addedParams.Add(row);
                    }
                    if (row.Name == null || !row.Name.Equals(name))
                        actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, name));
                    int index = 2;
                    foreach (PARAM.Cell c in row.Cells)
                    {
                        string v = csvs[index];
                        index++;
                        // Array types are unhandled
                        if (c.Value.GetType().IsArray)
                            continue;
                        object newval = PerformOperation(c, "=", v);
                        if (newval == null)
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {v} to field {c.Def.InternalName}");
                        if (!c.Value.Equals(newval))
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
        public static (MassEditResult, CompoundAction) PerformSingleMassEdit(string csvString, string param, string field, bool useSpace)
        {
            try
            {
                PARAM p = ParamBank.Params[param];
                if (p == null)
                    return (new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected"), null);
                string[] csvLines = csvString.Split("\n");
                if (csvLines[0].Contains("ID,Name"))
                    csvLines[0] = ""; //skip column label row
                int changeCount = 0;
                List<EditorAction> actions = new List<EditorAction>();
                foreach (string csvLine in csvLines)
                {
                    if (csvLine.Trim().Equals(""))
                        continue;
                    string[] csvs = csvLine.Trim().Split(useSpace ? ' ' : ',', 2);
                    if (csvs.Length != 2)
                        return (new MassEditResult(MassEditResultType.PARSEERROR, "CSV has wrong number of values"), null);
                    int id = int.Parse(csvs[0]);
                    string value = csvs[1];
                    PARAM.Row row = p[id];
                    if (row == null)
                        return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate row {id}"), null);
                    if (field.Equals("Name"))
                    {
                        if (row.Name == null || !row.Name.Equals(value))
                            actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, value));
                    }
                    else
                    {
                        PARAM.Cell cell = row[field];
                        if (cell == null)
                        {
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate field {field}"), null);
                        }
                        // Array types are unhandled
                        if (cell.Value.GetType().IsArray)
                            continue;
                        object newval = PerformOperation(cell, "=", value);
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {value} to field {cell.Def.InternalName}"), null);
                        if (!cell.Value.Equals(newval))
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
            PARAM param = ParamBank.Params[paramName];
            List<PARAM.Row> newRows = new List<PARAM.Row>(param.Rows.ToArray());
            newRows.Sort((PARAM.Row a, PARAM.Row b)=>{return a.ID - b.ID;});
            return new AddParamsAction(param, paramName, newRows, true, true, false); //appending same params and allowing overwrite
        }
    }
}