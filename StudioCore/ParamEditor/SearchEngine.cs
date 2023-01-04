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
        internal Func<A, List<B>> unpacker;
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
            //assumes unpacking doesn't fail
            string[] conditions = command.Split("&&", StringSplitOptions.TrimEntries);

            List<B> liveRows = new List<B>();
            liveRows = unpacker(param);
            List<B> originalRows = liveRows;

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
                    Func<B, bool> criteria = filter(param);
                    List<B> newRows = new List<B>();
                    foreach (B row in liveRows)
                    {
                        if (criteria(row))
                            newRows.Add(row);
                    }
                    liveRows = newRows;
                }
            }
            catch (Exception e)
            {
                liveRows = failureAllOrNone ? originalRows : new List<B>();
            }
            return liveRows;
        }
    }
    
    class ParamAndRowSearchEngine : SearchEngine<ParamEditorSelectionState, (MassEditRowSource, Param.Row)>
    {
        public static ParamAndRowSearchEngine parse = new ParamAndRowSearchEngine();
        internal override void Setup()
        {
            unpacker = (selection)=>{
                List<(MassEditRowSource, Param.Row)> list = new List<(MassEditRowSource, Param.Row)>();
                list.AddRange(selection.getSelectedRows().Select((x, i) => (MassEditRowSource.Selection, x)));
                list.AddRange(ParamBank.ClipboardRows.Select((x, i) => (MassEditRowSource.Clipboard, x)));
                return list;
            };
            filterList.Add("selection", (0, noArgs(noContext((row)=>row.Item1 == MassEditRowSource.Selection))));
            filterList.Add("clipboard", (0, noArgs(noContext((row)=>row.Item1 == MassEditRowSource.Clipboard))));
        }
    }
    enum MassEditRowSource
    {
        Selection,
        Clipboard
    }
    class ParamSearchEngine : SearchEngine<bool, (ParamBank, Param)>
    {
        public static ParamSearchEngine pse = new ParamSearchEngine(ParamBank.PrimaryBank);

        private ParamSearchEngine(ParamBank bank)
        {
            this.bank = bank;
        }
        ParamBank bank;
        internal override void Setup()
        {
            unpacker = (dummy)=>ParamBank.AuxBanks.Select((aux, i) => aux.Value.Params.Select((x, i) => (aux.Value, x.Value))).Aggregate(bank.Params.Values.Select((x, i) => (bank, x)), (o, n) => o.Concat(n)).ToList();
            filterList.Add("modified", (0, noArgs(noContext((param)=>{
                if (param.Item1 != bank)
                    return false;
                HashSet<int> cache = bank.GetVanillaDiffRows(bank.GetKeyForParam(param.Item2));
                return cache.Count > 0;
            }))));
            filterList.Add("original", (0, noArgs(noContext((param)=>{
                if (param.Item1 != bank)
                    return false;
                HashSet<int> cache = bank.GetVanillaDiffRows(bank.GetKeyForParam(param.Item2));
                return cache.Count == 0;
            }))));
            filterList.Add("param", (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((param)=>param.Item1 != bank ? false : rx.Match(bank.GetKeyForParam(param.Item2) == null ? "" : bank.GetKeyForParam(param.Item2)).Success);
            }));
            filterList.Add("auxparam", (2, (args, lenient)=>{
                ParamBank auxBank = ParamBank.AuxBanks[args[0]];
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                return noContext((param)=>param.Item1 != auxBank ? false : rx.Match(auxBank.GetKeyForParam(param.Item2) == null ? "" : auxBank.GetKeyForParam(param.Item2)).Success);
            }));
            defaultFilter = (1, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((param)=>param.Item1 != bank ? false : rx.Match(bank.GetKeyForParam(param.Item2) == null ? "" : bank.GetKeyForParam(param.Item2)).Success);
            });
        }
    }
    class RowSearchEngine : SearchEngine<(ParamBank, Param), Param.Row>
    {
        public static RowSearchEngine rse = new RowSearchEngine(ParamBank.PrimaryBank);
        private RowSearchEngine(ParamBank bank)
        {
            this.bank = bank;
        }
        ParamBank bank;
        internal override void Setup()
        {
            unpacker = (param) => new List<Param.Row>(param.Item2.Rows);
            filterList.Add("modified", (0, noArgs((context)=>{
                    string paramName = context.Item1.GetKeyForParam(context.Item2);
                    HashSet<int> cache = context.Item1.GetVanillaDiffRows(paramName);
                    return (row) => cache.Contains(row.ID);
                }
            )));
            filterList.Add("original", (0, noArgs((context)=>{
                    string paramName = context.Item1.GetKeyForParam(context.Item2);
                    HashSet<int> cache = context.Item1.GetVanillaDiffRows(paramName);
                    return (row) => !cache.Contains(row.ID);
                }
            )));
            filterList.Add("added", (0, noArgs((context)=>{
                    string paramName = context.Item1.GetKeyForParam(context.Item2);
                    if (!ParamBank.VanillaBank.Params.ContainsKey(paramName))
                        return (row) => true;
                    Param vanilParam = ParamBank.VanillaBank.Params[paramName];
                    return (row) => vanilParam[row.ID] == null;
                }
            )));
            filterList.Add("notadded", (0, noArgs((context)=>{
                    string paramName = context.Item1.GetKeyForParam(context.Item2);
                    if (!ParamBank.VanillaBank.Params.ContainsKey(paramName))
                        return (row) => false;
                    Param vanilParam = ParamBank.VanillaBank.Params[paramName];
                    return (row) => vanilParam[row.ID] != null;
                }
            )));
            filterList.Add("mergeable", (0, noArgs((context)=>{
                string paramName = context.Item1.GetKeyForParam(context.Item2);
                if (paramName == null)
                    return (row) => true;
                HashSet<int> cache = context.Item1.GetVanillaDiffRows(paramName);
                var auxCaches = ParamBank.AuxBanks.Select(x=>(x.Value.GetPrimaryDiffRows(paramName), x.Value.GetVanillaDiffRows(paramName))).ToList();
                return (row) => !cache.Contains(row.ID) && auxCaches.Where((x) => x.Item2.Contains(row.ID) && x.Item1.Contains(row.ID)).Count() > 0;
                }
            )));
            filterList.Add("conflicts", (0, noArgs((context)=>{
                string paramName = context.Item1.GetKeyForParam(context.Item2);
                HashSet<int> cache = context.Item1.GetVanillaDiffRows(paramName);
                var auxCaches = ParamBank.AuxBanks.Select(x=>(x.Value.GetPrimaryDiffRows(paramName), x.Value.GetVanillaDiffRows(paramName))).ToList();
                return (row)=>cache.Contains(row.ID) && auxCaches.Where((x) => x.Item2.Contains(row.ID) && x.Item1.Contains(row.ID)).Count() > 0;
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
                        Param.Cell? cq = row[field];
                        if (cq == null) throw new Exception();
                        Param.Cell c = cq.Value;
                        string term = c.Value.ToString();
                        if (c.Value.GetType() == typeof(byte[]))
                            term = ParamUtils.Dummy8Write((byte[])c.Value);
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
                    List<ParamRef> validFields = FieldMetaData.Get(context.Item2.AppliedParamdef.Fields.Find((f)=>f.InternalName.Equals(field))).RefTypes.FindAll((p)=>bank.Params.ContainsKey(p.param));
                    return (row)=>
                    {
                        Param.Cell? c = row[field];
                        if (c == null) throw new Exception();
                        int val = (int) c.Value.Value;
                        foreach (ParamRef rt in validFields)
                        {
                            Param.Row r = bank.Params[rt.param][val];
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
                    FMGBank.FmgEntryCategory category = FMGBank.FmgEntryCategory.None;
                    switch(context.Item1.GetKeyForParam(context.Item2))
                    {
                        case "EquipParamAccessory": category = FMGBank.FmgEntryCategory.Rings; break;
                        case "EquipParamGoods": category = FMGBank.FmgEntryCategory.Goods; break;
                        case "EquipParamWeapon": category = FMGBank.FmgEntryCategory.Weapons; break;
                        case "EquipParamProtector": category = FMGBank.FmgEntryCategory.Armor; break;
                        case "EquipParamGem": category = FMGBank.FmgEntryCategory.Gem; break;
                        case "SwordArtsParam": category = FMGBank.FmgEntryCategory.SwordArts; break;
                    }
                    if (category == FMGBank.FmgEntryCategory.None)
                        throw new Exception();
                    var fmgEntries = FMGBank.GetFmgEntriesByCategory(category, false);
                    Dictionary<int, FMG.Entry> _cache = new Dictionary<int, FMG.Entry>();
                    foreach (var fmgEntry in fmgEntries)
                    {
                        _cache[fmgEntry.ID] = fmgEntry;
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

    class CellSearchEngine : SearchEngine<Param.Row, (PseudoColumn, Param.Column)>
    {
        public static CellSearchEngine cse = new CellSearchEngine();
        internal override void Setup()
        {
            unpacker = (row) => {
                var list = new List<(PseudoColumn, Param.Column)>();
                list.Add((PseudoColumn.ID, null));
                list.Add((PseudoColumn.Name, null));
                list.AddRange(row.Cells.Select((cell, i) => (PseudoColumn.None, cell)));
                return list;
            };
            defaultFilter = (1, (args, lenient) => {
                bool matchID = args[0] == "ID";
                bool matchName = args[0] == "Name";
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((cell) => (matchID && cell.Item1 == PseudoColumn.ID) || (matchName && cell.Item1 == PseudoColumn.Name) || (cell.Item2 != null && rx.Match(cell.Item2.Def.InternalName).Success));
            });
        }
    }
}