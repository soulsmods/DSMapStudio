using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;
using FSParam;
using ImGuiNET;
using SoulsFormats;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;

namespace StudioCore.Editor
{
    /* Restricted characters: colon, space, forward slash
     *
     */
    class SearchEngine<A,B>
    {
        public SearchEngine()
        {
            Setup();
        }

        internal Dictionary<string, (int, Func<string[], bool, Func<A, Func<B, bool>>>)> filterList = new Dictionary<string, (int, Func<string[], bool, Func<A, Func<B, bool>>>)>();
        internal (int, Func<string[], bool, Func<A, Func<B, bool>>>) defaultFilter;
        internal Func<A, IReadOnlyList<B>> unpacker;
        protected void addExistsFilter() {
            filterList.Add("exists", (0, noArgs(noContext((B)=>true))));
        }
        protected Func<string[], bool, Func<A, Func<B, bool>>> noArgs(Func<A, Func<B, bool>> func)
        {
            return (args, lenient)=>func;
        }
        protected Func<A, Func<B, bool>> noContext(Func<B, bool> func)
        {
            return (context)=>func;
        }

        internal virtual void Setup(){
        }

        public bool HandlesCommand(string command)
        {
            return filterList.ContainsKey(command.Split(" ")[0]);
        }
        public List<string> AvailableCommands()
        {
            List<string> options = new List<string>();
            foreach (string op in filterList.Keys)
            {
                options.Add(op+"("+(filterList[op].Item1)+" args)");
            }
            if (defaultFilter!=(0, null))
                options.Add("or omit specifying and use default ("+defaultFilter.Item1+"args)");
            return options;
        }

        public List<B> Search(A param, string command, bool lenient, bool failureAllOrNone)
        {
            return Search(new List<A>(new A[]{param}), command, lenient, failureAllOrNone);
        }
        public List<B> Search(List<A> param, string command, bool lenient, bool failureAllOrNone)
        {
            //assumes unpacking doesn't fail
            string[] conditions = command.Split("&&", StringSplitOptions.TrimEntries);

            List<(A, IReadOnlyList<B>)> liveRows = new List<(A, IReadOnlyList<B>)>();
            List<(A, IReadOnlyList<B>)> originalRows = liveRows;
            foreach (A p in param)
            {
                liveRows.Add((p, unpacker(p)));
            }

            try {
                foreach (string condition in conditions)
                {
                    //temp
                    if (condition.Equals(""))
                        break;
                    string[] cmd = condition.Split(' ', 2);

                    int argC;
                    Func<string[], bool, Func<A, Func<B, bool>>> method;
                    string[] args;
                    if (filterList.ContainsKey(cmd[0]))
                    {
                        (argC, method) = filterList[cmd[0]];
                        args = cmd.Length==1?new string[0] : cmd[1].Split(' ', argC, StringSplitOptions.TrimEntries);
                    }
                    else
                    {
                        (argC, method) = defaultFilter;
                        args = condition.Split(" ", argC, StringSplitOptions.TrimEntries);
                    }
                    var filter = method(args, lenient);
                    List<(A, IReadOnlyList<B>)> rows = new List<(A, IReadOnlyList<B>)>();
                    foreach ((A p, IReadOnlyList<B> live) in liveRows)
                    {
                        Func<B, bool> criteria = filter(p);
                        List<B> newRows = new List<B>();
                        foreach (B row in live)
                        {
                            if (criteria(row))
                                newRows.Add(row);
                        }
                        rows.Add((p, newRows));
                    }
                    liveRows = rows;
                }
            }
            catch (Exception e)
            {
                liveRows = failureAllOrNone ? originalRows : new List<(A, IReadOnlyList<B>)>();
            }
            //assumes serialising doesn't fail
            List<B> finalRows = new List<B>();
            foreach ((A p, IReadOnlyList<B> l) in liveRows)
            {
                finalRows.AddRange(l);
            }
            return finalRows;
        }
    }
    
    class ParamAndRowSearchEngine : SearchEngine<ParamEditorSelectionState, Param.Row>
    {
        public static ParamAndRowSearchEngine parse = new ParamAndRowSearchEngine();
        internal override void Setup()
        {
            unpacker = (selection)=>selection.getSelectedRows();
            filterList.Add("selection", (0, noArgs(noContext((row)=>true))));
        }
    }
    class ParamSearchEngine : SearchEngine<bool, Param>
    {
        public static ParamSearchEngine pse = new ParamSearchEngine();
        internal override void Setup()
        {
            unpacker = (dummy)=>new List<FSParam.Param>(ParamBank.Params.Values);
            filterList.Add("modified", (0, noArgs(noContext((param)=>{
                    HashSet<int> cache = ParamBank.DirtyParamCache[ParamBank.GetKeyForParam(param)];
                    return cache.Count>0;
                }
            ))));
            filterList.Add("original", (0, noArgs(noContext((param)=>{
                    HashSet<int> cache = ParamBank.DirtyParamCache[ParamBank.GetKeyForParam(param)];
                    return cache.Count==0;
                }
            ))));
            filterList.Add("param", (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((param)=>rx.Match(ParamBank.GetKeyForParam(param) == null ? "" : ParamBank.GetKeyForParam(param)).Success);
            }));
            defaultFilter = (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((param)=>rx.Match(ParamBank.GetKeyForParam(param) == null ? "" : ParamBank.GetKeyForParam(param)).Success);
            });
        }
    }
    class RowSearchEngine : SearchEngine<Param, Param.Row>
    {
        public static RowSearchEngine rse = new RowSearchEngine();
        internal override void Setup()
        {
            unpacker = (param) => param.Rows;
            filterList.Add("modified", (0, noArgs((context)=>{
                    string paramName = ParamBank.GetKeyForParam(context);
                    HashSet<int> cache = ParamBank.DirtyParamCache[paramName];
                    return (row)=>cache.Contains(row.ID);
                }
            )));
            filterList.Add("original", (0, noArgs((context)=>{
                    string paramName = ParamBank.GetKeyForParam(context);
                    HashSet<int> cache = ParamBank.DirtyParamCache[paramName];
                    return (row)=>!cache.Contains(row.ID);
                }
            )));
            filterList.Add("id", (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0].ToLower()) : new Regex($@"^{args[0]}$");
                return noContext((row)=>rx.Match(row.ID.ToString()).Success);
            }));
            filterList.Add("name", (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((row)=>rx.Match(row.Name == null ? "" : row.Name).Success);
            }));
            filterList.Add("prop", (2, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                string field = args[0].Replace(@"\s", " ");
                return noContext((row)=>{
                        Param.Cell? c = row[field];
                        if (c == null) throw new Exception();
                        string term = c.Value.Value.ToString();
                        return rx.Match(term).Success;
                });
            }));
            filterList.Add("idrange", (2, (args, lenient)=>{
                double floor = double.Parse(args[0]);
                double ceil = double.Parse(args[1]);
                return noContext((row)=>row.ID >= floor && row.ID <= ceil);
            }));
            filterList.Add("proprange", (3, (args, lenient)=>{
                string field = args[0].Replace(@"\s", " ");
                double floor = double.Parse(args[1]);
                double ceil = double.Parse(args[2]);
                return noContext((row)=>
                {
                        Param.Cell? c = row[field];
                        if (c == null) throw new Exception();
                        return (Convert.ToDouble(c.Value.Value)) >= floor && (Convert.ToDouble(c.Value.Value)) <= ceil;
                });
            }));
            filterList.Add("propref", (2, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                string field = args[0].Replace(@"\s", " ");
                return (context)=>{
                    List<string> validFields = FieldMetaData.Get(context.AppliedParamdef.Fields.Find((f)=>f.InternalName.Equals(field))).RefTypes.FindAll((p)=>ParamBank.Params.ContainsKey(p));
                    return (row)=>
                    {
                        Param.Cell? c = row[field];
                        if (c == null) throw new Exception();
                        int val = (int) c.Value.Value;
                        foreach (string rt in validFields)
                        {
                            Param.Row r = ParamBank.Params[rt][val];
                            if (r != null && rx.Match(r.Name ?? "").Success)
                                return true;
                        }
                        return false;
                    };
                };
            }));
            filterList.Add("fmg", (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                string field = args[0].Replace(@"\s", " ");
                return (context)=>{
                    FMGBank.ItemCategory category = FMGBank.ItemCategory.None;
                    switch(ParamBank.GetKeyForParam(context))
                    {
                        case "EquipParamAccessory": category = FMGBank.ItemCategory.Rings; break;
                        case "EquipParamGoods": category = FMGBank.ItemCategory.Goods; break;
                        case "EquipParamWeapon": category = FMGBank.ItemCategory.Weapons; break;
                        case "EquipParamProtector": category = FMGBank.ItemCategory.Armor; break;
                        case "EquipParamGem": category = FMGBank.ItemCategory.Gem; break;
                        case "SwordArtsParam": category = FMGBank.ItemCategory.SwordArts; break;
                    }
                    if (category == FMGBank.ItemCategory.None)
                        throw new Exception();
                    var fmgEntries = FMGBank.GetItemFMGEntriesByType(category, FMGBank.ItemType.Title, false);
                    Dictionary<int, FMG.Entry> _cache = new Dictionary<int, FMG.Entry>();
                    foreach (var fmgEntry in fmgEntries)
                    {
                        _cache.Add(fmgEntry.ID, fmgEntry);
                    }
                    return (row)=>{
                        if (!_cache.ContainsKey(row.ID))
                            return false;
                        FMG.Entry e = _cache[row.ID];
                        return e != null && rx.Match(e.Text ?? "").Success;
                    };
                };
            }));
            defaultFilter = (1, (args, lenient)=>{
                if (!lenient)
                    return noContext((row)=>false);
                Regex rx = new Regex(args[0], RegexOptions.IgnoreCase);
                return noContext((row)=>rx.Match(row.Name ?? "").Success || rx.Match(row.ID.ToString()).Success);
            });
        }
    }

    class CellSearchEngine : SearchEngine<Param.Row, Param.Column>
    {
        public static CellSearchEngine cse = new CellSearchEngine();
        internal override void Setup()
        {
            unpacker = (row)=>new List<Param.Column>(row.Cells);
            defaultFilter = (1, (args, lenient) => {
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((cell)=>rx.Match(cell.Def.InternalName).Success);
            });
        }
    }
}