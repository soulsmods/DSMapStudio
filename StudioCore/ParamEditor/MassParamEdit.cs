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
        // eg "selection"
        private static readonly string paramrowfilterselection = $@"(selection:\s+)";
        // eg "EquipParamWeapon: "
        private static readonly string paramfilterRx = $@"(param\s+(?<paramrx>[^:]+):\s+)";

        private static readonly string rowfiltersingleRx = $@"(?<rowkey>modified|original)";
        // eg "id (100|200)00"
        private static readonly string rowfiltershortkeyRx = $@"((?<rowkey>id|name)\s+)";
        private static readonly string rowfiltershortRx = $@"({rowfiltershortkeyRx}(?<rowexp>[^:]+))";
        // eg "prop sellValue 100"
        private static readonly string rowfilterlongkeyRx = $@"((?<rowkey>prop|propref)\s+)";
        private static readonly string rowfilterlongRx = $@"({rowfilterlongkeyRx}(?<rowfield>[^:]+)\s+(?<rowexp>[^:]+))";
        public static readonly string rowfilterRx = $@"({rowfiltersingleRx}|{rowfiltershortRx}|{rowfilterlongRx})";
        // eg "correctFaith: "
        private static readonly string fieldRx = $@"(?<fieldrx>[^:]+):\s+";
        // eg "* 2;
        private static readonly string opRx = $@"(?<op>=|\+|-|\*|/|ref|replace)\s+";
        private static readonly string opFieldRx = $@"{opRx}(?<fieldtype>field\s+)?";
        private static readonly string operationRx = $@"{opFieldRx}(?<opparam>[^;]+);";

        private static readonly Regex commandRx = new Regex($@"^({paramrowfilterselection}|({paramfilterRx}{rowfilterRx}:\s+)){fieldRx}{operationRx}$");

        public static List<string> GetRegexAutocomplete(string text, string activeViewParam)
        {
            if (text == null)
            {
                text = "";
            }
            List<string> options = new List<string>();

            if (new Regex($@"({paramrowfilterselection}|({paramfilterRx}{rowfilterRx}:\s+)){fieldRx}{opFieldRx}").IsMatch(text))
            {
                if (activeViewParam == null || !ParamBank.Params.ContainsKey(activeViewParam))
                    return options;
                foreach (PARAMDEF.Field field in ParamBank.Params[activeViewParam].AppliedParamdef.Fields)
                {
                    options.Add(Regex.Escape(field.InternalName)+": ");
                }
                return options;
            }
            if (new Regex($@"({paramrowfilterselection}|({paramfilterRx}{rowfilterRx}:\s+)){fieldRx}{opRx}").IsMatch(text))
            {
                options.Add("field ");
                return options;
            }
            if (new Regex($@"({paramrowfilterselection}|({paramfilterRx}{rowfilterRx}:\s+)){fieldRx}").IsMatch(text))
            {
                options.Add("= ");
                options.Add("+ ");
                options.Add("- ");
                options.Add("* ");
                options.Add("/ ");
                options.Add("ref ");
                options.Add("replace ");
                return options;
            }
            if (new Regex($@"({paramrowfilterselection}|({paramfilterRx}{rowfilterRx}:\s+))").IsMatch(text))
            {
                if (activeViewParam == null || !ParamBank.Params.ContainsKey(activeViewParam))
                    return options;
                foreach (PARAMDEF.Field field in ParamBank.Params[activeViewParam].AppliedParamdef.Fields)
                {
                    options.Add(Regex.Escape(field.InternalName)+": ");
                }
                return options;
            }
            if (new Regex(paramfilterRx+rowfilterlongkeyRx).IsMatch(text))
            {
                if (activeViewParam == null || !ParamBank.Params.ContainsKey(activeViewParam))
                    return options;
                foreach (PARAMDEF.Field field in ParamBank.Params[activeViewParam].AppliedParamdef.Fields)
                {
                    options.Add(Regex.Escape(field.InternalName)+" ");
                }
                return options;
            }
            if (new Regex(paramfilterRx).IsMatch(text))
            {
                options.Add("modified: ");
                options.Add("original: ");
                options.Add("id ");
                options.Add("name ");
                options.Add("prop ");
                options.Add("propref ");
                return options;
            }
            if (text == "")
            {
                options.Add("selection: ");
                foreach(string param in ParamBank.Params.Keys)
                {
                    options.Add("param " + Regex.Escape(param) + ": ");
                }
                return options;
            }
            return options;
        }

        public static (MassEditResult, ActionManager child) PerformMassEdit(string commandsString, string contextActiveParam, List<PARAM.Row> contextActiveRows)
        {
            string[] commands = commandsString.Split('\n');
            int changeCount = 0;
            ActionManager childManager = new ActionManager();
            foreach (string command in commands)
            {
                List<EditorAction> partialActions = new List<EditorAction>();
                Match comm = commandRx.Match(command);
                if (comm.Success)
                {
                    Group paramrx = comm.Groups["paramrx"];
                    Regex fieldRx = new Regex($@"^{comm.Groups["fieldrx"].Value}$");
                    string op = comm.Groups["op"].Value;
                    bool isopparamField = comm.Groups["fieldtype"].Success;
                    string opparam = comm.Groups["opparam"].Value;
                    
                    List<PARAM> affectedParams = new List<PARAM>();
                    if (paramrx.Success)
                        affectedParams = GetMatchingParams(new Regex($@"^{paramrx.Value}$"));
                    else
                        affectedParams.Add(ParamBank.Params[contextActiveParam]);

                    List<PARAM.Row> affectedRows = new List<PARAM.Row>();
                    foreach (PARAM param in affectedParams)
                    {
                        if (!paramrx.Success)
                            affectedRows = contextActiveRows;
                        else
                            affectedRows.AddRange(GetMatchingParamRows(param, comm, false, false));
                    }
                    bool editName = fieldRx.Match("Name").Success;
                    foreach (PARAM.Row row in affectedRows)
                    {
                        List<PARAM.Cell> affectedCells = GetMatchingCells(row, fieldRx);

                        string opparamcontext = opparam;
                        if (isopparamField)
                        {
                            foreach (PARAM.Cell cell in row.Cells)
                            {
                                if (cell.Def.InternalName.Equals(opparam))
                                {
                                    opparamcontext = cell.Value.ToString();
                                    break;
                                }
                            }
                            if (opparamcontext.Equals(opparam))
                                return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not look up field {opparam} in row {row.Name}"), null);
                        }
                        
                        changeCount += affectedCells.Count;
                        foreach (PARAM.Cell cell in affectedCells)
                        {
                            object newval = PerformOperation(cell, op, opparamcontext);
                            if (newval == null)
                            {
                                return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {op} {opparamcontext} on field {cell.Def.InternalName}"), null);
                            }
                            partialActions.Add(new PropertiesChangedAction(cell.GetType().GetProperty("Value"), -1, cell, newval));
                        }
                        if (editName)
                        {
                            string newval = PerformNameOperation(row.Name, op, opparamcontext);
                            if (newval == null)
                            {
                                return (new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {op} {opparamcontext} on name"), null);
                            }
                            partialActions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, newval));
                        
                        }
                    }
                }
                else
                {
                    return (new MassEditResult(MassEditResultType.PARSEERROR, $@"Unrecognised command {command}"), null);
                }
                childManager.ExecuteAction(new CompoundAction(partialActions));
            }
            return (new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected"), childManager);
        }

        public static List<PARAM> GetMatchingParams(Regex paramrx)
        {
            List<PARAM> plist = new List<PARAM>();
            foreach (string name in ParamBank.Params.Keys)
            {
                if (paramrx.Match(name).Success)
                {
                    plist.Add(ParamBank.Params[name]);
                }
            }
            return plist;
        }

        public static List<PARAM.Row> GetMatchingParamRows(PARAM param, Match command, bool lenient, bool failureAllOrNone)
        {
            string rowkeyexp = command.Groups["rowkey"].Value;
            string rowfield = command.Groups["rowfield"].Value;
            string rowexp = command.Groups["rowexp"].Value;
            if (rowkeyexp == "modified")
                return GetMatchingParamRowsByState(param, true, failureAllOrNone);
            else if (rowkeyexp == "original")
                return GetMatchingParamRowsByState(param, false, failureAllOrNone);
            else if (rowkeyexp == "id")
                return GetMatchingParamRowsByID(param, rowexp, lenient, failureAllOrNone);
            else if (rowkeyexp == "name")
                return GetMatchingParamRowsByName(param, rowexp, lenient, failureAllOrNone);
            else if (rowkeyexp == "prop")
                return GetMatchingParamRowsByPropVal(param, rowfield, rowexp, lenient, failureAllOrNone);
            else if (rowkeyexp == "propref")
                return GetMatchingParamRowsByPropRef(param, rowfield, rowexp, lenient, failureAllOrNone);
            else
                return failureAllOrNone ? param.Rows : new List<PARAM.Row>();
        }
        
        public static List<PARAM.Row> GetMatchingParamRowsByState(PARAM param, bool modified, bool failureAllOrNone)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            string paramName = ParamBank.GetKeyForParam(param);
            if (paramName == null)
                return failureAllOrNone ? param.Rows : rlist;
            HashSet<int> cache = ParamBank.DirtyParamCache[paramName];
            if (cache == null)
                return failureAllOrNone ? param.Rows : rlist;
            try
            {
                foreach (PARAM.Row row in param.Rows)
                {
                    if (cache.Contains(row.ID) ^ !modified)
                        rlist.Add(row);
                }
                return rlist;
            }
            catch
            {
                return failureAllOrNone ? param.Rows : rlist;
            }
        }

        public static List<PARAM.Row> GetMatchingParamRowsByID(PARAM param, string rowvalexp, bool lenient, bool failureAllOrNone)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            try
            {
                Regex rx = lenient ? new Regex(rowvalexp) : new Regex($@"^{rowvalexp}$");
                foreach (PARAM.Row row in param.Rows)
                {
                    string term = row.ID.ToString();
                    if (rx.Match(term).Success)
                        rlist.Add(row);
                }
                return rlist;
            }
            catch
            {
                return failureAllOrNone ? param.Rows : rlist;
            }
        }

        public static List<PARAM.Row> GetMatchingParamRowsByName(PARAM param, string namerx, bool lenient, bool failureAllOrNone)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            try
            {
                Regex rownamerx = lenient ? new Regex(namerx.ToLower()) : new Regex($@"^{namerx}$");
                foreach (PARAM.Row row in param.Rows)
                {
                    string nameToMatch = row.Name == null ? "" : row.Name;
                    if (rownamerx.Match(lenient ? nameToMatch.ToLower() : nameToMatch).Success)
                        rlist.Add(row);
                }
                return rlist;
            }
            catch
            {
                return failureAllOrNone ? param.Rows : rlist;
            }
        }

        public static List<PARAM.Row> GetMatchingParamRowsByPropVal(PARAM param, string rowfield, string rowvalexp, bool lenient, bool failureAllOrNone)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            try
            {
                Regex rx = lenient ? new Regex(rowvalexp.ToLower()) : new Regex($@"^{rowvalexp}$");
                foreach (PARAM.Row row in param.Rows)
                {
                    PARAM.Cell c = row[rowfield.Replace(@"\s", " ")];
                    if (c==null)
                        continue;
                    string term = c.Value.ToString();
                    if (c != null && rx.Match(term).Success)
                        rlist.Add(row);
                }
                return rlist;
            }
            catch
            {
                return failureAllOrNone ? param.Rows : rlist;
            }
        }

        public static List<PARAM.Row> GetMatchingParamRowsByPropRef(PARAM param, string rowfield, string namerx, bool lenient, bool failureAllOrNone)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            try
            {
                Regex rownamerx = lenient ? new Regex(namerx.ToLower()) : new Regex($@"^{namerx}$");
                foreach (PARAM.Row row in param.Rows)
                {
                    PARAM.Cell c = row[rowfield.Replace(@"\s", " ")];
                    if (c == null)
                        continue;
                    int val = (int) c.Value;
                    foreach (string rt in FieldMetaData.Get(c.Def).RefTypes)
                    {
                        if (!ParamBank.Params.ContainsKey(rt))
                            continue;
                        PARAM.Row r = ParamBank.Params[rt][val];
                        if (r==null)
                            continue;
                        string nameToMatch = r.Name == null ? "" : r.Name;
                        if (r != null && rownamerx.Match(lenient ? nameToMatch.ToLower() : nameToMatch).Success)
                        {
                            rlist.Add(row);
                            break;
                        }
                    }
                }
                return rlist;
            }
            catch
            {
                return failureAllOrNone ? param.Rows : rlist;
            }
        }

        public static List<PARAM.Cell> GetMatchingCells(PARAM.Row row, Regex fieldrx)
        {
            List<PARAM.Cell> clist = new List<PARAM.Cell>();
            foreach (PARAM.Cell c in row.Cells)
            {
                if (fieldrx.Match(c.Def.InternalName).Success)
                {
                    clist.Add(c);
                }
            }
            return clist;
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
                    string[] csvs = csvLine.Trim().Split(useSpace ? ' ' : ',', 2, StringSplitOptions.RemoveEmptyEntries);
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