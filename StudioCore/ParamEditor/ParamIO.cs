#nullable enable
using FSParam;
using SoulsFormats;
using StudioCore.Editor;
using System;
using System.Collections.Generic;

namespace StudioCore.ParamEditor;

public class ParamIO
{
    public static string GenerateColumnLabels(Param param, char separator)
    {
        var str = "";
        str += $@"ID{separator}Name{separator}";
        foreach (PARAMDEF.Field? f in param.AppliedParamdef.Fields)
        {
            str += $@"{f.InternalName}{separator}";
        }

        return str + "\n";
    }

    public static string GenerateCSV(IReadOnlyList<Param.Row> rows, Param param, char separator)
    {
        var gen = "";
        gen += GenerateColumnLabels(param, separator);

        foreach (Param.Row row in rows)
        {
            var name = row.Name == null ? "null" : row.Name.Replace(separator, '-');
            var rowgen = $@"{row.ID}{separator}{name}";
            foreach (Param.Column cell in row.Columns)
            {
                rowgen += $@"{separator}{row[cell].Value.ToParamEditorString()}";
            }

            gen += rowgen + "\n";
        }

        return gen;
    }

    public static string GenerateSingleCSV(IReadOnlyList<Param.Row> rows, Param param, string field, char separator)
    {
        var gen = $@"ID{separator}{field}" + "\n";
        foreach (Param.Row row in rows)
        {
            string rowgen;
            if (field.Equals("Name"))
            {
                var name = row.Name == null ? "null" : row.Name.Replace(separator, '-');
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

    public static (string, CompoundAction?) ApplyCSV(ParamBank bank, string csvString, string param,
        bool appendOnly, bool replaceParams, char separator)
    {
#if !DEBUG
            try
            {
#endif
        Param p = bank.Params[param];
        if (p == null)
        {
            return ("No Param selected", null);
        }

        var csvLength = p.AppliedParamdef.Fields.Count + 2; // Include ID and name
        var csvLines = csvString.Split("\n");
        if (csvLines[0].StartsWith($@"ID{separator}Name"))
        {
            csvLines[0] = ""; //skip column label row
        }

        var changeCount = 0;
        var addedCount = 0;
        List<EditorAction> actions = new();
        List<Param.Row> addedParams = new();
        foreach (var csvLine in csvLines)
        {
            if (csvLine.Trim().Equals(""))
            {
                continue;
            }

            var csvs = csvLine.Trim().Split(separator);
            if (csvs.Length != csvLength && !(csvs.Length == csvLength + 1 && csvs[csvLength].Trim().Equals("")))
            {
                return ("CSV has wrong number of values", null);
            }

            var id = int.Parse(csvs[0]);
            var name = csvs[1];
            Param.Row? row = p[id];
            if (row == null || replaceParams)
            {
                row = new Param.Row(id, name, p);
                addedParams.Add(row);
            }

            if (!name.Equals(row.Name))
            {
                actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, name));
            }

            var index = 2;
            foreach (Param.Column col in row.Columns)
            {
                var v = csvs[index];
                index++;

                if (col.ValueType.IsArray)
                {
                    var newval = ParamUtils.Dummy8Read(v, col.Def.ArrayLength);
                    if (newval == null)
                    {
                        return ($@"Could not assign {v} to field {col.Def.InternalName}", null);
                    }

                    actions.AppendParamEditAction(row, (PseudoColumn.None, col), newval);
                }
                else
                {
                    var newval = Convert.ChangeType(v, row.Get((PseudoColumn.None, col)).GetType());
                    if (newval == null)
                    {
                        return ($@"Could not assign {v} to field {col.Def.InternalName}", null);
                    }

                    actions.AppendParamEditAction(row, (PseudoColumn.None, col), newval);
                }
            }
        }

        changeCount = actions.Count;
        addedCount = addedParams.Count;
        if (addedCount != 0)
        {
            actions.Add(new AddParamsAction(p, "legacystring", addedParams, appendOnly, replaceParams));
        }

        return ($@"{changeCount} cells affected, {addedCount} rows added", new CompoundAction(actions));
#if !DEBUG
            }
            catch
            {
                return ("Unable to parse CSV into correct data types", null);
            }
#else
        return ("Unable to parse CSV into correct data types", null);
#endif
    }

    public static (string, CompoundAction?) ApplySingleCSV(ParamBank bank, string csvString, string param,
        string field, char separator, bool ignoreMissingRows, bool onlyAffectEmptyNames = false)
    {
        try
        {
            Param p = bank.Params[param];
            if (p == null)
            {
                return ("No Param selected", null);
            }

            var csvLines = csvString.Split("\n");
            if (csvLines[0].Trim().StartsWith($@"ID{separator}"))
            {
                if (!csvLines[0].Contains($@"ID{separator}{field}"))
                {
                    return ("CSV has wrong field name", null);
                }

                csvLines[0] = ""; //skip column label row
            }

            var changeCount = 0;
            List<EditorAction> actions = new();
            foreach (var csvLine in csvLines)
            {
                if (csvLine.Trim().Equals(""))
                {
                    continue;
                }

                var csvs = csvLine.Trim().Split(separator, 2);
                if (csvs.Length != 2 && !(csvs.Length == 3 && csvs[2].Trim().Equals("")))
                {
                    return ("CSV has wrong number of values", null);
                }

                var id = int.Parse(csvs[0]);
                var value = csvs[1];
                Param.Row? row = p[id];
                if (row == null)
                {
                    if (ignoreMissingRows)
                    {
                        continue;
                    }

                    return ($@"Could not locate row {id}", null);
                }

                if (field.Equals("Name"))
                {
                    // 'onlyAffectEmptyNames' argument check is exclusively relevant for "Import Row Names" function at the moment.
                    if (!value.Equals(row.Name) &&
                        (onlyAffectEmptyNames == false || string.IsNullOrEmpty(row.Name)))
                    {
                        actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, value));
                    }
                }
                else
                {
                    Param.Column? col = p[field];
                    if (col == null)
                    {
                        return ($@"Could not locate field {field}", null);
                    }

                    if (col.ValueType.IsArray)
                    {
                        var newval = ParamUtils.Dummy8Read(value, col.Def.ArrayLength);
                        if (newval == null)
                        {
                            return ($@"Could not assign {value} to field {col.Def.InternalName}", null);
                        }

                        actions.AppendParamEditAction(row, (PseudoColumn.None, col), newval);
                    }
                    else
                    {
                        var newval = Convert.ChangeType(value, row.Get((PseudoColumn.None, col)).GetType());
                        if (newval == null)
                        {
                            return ($@"Could not assign {value} to field {col.Def.InternalName}", null);
                        }

                        actions.AppendParamEditAction(row, (PseudoColumn.None, col), newval);
                    }
                }
            }

            changeCount = actions.Count;
            return ($@"{changeCount} rows affected", new CompoundAction(actions));
        }
        catch
        {
            return ("Unable to parse CSV into correct data types", null);
        }
    }
}
