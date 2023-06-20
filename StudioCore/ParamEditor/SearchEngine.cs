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
    /* Restricted characters: colon, space, forward slash, ampersand, exclamation mark
     *
     */
    class SearchEngine<A,B>
    {
        public SearchEngine()
        {
            Setup();
        }

        internal Dictionary<string, SearchEngineCommand<A,B>> filterList = new Dictionary<string, SearchEngineCommand<A,B>>();
        internal SearchEngineCommand<A,B> defaultFilter = null;
        internal Func<A, List<B>> unpacker;
        protected void addExistsFilter() {
            filterList.Add("exists", newCmd(new string[0], noArgs(noContext((B)=>true))));
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
        internal SearchEngineCommand<A, B> newCmd(string[] args, Func<string[], bool, Func<A, Func<B, bool>>> func, Func<bool> shouldShow = null)
        {
            return new SearchEngineCommand<A, B>(args, func, shouldShow);
        }

        public bool HandlesCommand(string command)
        {
            if (command.Length > 0 && command.StartsWith('!'))
                command = command.Substring(1);
            return filterList.ContainsKey(command.Split(" ")[0]);
        }
        public List<string> AvailableCommandsForHelpText()
        {
            List<string> options = new List<string>();
            foreach (string op in filterList.Keys)
            {
                SearchEngineCommand<A,B> cmd = filterList[op];
                if (cmd.shouldShow == null || cmd.shouldShow())
                    options.Add(op+"("+(filterList[op].args.Length)+" args)");
            }
            if (defaultFilter != null && (defaultFilter.shouldShow == null || defaultFilter.shouldShow()))
                options.Add("or omit specifying and use default ("+defaultFilter.args.Length+"args)");
            return options;
        }
        public List<(string, string[])> VisibleCommands()
        {
            List<(string, string[])> options = new List<(string, string[])>();
            foreach (string op in filterList.Keys)
            {
                SearchEngineCommand<A,B> cmd = filterList[op];
                if (cmd.shouldShow == null || cmd.shouldShow())
                    options.Add((op, cmd.args));
            }
            return options;
        }
        public List<(string, string[])> AllCommands()
        {
            List<(string, string[])> options = new List<(string, string[])>();
            foreach (string op in filterList.Keys)
            {
                options.Add((op, filterList[op].args));
            }
            if (defaultFilter != null)
                options.Add(("", defaultFilter.args));
            return options;
        }

        public List<B> Search(A param, string command, bool lenient, bool failureAllOrNone)
        {
            return Search(param, unpacker(param), command, lenient, failureAllOrNone);
        }

        public virtual List<B> Search(A context, List<B> sourceSet, string command, bool lenient, bool failureAllOrNone)
        {
            //assumes unpacking doesn't fail
            string[] conditions = command.Split("&&", StringSplitOptions.TrimEntries);
            List<B> liveSet = sourceSet;

            try {
                foreach (string condition in conditions)
                {
                    //temp
                    if (condition.Equals(""))
                        break;
                    string[] cmd = condition.Split(' ', 2);

                    SearchEngineCommand<A,B> selectedCommand;
                    int argC;
                    string[] args;
                    bool not = false;
                    if (cmd[0].Length > 0 && cmd[0].StartsWith('!'))
                    {
                        cmd[0] = cmd[0].Substring(1);
                        not = true;
                    }
                    if (filterList.ContainsKey(cmd[0]))
                    {
                        selectedCommand = filterList[cmd[0]];
                        argC = selectedCommand.args.Length;
                        args = cmd.Length==1?new string[0] : cmd[1].Split(' ', argC, StringSplitOptions.TrimEntries);
                    }
                    else
                    {
                        selectedCommand = defaultFilter;
                        argC = selectedCommand.args.Length;
                        args = condition.Split(" ", argC, StringSplitOptions.TrimEntries);
                    }
                    for (int i=0; i<argC; i++)
                    {
                        if (args[i].StartsWith('$'))
                            args[i] = MassParamEdit.massEditVars[args[i].Substring(1)].ToString();
                    }

                    var filter = selectedCommand.func(args, lenient);
                    Func<B, bool> criteria = filter(context);
                    List<B> newRows = new List<B>();
                    foreach (B row in liveSet)
                    {
                        if (not ^ criteria(row))
                            newRows.Add(row);
                    }
                    liveSet = newRows;
                }
            }
            catch (Exception e)
            {
                liveSet = failureAllOrNone ? sourceSet : new List<B>();
            }
            return liveSet;
        }
    }
    class SearchEngineCommand<A, B>
    {
        public string[] args;
        internal Func<bool> shouldShow;
        internal Func<string[], bool, Func<A, Func<B, bool>>> func;
        internal SearchEngineCommand(string[] args, Func<string[], bool, Func<A, Func<B, bool>>> func, Func<bool> shouldShow)
        {
            this.args = args;
            this.func = func;
            this.shouldShow = shouldShow;
        }
    }

    /*
     *  Handles conversion to a secondary searchengine which handles && conditions and conversion back to the anticipated type
     */
    class MultiStageSearchEngine<A,B, C,D> : SearchEngine<A,B>
    {
        internal Func<A, B, C> contextGetterForMultiStage = null;
        internal Func<B, D> sourceListGetterForMultiStage = null;
        internal SearchEngine<C, D> searchEngineForMultiStage = null;
        internal Func<D, B, B> resultRetrieverForMultiStage = null;

        public override List<B> Search(A context, List<B> sourceSet, string command, bool lenient, bool failureAllOrNone)
        {
            string[] conditions = command.Split("&&", 2, StringSplitOptions.TrimEntries);
            List<B> stage1list = base.Search(context, sourceSet, conditions[0], lenient, failureAllOrNone);
            if (conditions.Length == 1)
                return stage1list;
            B exampleItem = stage1list.FirstOrDefault();
            List<D> stage2list = searchEngineForMultiStage.Search(contextGetterForMultiStage(context, exampleItem), stage1list.Select((x) => sourceListGetterForMultiStage(x)).ToList(), conditions[1], lenient, failureAllOrNone);
            return stage2list.Select((x) => resultRetrieverForMultiStage(x, exampleItem)).ToList();
        }
    }

    class ParamAndRowSearchEngine : MultiStageSearchEngine<ParamEditorSelectionState, (MassEditRowSource, Param.Row), (ParamBank, Param), Param.Row>
    {
        public static SearchEngine<ParamEditorSelectionState, (MassEditRowSource, Param.Row)> parse = new ParamAndRowSearchEngine();
        internal override void Setup()
        {
            unpacker = (selection)=>{
                List<(MassEditRowSource, Param.Row)> list = new List<(MassEditRowSource, Param.Row)>();
                list.AddRange(selection.getSelectedRows().Select((x, i) => (MassEditRowSource.Selection, x)));
                list.AddRange(ParamBank.ClipboardRows.Select((x, i) => (MassEditRowSource.Clipboard, x)));
                return list;
            };
            filterList.Add("selection", newCmd(new string[0], noArgs(noContext((row)=>row.Item1 == MassEditRowSource.Selection))));
            filterList.Add("clipboard", newCmd(new string[0], noArgs(noContext((row)=>row.Item1 == MassEditRowSource.Clipboard)), ()=>ParamBank.ClipboardRows?.Count > 0));
            contextGetterForMultiStage = (ParamEditorSelectionState state, (MassEditRowSource, Param.Row) exampleItem) => (ParamBank.PrimaryBank, ParamBank.PrimaryBank.Params[exampleItem.Item1 == MassEditRowSource.Selection ? state.getActiveParam() : ParamBank.ClipboardParam]);
            sourceListGetterForMultiStage = ((MassEditRowSource, Param.Row) row) => row.Item2;
            searchEngineForMultiStage = RowSearchEngine.rse;
            resultRetrieverForMultiStage = (Param.Row row, (MassEditRowSource, Param.Row) exampleItem) => (exampleItem.Item1, row);
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
            filterList.Add("modified", newCmd(new string[0], noArgs(noContext((param)=>{
                if (param.Item1 != bank)
                    return false;
                HashSet<int> cache = bank.GetVanillaDiffRows(bank.GetKeyForParam(param.Item2));
                return cache.Count > 0;
            }))));
            filterList.Add("param", newCmd(new string[]{"param name (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((param)=>param.Item1 != bank ? false : rx.IsMatch(bank.GetKeyForParam(param.Item2) == null ? "" : bank.GetKeyForParam(param.Item2)));
            }));
            filterList.Add("auxparam", newCmd(new string[]{"parambank name", "param name (regex)"}, (args, lenient)=>{
                ParamBank auxBank = ParamBank.AuxBanks[args[0]];
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                return noContext((param)=>param.Item1 != auxBank ? false : rx.IsMatch(auxBank.GetKeyForParam(param.Item2) == null ? "" : auxBank.GetKeyForParam(param.Item2)));
            }, ()=>ParamBank.AuxBanks.Count > 0 && CFG.Current.Param_AdvancedMassedit));
            defaultFilter = newCmd(new string[]{"param name (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((param)=>param.Item1 != bank ? false : rx.IsMatch(bank.GetKeyForParam(param.Item2) == null ? "" : bank.GetKeyForParam(param.Item2)));
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
            filterList.Add("modified", newCmd(new string[0], noArgs((context)=>{
                    string paramName = context.Item1.GetKeyForParam(context.Item2);
                    HashSet<int> cache = context.Item1.GetVanillaDiffRows(paramName);
                    return (row) => cache.Contains(row.ID);
                }
            )));
            filterList.Add("added", newCmd(new string[0], noArgs((context)=>{
                    string paramName = context.Item1.GetKeyForParam(context.Item2);
                    if (!ParamBank.VanillaBank.Params.ContainsKey(paramName))
                        return (row) => true;
                    Param vanilParam = ParamBank.VanillaBank.Params[paramName];
                    return (row) => vanilParam[row.ID] == null;
                }
            )));
            filterList.Add("mergeable", newCmd(new string[0], noArgs((context)=>{
                string paramName = context.Item1.GetKeyForParam(context.Item2);
                if (paramName == null)
                    return (row) => true;
                HashSet<int> pCache = ParamBank.PrimaryBank.GetVanillaDiffRows(paramName);
                var auxCaches = ParamBank.AuxBanks.Select(x=>(x.Value.GetPrimaryDiffRows(paramName), x.Value.GetVanillaDiffRows(paramName))).ToList();
                return (row) => !pCache.Contains(row.ID) && auxCaches.Where((x) => x.Item2.Contains(row.ID) && x.Item1.Contains(row.ID)).Count() == 1;
                }
            ), ()=>ParamBank.AuxBanks.Count > 0));
            filterList.Add("conflicts", newCmd(new string[0], noArgs((context)=>{
                string paramName = context.Item1.GetKeyForParam(context.Item2);
                HashSet<int> pCache = ParamBank.PrimaryBank.GetVanillaDiffRows(paramName);
                var auxCaches = ParamBank.AuxBanks.Select(x=>(x.Value.GetPrimaryDiffRows(paramName), x.Value.GetVanillaDiffRows(paramName))).ToList();
                return (row) => (pCache.Contains(row.ID)?1:0) + auxCaches.Where((x) => x.Item2.Contains(row.ID) && x.Item1.Contains(row.ID)).Count() > 1;
                }
            ), ()=>ParamBank.AuxBanks.Count > 0));
            filterList.Add("id", newCmd(new string[]{"row id (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0].ToLower()) : new Regex($@"^{args[0]}$");
                return noContext((row)=>rx.IsMatch(row.ID.ToString()));
            }));
            filterList.Add("idrange", newCmd(new string[]{"row id minimum (inclusive)", "row id maximum (inclusive)"}, (args, lenient)=>{
                double floor = double.Parse(args[0]);
                double ceil = double.Parse(args[1]);
                return noContext((row)=>row.ID >= floor && row.ID <= ceil);
            }));
            filterList.Add("name", newCmd(new string[]{"row name (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((row)=>rx.IsMatch(row.Name == null ? "" : row.Name));
            }));
            filterList.Add("prop", newCmd(new string[]{"field internalName", "field value (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                string field = args[0].Replace(@"\s", " ");
                return noContext((row)=>{
                        Param.Cell? cq = row[field];
                        if (cq == null) throw new Exception();
                        Param.Cell c = cq.Value;
                        string term = c.Value.ToParamEditorString();
                        return rx.IsMatch(term);
                });
            }));
            filterList.Add("proprange", newCmd(new string[]{"field internalName", "field value minimum (inclusive)", "field value maximum (inclusive)"}, (args, lenient)=>{
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
            filterList.Add("propref", newCmd(new string[]{"field internalName", "referenced row name (regex)"}, (args, lenient)=>{
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
                            if (r != null && rx.IsMatch(r.Name ?? ""))
                                return true;
                        }
                        return false;
                    };
                };
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            filterList.Add("fmg", newCmd(new string[]{"fmg title (regex)"}, (args, lenient)=>{
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
                        return e != null && rx.IsMatch(e.Text ?? "");
                    };
                };
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            filterList.Add("vanillaprop", newCmd(new string[]{"field internalName", "field value (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                string field = args[0].Replace(@"\s", " ");
                return (param) => {
                    Param vparam = ParamBank.VanillaBank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return (row)=>{
                        Param.Row vrow = vparam[row.ID];
                        if (vrow == null)
                            return false;
                        Param.Cell? cq = vrow[field];
                        if (cq == null) throw new Exception();
                        Param.Cell c = cq.Value;
                        string term = c.Value.ToParamEditorString();
                        return rx.IsMatch(term);
                    };
                };
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            filterList.Add("vanillaproprange", newCmd(new string[]{"field internalName", "field value minimum (inclusive)", "field value maximum (inclusive)"}, (args, lenient)=>{
                string field = args[0].Replace(@"\s", " ");
                double floor = double.Parse(args[1]);
                double ceil = double.Parse(args[2]);
                return (param) => {
                    Param vparam = ParamBank.VanillaBank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return (row)=>{
                        Param.Row vrow = vparam[row.ID];
                        if (vrow == null)
                            return false;
                        Param.Cell? c = vrow[field];
                        if (c == null) throw new Exception();
                        return (Convert.ToDouble(c.Value.Value)) >= floor && (Convert.ToDouble(c.Value.Value)) <= ceil;
                    };
                };
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            filterList.Add("auxprop", newCmd(new string[]{"parambank name", "field internalName", "field value (regex)"}, (args, lenient)=>{
                Regex rx = lenient ? new Regex(args[2], RegexOptions.IgnoreCase) : new Regex($@"^{args[2]}$");
                string field = args[1].Replace(@"\s", " ");
                ParamBank bank;
                if (!ParamBank.AuxBanks.TryGetValue(args[0], out bank))
                    throw new Exception("Unable to find auxbank "+args[0]);
                return (param) => {
                    Param vparam = bank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return (row)=>{
                        Param.Row vrow = vparam[row.ID];
                        if (vrow == null)
                            return false;
                        Param.Cell? cq = vrow[field];
                        if (cq == null) throw new Exception();
                        Param.Cell c = cq.Value;
                        string term = c.Value.ToParamEditorString();
                        return rx.IsMatch(term);
                    };
                };
            }, ()=>ParamBank.AuxBanks.Count > 0 && CFG.Current.Param_AdvancedMassedit));
            filterList.Add("auxproprange", newCmd(new string[]{"parambank name", "field internalName", "field value minimum (inclusive)", "field value maximum (inclusive)"}, (args, lenient)=>{
                string field = args[0].Replace(@"\s", " ");
                double floor = double.Parse(args[1]);
                double ceil = double.Parse(args[2]);
                ParamBank bank;
                if (!ParamBank.AuxBanks.TryGetValue(args[0], out bank))
                    throw new Exception("Unable to find auxbank "+args[0]);
                return (param) => {
                    Param vparam = bank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return (row)=>{
                        Param.Row vrow = vparam[row.ID];
                        Param.Cell? c = vrow[field];
                        if (c == null) throw new Exception();
                        return (Convert.ToDouble(c.Value.Value)) >= floor && (Convert.ToDouble(c.Value.Value)) <= ceil;
                    };
                };
            }, ()=>ParamBank.AuxBanks.Count > 0 && CFG.Current.Param_AdvancedMassedit));
            
            filterList.Add("semijoin", newCmd(new string[]{"this field internalName", "other param", "other param field internalName", "other param row search"}, (args, lenient)=>{
                string thisField = args[0].Replace(@"\s", " ");
                string otherParam = args[1];
                string otherField = args[2].Replace(@"\s", " ");
                string otherSearchTerm = args[3];
                Param otherParamReal;
                if (!ParamBank.PrimaryBank.Params.TryGetValue(otherParam, out otherParamReal))
                    throw new Exception("Could not find param "+otherParam);
                List<Param.Row> rows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, otherParamReal), otherSearchTerm, lenient, false);
                (PseudoColumn, Param.Column) otherFieldReal = ParamUtils.GetCol(otherParamReal, otherField);
                if (!otherFieldReal.IsColumnValid())
                    throw new Exception("Could not find field "+otherField);
                HashSet<string> possibleValues = rows.Select((x) => x.Get(otherFieldReal).ToParamEditorString()).Distinct().ToHashSet();
                return (param) => {
                    (PseudoColumn, Param.Column) thisFieldReal = ParamUtils.GetCol(param.Item2, thisField);
                    if (!thisFieldReal.IsColumnValid())
                        throw new Exception("Could not find field "+thisField);
                    return (row)=>{
                        string toFind = row.Get(thisFieldReal).ToParamEditorString();
                        return possibleValues.Contains(toFind);
                    };
                };
            }, ()=>CFG.Current.Param_AdvancedMassedit));
            defaultFilter = newCmd(new string[]{"row ID or Name (regex)"}, (args, lenient)=>{
                if (!lenient)
                    return noContext((row)=>false);
                Regex rx = new Regex(args[0], RegexOptions.IgnoreCase);
                return noContext((row)=>rx.IsMatch(row.Name ?? "") || rx.IsMatch(row.ID.ToString()));
            });
        }
    }

    class CellSearchEngine : SearchEngine<(string, Param.Row), (PseudoColumn, Param.Column)>
    {
        public static CellSearchEngine cse = new CellSearchEngine();
        internal override void Setup()
        {
            unpacker = (row) => {
                var list = new List<(PseudoColumn, Param.Column)>();
                list.Add((PseudoColumn.ID, null));
                list.Add((PseudoColumn.Name, null));
                list.AddRange(row.Item2.Cells.Select((cell, i) => (PseudoColumn.None, cell)));
                return list;
            };
            defaultFilter = newCmd(new string[]{"field internalName (regex)"}, (args, lenient) => {
                bool matchID = args[0] == "ID";
                bool matchName = args[0] == "Name";
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((cell) => {
                    if (matchID && cell.Item1 == PseudoColumn.ID)
                        return true;
                    if (matchName && cell.Item1 == PseudoColumn.Name)
                        return true;
                    if (cell.Item2 != null)
                    {
                        var meta = lenient ? FieldMetaData.Get(cell.Item2.Def) : null;
                        if (lenient && meta?.AltName != null && rx.IsMatch(meta?.AltName))
                            return true;
                        if (rx.IsMatch(cell.Item2.Def.InternalName))
                            return true;
                    }
                    return false;
                });
            });
            filterList.Add("modified", newCmd(new string[0], (args, lenient) => (row) => {
                if (row.Item1 == null)
                    throw new Exception("Can't check if cell is modified - not part of a param");
                Param vParam = ParamBank.VanillaBank.Params?[row.Item1];
                if (vParam == null)
                    throw new Exception("Can't check if cell is modified - no vanilla param");
                Param.Row r = vParam[row.Item2.ID];
                if (r == null)
                    return (col) => true;
                else
                    return (col) => {
                        (PseudoColumn, Param.Column) vcol = col.GetAs(vParam);
                        object valA = row.Item2.Get(col);
                        object valB = r.Get(vcol);
                        return ParamUtils.IsValueDiff(ref valA, ref valB, col.GetColumnType());
                    };
            }));
            filterList.Add("auxmodified", newCmd(new string[]{"parambank name"}, (args, lenient) => {
                if (!ParamBank.AuxBanks.ContainsKey(args[0]))
                    throw new Exception("Can't check if cell is modified - parambank not found");
                ParamBank bank = ParamBank.AuxBanks[args[0]];
                return (row) => {
                    if (row.Item1 == null)
                        throw new Exception("Can't check if cell is modified - not part of a param");
                    Param auxParam = bank.Params?[row.Item1];
                    if (auxParam == null)
                        throw new Exception("Can't check if cell is modified - no aux param");
                    Param vParam = ParamBank.VanillaBank.Params?[row.Item1];
                    if (vParam == null)
                        throw new Exception("Can't check if cell is modified - no vanilla param");
                    Param.Row r = auxParam[row.Item2.ID];
                    Param.Row r2 = vParam[row.Item2.ID];
                    if (r == null)
                        return (col) => false;
                    else if (r2 == null)
                        return (col) => true;
                    else
                        return (col) => {
                            (PseudoColumn, Param.Column) auxcol = col.GetAs(auxParam);
                            (PseudoColumn, Param.Column) vcol = col.GetAs(vParam);
                            object valA = r.Get(auxcol);
                            object valB = r2.Get(vcol);
                            return ParamUtils.IsValueDiff(ref valA, ref valB, col.GetColumnType());
                        };
                };
            }, ()=>ParamBank.AuxBanks.Count > 0));
            filterList.Add("sftype", newCmd(new string[]{"paramdef type"}, (args, lenient) => {
                Regex r = new Regex('^'+args[0]+'$', lenient ? RegexOptions.IgnoreCase : RegexOptions.None); //Leniency rules break from the norm
                return (row) => (col) => r.IsMatch(col.GetColumnSfType());
            }, ()=>CFG.Current.Param_AdvancedMassedit));
        }
    }

    

    class VarSearchEngine : SearchEngine<bool, string>
    {
        public static VarSearchEngine vse = new VarSearchEngine();
        internal override void Setup()
        {
            unpacker = (dummy) => {
                return MassParamEdit.massEditVars.Keys.ToList();
            };
            filterList.Add("vars", newCmd(new string[]{"variable names (regex)"}, (args, lenient) => {
                if (args[0].StartsWith('$'))
                    args[0] = args[0].Substring(1);
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext((name) => rx.IsMatch(name));
            }));
            
        }
    }
}