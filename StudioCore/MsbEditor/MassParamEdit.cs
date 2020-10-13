using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;
using ImGuiNET;
using SoulsFormats;

namespace StudioCore.MsbEditor
{

    public enum MassEditResultType
    {
        SUCCESS,
        PARSEERROR,
        OPERATIONERROR
    }
    public class MassEditResult
    {
        public MassEditResultType type;
        public string information;
        public MassEditResult(MassEditResultType result, string info)
        {
            type = result;
            information = info;
        }
    }
    //It's all static baby yeah
    public class MassParamEdit
    {
        public static object performOperation(PARAM.Cell cell, string op, string opparam)
        {
            try
            {
                if(op.Equals("ref"))
                {
                    if(cell.Value.GetType()==typeof(int))
                    {
                        foreach(string reftype in cell.Def.Meta.RefTypes)
                        {
                            PARAM p = ParamBank.Params[reftype];
                            if(p==null)
                                continue;
                            foreach(PARAM.Row r in p.Rows)
                            {
                                if(r.Name==null)
                                    continue;
                                if(r.Name.Equals(opparam))
                                    return (int)r.ID;
                            }
                        }
                    }
                }
                if(op.Equals("="))
                {
                    if(cell.Value.GetType()==typeof(bool))
                        return bool.Parse(opparam);
                    else if(cell.Value.GetType()==typeof(string))
                        return opparam;
                }
                //csgrad meme
                if(cell.Value.GetType()==typeof(long))
                    return performBasicOperation<long>(cell, op, double.Parse(opparam));
                else if(cell.Value.GetType()==typeof(int))
                    return performBasicOperation<int>(cell, op, double.Parse(opparam));
                else if(cell.Value.GetType()==typeof(short))
                    return performBasicOperation<short>(cell, op, double.Parse(opparam));
                else if(cell.Value.GetType()==typeof(ushort))
                    return performBasicOperation<ushort>(cell, op, double.Parse(opparam));
                else if(cell.Value.GetType()==typeof(sbyte))
                    return performBasicOperation<sbyte>(cell, op, double.Parse(opparam));
                else if(cell.Value.GetType()==typeof(byte))
                    return performBasicOperation<byte>(cell, op, double.Parse(opparam));
                else if(cell.Value.GetType()==typeof(float))
                    return performBasicOperation<float>(cell, op, double.Parse(opparam));
            }
            catch(FormatException f)
            {
            }
            return null;
        }
        public static T performBasicOperation<T>(PARAM.Cell c, string op, double opparam) where T : struct, IFormattable
        {
            try
            {
                dynamic val = c.Value;
                dynamic opp = opparam;
                //this is a hellish mess
                //I feel like the cs grad meme
                if(op.Equals("="))
                    return (T)(opp);
                else if(op.Equals("+"))
                    return (T)(val+opp);
                else if(op.Equals("-"))
                    return (T)(val-opp);
                else if(op.Equals("*"))
                    return (T)(val*opp);
                else if(op.Equals("/"))
                    return (T)(val/opp);
            }
            catch(Exception e){
                //Operation error
            }
            return default(T);
        }
    }

    public class MassParamEditRegex : MassParamEdit
    {
        //eg "EquipParamWeapon: "
        private static string paramfilterRx = $@"(?<paramrx>[^\s:]+):\s+";
        //eg "id (100|200)00"
        private static string rowfilteridRx = $@"(id\s+(?<rowidexp>[^:]+))";
        //eg "name \s+ Arrow"
        private static string rowfilternameRx = $@"(name\s+(?<rownamerx>[^:]+))";
        //eg "prop sellValue 100"
        private static string rowfilterpropRx = $@"(prop\s+(?<rowpropfield>[^\s]+)\s+(?<rowpropvalexp>[^:]+))";
        //eg "propref originalEquipWep0 Dagger\[.+\]"
        private static string rowfilterproprefRx = $@"(propref\s+(?<rowpropreffield>[^\s]+)\s+(?<rowproprefnamerx>[^:]+))";
        private static string rowfilterRx = $@"({rowfilteridRx}|{rowfilternameRx}|{rowfilterpropRx}|{rowfilterproprefRx}):\s+";
        //eg "correctFaith: "
        private static string fieldRx = $@"(?<fieldrx>[^\:]+):\s+";
        //eg "* 2;
        private static string operationRx = $@"(?<op>=|\+|-|\*|/|ref)\s+(?<opparam>[^;]+);";
        private static Regex commandRx = new Regex($@"^{paramfilterRx}{rowfilterRx}{fieldRx}{operationRx}$");
        public static MassEditResult PerformMassEdit(string commandsString, ActionManager actionManager)
        {
            string[] commands = commandsString.Split('\n');
            int changeCount = 0;
            List<Action> actions = new List<Action>();
            foreach(string command in commands)
            {
                Match comm = commandRx.Match(command);
                if(comm.Success)
                {
                    Regex paramRx = new Regex($@"^{comm.Groups["paramrx"].Value}$");
                    Regex fieldRx = new Regex($@"^{comm.Groups["fieldrx"].Value}$");
                    string op = comm.Groups["op"].Value;
                    string opparam = comm.Groups["opparam"].Value;
                    Group rowidexp = comm.Groups["rowidexp"];
                    Group rownamerx = comm.Groups["rownamerx"];
                    Group rowpropfield = comm.Groups["rowpropfield"];
                    Group rowpropreffield = comm.Groups["rowpropreffield"];

                    List<PARAM> affectedParams = getMatchingParams(paramRx);
                    List<PARAM.Row> affectedRows = new List<PARAM.Row>();
                    foreach(PARAM param in affectedParams)
                    {//not ideal to loop here as we rebuild regexes/recheck which method we use, but it's clean
                        if(rowidexp.Success)
                            affectedRows = getMatchingParamRows(param, rowidexp.Value);
                        else if(rownamerx.Success)
                            affectedRows = getMatchingParamRows(param, new Regex($@"^{rownamerx.Value}$"));
                        else if(rowpropfield.Success)
                            affectedRows = getMatchingParamRows(param, rowpropfield.Value, comm.Groups["rowpropvalexp"].Value);
                        else if(rowpropreffield.Success)
                            affectedRows = getMatchingParamRows(param, rowpropreffield.Value, new Regex($@"^{comm.Groups["rowproprefnamerx"].Value}$"));
                        //somehow matched but also failed to identify a row address
                    }
                    List<PARAM.Cell> affectedCells = getMatchingCells(affectedRows, fieldRx);
                    changeCount = affectedCells.Count;
                    foreach(PARAM.Cell cell in affectedCells)
                    {
                        object newval = performOperation(cell, op, opparam);
                        if(newval == null)
                        {
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not perform operation {op} {opparam} on field {cell.Def.DisplayName}");
                        }
                        actions.Add(new PropertiesChangedAction(cell.GetType().GetProperty("Value"), -1, cell, newval));
                    }
                }
                else
                {
                    return new MassEditResult(MassEditResultType.PARSEERROR, $@"Unrecognised command {command}");
                }
            }
            actionManager.ExecuteAction(new CompoundAction(actions));
            return new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected");
        }
        public static List<PARAM> getMatchingParams(Regex paramrx)
        {
            List<PARAM> plist = new List<PARAM>();
            foreach(string name in ParamBank.Params.Keys)
            {
                if(paramrx.Match(name).Success)
                {
                    plist.Add(ParamBank.Params[name]);
                }
            }
            return plist;
        }
        public static List<PARAM.Row> getMatchingParamRows(PARAM param, string rowvalexp)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows)
            {
                if(matchNumExp(row.ID, rowvalexp))
                    rlist.Add(row);
            }
            return rlist;
        }
        public static List<PARAM.Row> getMatchingParamRows(PARAM param, Regex rownamerx)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows)
            {
                if(rownamerx.Match(row.Name==null?"":row.Name).Success)
                    rlist.Add(row);
            }
            return rlist;
        }
        public static List<PARAM.Row> getMatchingParamRows(PARAM param, string rowfield, string rowvalexp)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows)
            {
                PARAM.Cell c = row[rowfield];
                if(c!=null && matchNumExp(c.Value, rowvalexp))
                    rlist.Add(row);
            }
            return rlist;
        }
        public static List<PARAM.Row> getMatchingParamRows(PARAM param, string rowfield, Regex rownamerx)
        {
            List<PARAM.Row> rlist = new List<PARAM.Row>();
            foreach(PARAM.Row row in param.Rows)
            {
                PARAM.Cell c = row[rowfield];
                if(c!=null)
                {
                    int val = (int)c.Value;//assume int value
                    foreach(string rt in c.Def.Meta.RefTypes)
                    {
                        if(!ParamBank.Params.ContainsKey(rt))
                            continue;
                        PARAM.Row r = ParamBank.Params[rt][val];
                        if(r!=null && rownamerx.Match(r.Name==null?"":r.Name).Success)
                        {
                            rlist.Add(row);//don't add the ref'd row lol
                            break;//no need to check other refs
                        }
                    }
                }
            }
            return rlist;
        }
        public static List<PARAM.Cell> getMatchingCells(List<PARAM.Row> rows, Regex fieldrx)
        {
            List<PARAM.Cell> clist = new List<PARAM.Cell>();
            foreach(PARAM.Row row in rows){//we love seeing loops on top of loops so brazenly
                foreach(PARAM.Cell c in row.Cells)
                {
                    if(fieldrx.Match(c.Def.DisplayName).Success)
                    {//using displayname instead of internal. questionable?
                        clist.Add(c);
                        //intentionally not breaking - potential for changing multiple cells at once eg. originalweapon0-10
                    }
                }
            }
            return clist;
        }
        public static bool matchNumExp(object val, string valexp)
        {
            Regex rx = new Regex($@"^{valexp}$");//just use regex even though it's not great for numbers
            return rx.Match(val.ToString()).Success;
        }
        
    }

    public class MassParamEditCSV : MassParamEdit
    {
        public static string GenerateCSV(PARAM param)
        {
            string gen = "";
            foreach(PARAM.Row row in param.Rows)
            {
                string rowgen = $@"{row.ID},{row.Name}";
                foreach(PARAM.Cell cell in row.Cells)
                {
                    rowgen += $@",{cell.Value}";
                }
                gen += rowgen+"\n";
            }
            return gen;
        }
        public static MassEditResult PerformMassEdit(string csvString, ActionManager actionManager, string param)
        {
            try
            {
                PARAM p = ParamBank.Params[param];
                if(p==null)
                    return new MassEditResult(MassEditResultType.PARSEERROR, "No Param selected");
                int csvLength = p.AppliedParamdef.Fields.Count + 2;//Add ID and name
                string[] csvLines = csvString.Split('\n');
                int changeCount = 0;
                int addedCount = 0;
                List<Action> actions = new List<Action>();
                List<PARAM.Row> addedParams = new List<PARAM.Row>();
                foreach(string csvLine in csvLines)
                {
                    if(csvLine.Trim().Equals(""))
                        continue;
                    string[] csvs = csvLine.Split(',');
                    if(csvs.Length != csvLength || csvs.Length<2)
                    {
                        Console.WriteLine($@"oi oi {csvs.Length} {csvLength}");
                        return new MassEditResult(MassEditResultType.PARSEERROR, "CSV has wrong number of values");
                    }
                    int id = int.Parse(csvs[0]);
                    string name = csvs[1];
                    PARAM.Row row = p[id];
                    if(row==null)
                    {
                        row = new PARAM.Row(id, name, p.AppliedParamdef);
                        addedParams.Add(row);//gameplan is fill out row, then clone it into the real param
                    }
                    if(row.Name==null || !row.Name.Equals(name))
                        actions.Add(new PropertiesChangedAction(row.GetType().GetProperty("Name"), -1, row, name));
                    int index = 2;
                    foreach(PARAM.Cell c in row.Cells)
                    {
                        if(c.Value.GetType().IsArray)
                            continue;//don't mess with array types lol
                        string v = csvs[index];
                        object newval = performOperation(c, "=", v);
                        if(newval == null)
                            return new MassEditResult(MassEditResultType.OPERATIONERROR, $@"Could not assign {v} to field {c.Def.DisplayName}");
                        if(!c.Value.Equals(newval))
                            actions.Add(new PropertiesChangedAction(c.GetType().GetProperty("Value"), -1, c, newval));
                        index++;
                    }
                }
                changeCount = actions.Count;
                addedCount = addedParams.Count;
                actions.Add(new CloneParamsAction(p, "legacystring", addedParams, false));
                if(changeCount!=0 || addedCount!=0)
                    actionManager.ExecuteAction(new CompoundAction(actions));
                return new MassEditResult(MassEditResultType.SUCCESS, $@"{changeCount} cells affected, {addedCount} rows added");
            }
            catch(FormatException e)
            {
                return new MassEditResult(MassEditResultType.PARSEERROR, "Unable to parse CSV into correct data types");
            }
        }
    }
}