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
    public class ParamIO
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
        
        public static string GenerateCSV(IReadOnlyList<Param.Row> rows, Param param, char separator)
        {
            string gen = "";
            gen += GenerateColumnLabels(param, separator);

            foreach (Param.Row row in rows)
            {
                string name = row.Name==null ? "null" : row.Name.Replace(separator, '-');
                string rowgen = $@"{row.ID}{separator}{name}";
                foreach (Param.Column cell in row.Columns)
                    rowgen += $@"{separator}{row[cell].Value.ToParamEditorString()}";
                gen += rowgen + "\n";
            }
            return gen;
        }
        public static string GenerateSingleCSV(IReadOnlyList<Param.Row> rows, Param param, string field, char separator)
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
                    rowgen = $@"{row.ID}{separator}{row[field].Value.Value.ToParamEditorString()}";
                }
                gen += rowgen + "\n";
            }
            return gen;
        }
        
        public static MassEditResult ApplyCSV(ParamBank bank, string csvString, ActionManager actionManager, string param, bool appendOnly, bool replaceParams, char separator)
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
                if (csvLines[0].StartsWith($@"ID{separator}Name"))
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
                    foreach (Param.Column col in row.Columns)
                    {
                        string v = csvs[index];
                        index++;
                        object newval = Convert.ChangeType(v, row.Get((PseudoColumn.None, col)).GetType());
                        if (newval == null)
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {v} to field {col.Def.InternalName}");
                        actions.AppendParamEditAction(row, (PseudoColumn.None, col), newval);
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
        public static (MassEditResult, CompoundAction) ApplySingleCSV(ParamBank bank, string csvString, string param, string field, char separator, bool ignoreMissingRows, bool onlyAffectEmptyNames = false)
        {
            try
            {
                Param p = bank.Params[param];
                if (p == null)
                    return (new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected"), null);
                string[] csvLines = csvString.Split("\n");
                if (csvLines[0].Trim().StartsWith($@"ID{separator}"))
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
                        // 'onlyAffectEmptyNames' argument check is exclusively relevant for "Import Row Names" function at the moment.
                        if (!value.Equals(row.Name) && (onlyAffectEmptyNames == false || string.IsNullOrEmpty(row.Name)))
                            actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, value));
                    }
                    else
                    {
                        Param.Column? col = p[field];
                        if (col == null)
                        {
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not locate field {field}"), null);
                        }
                        object newval = Convert.ChangeType(value, row.Get((PseudoColumn.None, col)).GetType());
                        if (newval == null)
                            return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {value} to field {col.Def.InternalName}"), null);
                        actions.AppendParamEditAction(row, (PseudoColumn.None, col), newval);
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
}