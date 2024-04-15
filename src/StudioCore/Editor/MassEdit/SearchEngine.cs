using Andre.Formats;
using Microsoft.Toolkit.HighPerformance;
using Org.BouncyCastle.Tls;
using SoulsFormats;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace StudioCore.Editor.MassEdit;

/* Restricted characters: colon, space, forward slash, ampersand, exclamation mark
 *
 */
internal abstract class TypelessSearchEngine
{
    private static Dictionary<Type, List<(TypelessSearchEngine, Type)>> searchEngines = new();
    internal static void AddSearchEngine<TContextObject, TContextField, TElementObject, TElementField>(SearchEngine<TContextObject, TContextField, TElementObject, TElementField> engine)
    {
        if (!searchEngines.ContainsKey(typeof((TContextObject, TContextField))))
            searchEngines.Add(typeof((TContextObject, TContextField)), new());
        searchEngines[typeof((TContextObject, TContextField))].Add((engine, typeof((TElementObject, TElementField))));
    }
    internal static List<(TypelessSearchEngine, Type)> GetSearchEngines(Type t) //Type t is expected to be (TContextObject, TContextField)
    {
        return searchEngines.GetValueOrDefault(t) ?? ([]);
    }
    internal abstract List<(string, string[], string)> VisibleCommands(bool includeDefault);
    internal abstract List<(string, string[])> AllCommands();
    internal abstract List<string> AvailableCommandsForHelpText();
    internal abstract List<(TypelessSearchEngine, Type)> NextSearchEngines(); //Type t is expected to be (TContextObject, TContextField)
    internal abstract METypelessOperation NextOperation();
    internal abstract string NameForHelpTexts();
    internal abstract Type getContainerType();
    internal abstract Type getElementType();
    public abstract IEnumerable<(object, object)> SearchNoType((object, object) container, string command, bool lenient, bool failureAllOrNone);
    internal abstract bool HandlesCommand(string command);
}
internal class SearchEngine<TContextObject, TContextField, TElementObject, TElementField> : TypelessSearchEngine
{
    internal SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)> defaultFilter;

    internal Dictionary<string, SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)>> filterList = new();
    internal Func<(TContextObject, TContextField), List<(TElementObject, TElementField)>> unpacker;
    internal string name = "[unnamed search engine]";

    internal SearchEngine()
    {
        Setup();
        AddSearchEngine(this);
    }

    protected void addExistsFilter()
    {
        filterList.Add("exists", newCmd([], "Selects all elements", noArgs(noContext(b => true))));
    }

    protected Func<string[], bool, Func<(TContextObject, TContextField), Func<(TElementObject, TElementField), bool>>> noArgs(Func<(TContextObject, TContextField), Func<(TElementObject, TElementField), bool>> func)
    {
        return (args, lenient) => func;
    }

    protected Func<(TContextObject, TContextField), Func<(TElementObject, TElementField), bool>> noContext(Func<(TElementObject, TElementField), bool> func)
    {
        return context => func;
    }

    internal virtual void Setup()
    {
    }

    internal SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)> newCmd(string[] args, string wiki,
        Func<string[], bool, Func<(TContextObject, TContextField), Func<(TElementObject, TElementField), bool>>> func, Func<bool> shouldShow = null)
    {
        return new SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)>(args, wiki, func, shouldShow);
    }

    internal override bool HandlesCommand(string command)
    {
        if (command.Length > 0 && command.StartsWith('!'))
        {
            command = command.Substring(1);
        }

        return filterList.ContainsKey(command.Split(" ")[0]);
    }

    internal override List<string> AvailableCommandsForHelpText()
    {
        List<string> options = new();
        foreach (var op in filterList.Keys)
        {
            SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)> cmd = filterList[op];
            if (cmd.shouldShow == null || cmd.shouldShow())
            {
                options.Add(op + "(" + filterList[op].args.Length + " args)");
            }
        }

        if (defaultFilter != null && (defaultFilter.shouldShow == null || defaultFilter.shouldShow()))
        {
            options.Add("or omit specifying and use default (" + defaultFilter.args.Length + "args)");
        }

        return options;
    }

    internal override List<(string, string[], string)> VisibleCommands(bool includeDefault)
    {
        List<(string, string[], string)> options = new();
        foreach (var op in filterList.Keys)
        {
            SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)> cmd = filterList[op];
            if (cmd.shouldShow == null || cmd.shouldShow())
            {
                options.Add((op, cmd.args, cmd.wiki));
            }
        }
        if (includeDefault)
            options.Add((null, defaultFilter.args, defaultFilter.wiki));

        return options;
    }

    internal override List<(string, string[])> AllCommands()
    {
        List<(string, string[])> options = new();
        foreach (var op in filterList.Keys)
        {
            options.Add((op, filterList[op].args));
        }

        if (defaultFilter != null)
        {
            options.Add(("", defaultFilter.args));
        }

        return options;
    }
    public override IEnumerable<(object, object)> SearchNoType((object, object) container, string command, bool lenient, bool failureAllOrNone)
    {
        List<(TElementObject, TElementField)> res = Search(((TContextObject, TContextField))container, command, lenient, failureAllOrNone);
        return res.Select((x) => ((object)x.Item1, (object)x.Item2));
    }
    public List<(TElementObject, TElementField)> Search((TContextObject, TContextField) param, string command, bool lenient, bool failureAllOrNone)
    {
        return Search(param, unpacker(param), command, lenient, failureAllOrNone);
    }

    public virtual List<(TElementObject, TElementField)> Search((TContextObject, TContextField) context, List<(TElementObject, TElementField)> sourceSet, string command, bool lenient, bool failureAllOrNone)
    {
        //assumes unpacking doesn't fail
        var conditions = command.Split("&&", StringSplitOptions.TrimEntries);
        List<(TElementObject, TElementField)> liveSet = sourceSet;

        try
        {
            foreach (var condition in conditions)
            {
                //temp
                if (condition.Equals(""))
                {
                    break;
                }

                var cmd = condition.Split(' ', 2);

                SearchEngineCommand<(TContextObject, TContextField), (TElementObject, TElementField)> selectedCommand;
                int argC;
                string[] args;
                var not = false;
                if (cmd[0].Length > 0 && cmd[0].StartsWith('!'))
                {
                    cmd[0] = cmd[0].Substring(1);
                    not = true;
                }

                if (filterList.ContainsKey(cmd[0]))
                {
                    selectedCommand = filterList[cmd[0]];
                    argC = selectedCommand.args.Length;
                    args = cmd.Length == 1
                        ? []
                        : cmd[1].Split(' ', argC, StringSplitOptions.TrimEntries);
                }
                else
                {
                    selectedCommand = defaultFilter;
                    argC = selectedCommand.args.Length;
                    args = condition.Split(" ", argC, StringSplitOptions.TrimEntries);
                }

                for (var i = 0; i < argC; i++)
                {
                    if (args[i].StartsWith('$'))
                    {
                        args[i] = MassParamEdit.massEditVars[args[i].Substring(1)].ToString();
                    }
                }

                Func<(TContextObject, TContextField), Func<(TElementObject, TElementField), bool>> filter = selectedCommand.func(args, lenient);
                Func<(TElementObject, TElementField), bool> criteria = filter(context);
                List<(TElementObject, TElementField)> newRows = new();
                foreach ((TElementObject, TElementField) row in liveSet)
                {
                    if (not ^ criteria(row))
                    {
                        newRows.Add(row);
                    }
                }

                liveSet = newRows;
            }
        }
        catch (Exception e)
        {
            liveSet = failureAllOrNone ? sourceSet : [];
        }

        return liveSet;
    }

    internal override List<(TypelessSearchEngine, Type)> NextSearchEngines()
    {
        return GetSearchEngines(typeof((TElementObject, TElementField)));
    }
    internal override METypelessOperation NextOperation()
    {
        return METypelessOperation.GetEditOperation(typeof((TElementObject, TElementField)));
    }

    internal override string NameForHelpTexts()
    {
        return name;
    }

    internal override Type getContainerType()
    {
        return typeof((TContextObject, TContextField));
    }

    internal override Type getElementType()
    {
        return typeof((TElementObject, TElementField));
    }
}

internal class SearchEngineCommand<A, B>
{
    internal string[] args;
    internal Func<string[], bool, Func<A, Func<B, bool>>> func;
    internal Func<bool> shouldShow;
    internal string wiki;

    internal SearchEngineCommand(string[] args, string wiki, Func<string[], bool, Func<A, Func<B, bool>>> func,
        Func<bool> shouldShow)
    {
        this.args = args;
        this.wiki = wiki;
        this.func = func;
        this.shouldShow = shouldShow;
    }
}

internal class ParamRowSelectionSearchEngine : SearchEngine<bool, bool, string, Param.Row>
{
    public static ParamRowSelectionSearchEngine prsse = new();

    internal override void Setup()
    {
        name = "selection";
        unpacker = dummy => {
            string param = MassParamEditRegex.totalHackPleaseKillme.GetActiveParam();
            return MassParamEditRegex.totalHackPleaseKillme.GetSelectedRows().Select((x) => (param, x)).ToList();
        };
        filterList.Add("selection", newCmd([],
            "Selects param rows selected in the current param window",
            noArgs(noContext(param => true))));
    }
}
internal class ParamRowClipBoardSearchEngine : SearchEngine<bool, bool, string, Param.Row>
{
    public static ParamRowClipBoardSearchEngine prcse = new();

    internal override void Setup()
    {
        name = "clipboard";
        unpacker = dummy => {
            string param = ParamBank.ClipboardParam;
            return ParamBank.ClipboardRows.Select((x) => (param, x)).ToList();
        };
        filterList.Add("clipboard", newCmd([],
            "Selects param rows copied in the clipboard",
            noArgs(noContext(param => true))));
    }
}

internal class ParamSearchEngine : SearchEngine<bool, bool, ParamBank, Param>
{
    public static ParamSearchEngine pse = new(ParamBank.PrimaryBank);
    private readonly ParamBank bank;

    private ParamSearchEngine(ParamBank bank)
    {
        this.bank = bank;
    }

    internal override void Setup()
    {
        name = "param";
        unpacker = dummy =>
            ParamBank.AuxBanks.Select((aux, i) => aux.Value.Params.Select((x, i) => (aux.Value, x.Value)))
                .Aggregate(bank.Params.Values.Select((x, i) => (bank, x)), (o, n) => o.Concat(n)).Select((x, i) => (x.bank, x.x)).ToList();
        filterList.Add("modified", newCmd([],
            "Selects params where any rows do not match the vanilla version, or where any are added. Ignores row names",
            noArgs(noContext(param =>
            {
                if (param.Item1 != bank)
                {
                    return false;
                }

                HashSet<int> cache = bank.GetVanillaDiffRows(bank.GetKeyForParam(param.Item2));
                return cache.Count > 0;
            }))));
        filterList.Add("param", newCmd(["param name (regex)"],
            "Selects all params whose name matches the given regex", (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext(param =>
                    param.Item1 != bank
                        ? false
                        : rx.IsMatch(bank.GetKeyForParam(param.Item2) == null
                            ? ""
                            : bank.GetKeyForParam(param.Item2)));
            }));
        filterList.Add("auxparam", newCmd(["parambank name", "param name (regex)"],
            "Selects params from the specified regulation or parambnd where the param name matches the given regex",
            (args, lenient) =>
            {
                ParamBank auxBank = ParamBank.AuxBanks[args[0]];
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                return noContext(param =>
                    param.Item1 != auxBank
                        ? false
                        : rx.IsMatch(auxBank.GetKeyForParam(param.Item2) == null
                            ? ""
                            : auxBank.GetKeyForParam(param.Item2)));
            }, () => ParamBank.AuxBanks.Count > 0 && CFG.Current.Param_AdvancedMassedit));
        defaultFilter = newCmd(["param name (regex)"],
            "Selects all params whose name matches the given regex", (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext(param =>
                    param.Item1 != bank
                        ? false
                        : rx.IsMatch(bank.GetKeyForParam(param.Item2) == null
                            ? ""
                            : bank.GetKeyForParam(param.Item2)));
            });
    }
}

internal class RowSearchEngine : SearchEngine<ParamBank, Param, string, Param.Row>
{
    public static RowSearchEngine rse = new(ParamBank.PrimaryBank);
    private readonly ParamBank bank;

    private RowSearchEngine(ParamBank bank)
    {
        this.bank = bank;
    }

    internal override void Setup()
    {
        name = "row";
        unpacker = param =>
        {
            string name = param.Item1.GetKeyForParam(param.Item2);
            return param.Item2.Rows.Select((x, i) => (name, x)).ToList();
        };
        filterList.Add("modified", newCmd([],
            "Selects rows which do not match the vanilla version, or are added. Ignores row name", noArgs(context =>
                {
                    var paramName = context.Item1.GetKeyForParam(context.Item2);
                    HashSet<int> cache = context.Item1.GetVanillaDiffRows(paramName);
                    return row => cache.Contains(row.Item2.ID);
                }
            )));
        filterList.Add("added", newCmd([], "Selects rows where the ID is not found in the vanilla param",
            noArgs(context =>
                {
                    var paramName = context.Item1.GetKeyForParam(context.Item2);
                    if (!ParamBank.VanillaBank.Params.ContainsKey(paramName))
                    {
                        return row => true;
                    }

                    Param vanilParam = ParamBank.VanillaBank.Params[paramName];
                    return row => vanilParam[row.Item2.ID] == null;
                }
            )));
        filterList.Add("mergeable", newCmd([],
            "Selects rows which are not modified in the primary regulation or parambnd and there is exactly one equivalent row in another regulation or parambnd that is modified",
            noArgs(context =>
                {
                    var paramName = context.Item1.GetKeyForParam(context.Item2);
                    if (paramName == null)
                    {
                        return row => true;
                    }

                    HashSet<int> pCache = ParamBank.PrimaryBank.GetVanillaDiffRows(paramName);
                    List<(HashSet<int>, HashSet<int>)> auxCaches = ParamBank.AuxBanks.Select(x =>
                        (x.Value.GetPrimaryDiffRows(paramName), x.Value.GetVanillaDiffRows(paramName))).ToList();
                    return row =>
                        !pCache.Contains(row.Item2.ID) &&
                        auxCaches.Where(x => x.Item2.Contains(row.Item2.ID) && x.Item1.Contains(row.Item2.ID)).Count() == 1;
                }
            ), () => ParamBank.AuxBanks.Count > 0));
        filterList.Add("conflicts", newCmd([],
            "Selects rows which, among all equivalents in the primary and additional regulations or parambnds, there is more than row 1 which is modified",
            noArgs(context =>
                {
                    var paramName = context.Item1.GetKeyForParam(context.Item2);
                    HashSet<int> pCache = ParamBank.PrimaryBank.GetVanillaDiffRows(paramName);
                    List<(HashSet<int>, HashSet<int>)> auxCaches = ParamBank.AuxBanks.Select(x =>
                        (x.Value.GetPrimaryDiffRows(paramName), x.Value.GetVanillaDiffRows(paramName))).ToList();
                    return row =>
                        (pCache.Contains(row.Item2.ID) ? 1 : 0) + auxCaches
                            .Where(x => x.Item2.Contains(row.Item2.ID) && x.Item1.Contains(row.Item2.ID)).Count() > 1;
                }
            ), () => ParamBank.AuxBanks.Count > 0));
        filterList.Add("id", newCmd(["row id (regex)"], "Selects rows whose ID matches the given regex",
            (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[0].ToLower()) : new Regex($@"^{args[0]}$");
                return noContext(row => rx.IsMatch(row.Item2.ID.ToString()));
            }));
        filterList.Add("idrange", newCmd(["row id minimum (inclusive)", "row id maximum (inclusive)"],
            "Selects rows whose ID falls in the given numerical range", (args, lenient) =>
            {
                var floor = double.Parse(args[0]);
                var ceil = double.Parse(args[1]);
                return noContext(row => row.Item2.ID >= floor && row.Item2.ID <= ceil);
            }));
        filterList.Add("name", newCmd(["row name (regex)"],
            "Selects rows whose Name matches the given regex", (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext(row => rx.IsMatch(row.Item2.Name == null ? "" : row.Item2.Name));
            }));
        filterList.Add("prop", newCmd(["field internalName", "field value (regex)"],
            "Selects rows where the specified field has a value that matches the given regex", (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                var field = args[0];
                return noContext(row =>
                {
                    Param.Cell? cq = row.Item2[field];
                    if (cq == null)
                    {
                        throw new Exception();
                    }

                    Param.Cell c = cq.Value;
                    var term = c.Value.ToParamEditorString();
                    return rx.IsMatch(term);
                });
            }));
        filterList.Add("proprange", newCmd(
            ["field internalName", "field value minimum (inclusive)", "field value maximum (inclusive)"],
            "Selects rows where the specified field has a value that falls in the given numerical range",
            (args, lenient) =>
            {
                var field = args[0];
                var floor = double.Parse(args[1]);
                var ceil = double.Parse(args[2]);
                return noContext(row =>
                {
                    Param.Cell? c = row.Item2[field];
                    if (c == null)
                    {
                        throw new Exception();
                    }

                    return Convert.ToDouble(c.Value.Value) >= floor && Convert.ToDouble(c.Value.Value) <= ceil;
                });
            }));
        filterList.Add("propref", newCmd(["field internalName", "referenced row name (regex)"],
            "Selects rows where the specified field that references another param has a value referencing a row whose name matches the given regex",
            (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                var field = args[0];
                return context =>
                {
                    List<ParamRef> validFields = FieldMetaData
                        .Get(context.Item2.AppliedParamdef.Fields.Find(f => f.InternalName.Equals(field))).RefTypes
                        .FindAll(p => bank.Params.ContainsKey(p.param));
                    return row =>
                    {
                        Param.Cell? c = row.Item2[field];
                        if (c == null)
                        {
                            throw new Exception();
                        }

                        var val = (int)c.Value.Value;
                        foreach (ParamRef rt in validFields)
                        {
                            Param.Row r = bank.Params[rt.param][val];
                            if (r != null && rx.IsMatch(r.Name ?? ""))
                            {
                                return true;
                            }
                        }

                        return false;
                    };
                };
            }, () => CFG.Current.Param_AdvancedMassedit));
        filterList.Add("propwhere", newCmd(["field internalName", "cell/field selector"],
            "Selects rows where the specified field appears when the given cell/field search is given",
            (args, lenient) =>
            {
                var field = args[0];
                return context =>
                {
                    var paramName = context.Item1.GetKeyForParam(context.Item2);
                    IReadOnlyList<Param.Column> cols = context.Item2.Columns;
                    var vtup = context.Item2.GetCol(field);
                    (PseudoColumn, Param.Column) testCol = (vtup.Item1, vtup.Item2);
                    return row =>
                    {
                        (string paramName, Param.Row row) cseSearchContext = (paramName, row.Item2);
                        List<(PseudoColumn, Param.Column)> res = CellSearchEngine.cse.Search(cseSearchContext,
                            new List<(PseudoColumn, Param.Column)> { testCol }, args[1], lenient, false);
                        return res.Contains(testCol);
                    };
                };
            }, () => CFG.Current.Param_AdvancedMassedit));
        filterList.Add("fmg", newCmd(["fmg title (regex)"],
            "Selects rows which have an attached FMG and that FMG's text matches the given regex",
            (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return context =>
                {
                    var category = FmgEntryCategory.None;
                    var paramName = context.Item1.GetKeyForParam(context.Item2);
                    foreach ((var param, FmgEntryCategory cat) in ParamBank.ParamToFmgCategoryList)
                    {
                        if (paramName != param)
                        {
                            continue;
                        }

                        category = cat;
                    }

                    if (category == FmgEntryCategory.None)
                    {
                        throw new Exception();
                    }

                    List<FMG.Entry> fmgEntries = FMGBank.GetFmgEntriesByCategory(category, false);
                    Dictionary<int, FMG.Entry> _cache = new();
                    foreach (FMG.Entry fmgEntry in fmgEntries)
                    {
                        _cache[fmgEntry.ID] = fmgEntry;
                    }

                    return row =>
                    {
                        if (!_cache.ContainsKey(row.Item2.ID))
                        {
                            return false;
                        }

                        FMG.Entry e = _cache[row.Item2.ID];
                        return e != null && rx.IsMatch(e.Text ?? "");
                    };
                };
            }, () => CFG.Current.Param_AdvancedMassedit));
        filterList.Add("vanillaprop", newCmd(["field internalName", "field value (regex)"],
            "Selects rows where the vanilla equivilent of that row has a value for the given field that matches the given regex",
            (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[1], RegexOptions.IgnoreCase) : new Regex($@"^{args[1]}$");
                var field = args[0];
                return param =>
                {
                    Param vparam = ParamBank.VanillaBank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return row =>
                    {
                        Param.Row vrow = vparam[row.Item2.ID];
                        if (vrow == null)
                        {
                            return false;
                        }

                        Param.Cell? cq = vrow[field];
                        if (cq == null)
                        {
                            throw new Exception();
                        }

                        Param.Cell c = cq.Value;
                        var term = c.Value.ToParamEditorString();
                        return rx.IsMatch(term);
                    };
                };
            }, () => CFG.Current.Param_AdvancedMassedit));
        filterList.Add("vanillaproprange", newCmd(
            ["field internalName", "field value minimum (inclusive)", "field value maximum (inclusive)"],
            "Selects rows where the vanilla equivilent of that row has a value for the given field that falls in the given numerical range",
            (args, lenient) =>
            {
                var field = args[0];
                var floor = double.Parse(args[1]);
                var ceil = double.Parse(args[2]);
                return param =>
                {
                    Param vparam = ParamBank.VanillaBank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return row =>
                    {
                        Param.Row vrow = vparam[row.Item2.ID];
                        if (vrow == null)
                        {
                            return false;
                        }

                        Param.Cell? c = vrow[field];
                        if (c == null)
                        {
                            throw new Exception();
                        }

                        return Convert.ToDouble(c.Value.Value) >= floor && Convert.ToDouble(c.Value.Value) <= ceil;
                    };
                };
            }, () => CFG.Current.Param_AdvancedMassedit));
        filterList.Add("auxprop", newCmd(["parambank name", "field internalName", "field value (regex)"],
            "Selects rows where the equivilent of that row in the given regulation or parambnd has a value for the given field that matches the given regex.\nCan be used to determine if an aux row exists.",
            (args, lenient) =>
            {
                Regex rx = lenient ? new Regex(args[2], RegexOptions.IgnoreCase) : new Regex($@"^{args[2]}$");
                var field = args[1];
                ParamBank bank;
                if (!ParamBank.AuxBanks.TryGetValue(args[0], out bank))
                {
                    throw new Exception("Unable to find auxbank " + args[0]);
                }

                return param =>
                {
                    Param vparam = bank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return row =>
                    {
                        Param.Row vrow = vparam[row.Item2.ID];
                        if (vrow == null)
                        {
                            return false;
                        }

                        Param.Cell? cq = vrow[field];
                        if (cq == null)
                        {
                            throw new Exception();
                        }

                        Param.Cell c = cq.Value;
                        var term = c.Value.ToParamEditorString();
                        return rx.IsMatch(term);
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0 && CFG.Current.Param_AdvancedMassedit));
        filterList.Add("auxproprange", newCmd(
            [
                "parambank name", "field internalName", "field value minimum (inclusive)",
                "field value maximum (inclusive)"
            ],
            "Selects rows where the equivilent of that row in the given regulation or parambnd has a value for the given field that falls in the given range",
            (args, lenient) =>
            {
                var field = args[0];
                var floor = double.Parse(args[1]);
                var ceil = double.Parse(args[2]);
                ParamBank bank;
                if (!ParamBank.AuxBanks.TryGetValue(args[0], out bank))
                {
                    throw new Exception("Unable to find auxbank " + args[0]);
                }

                return param =>
                {
                    Param vparam = bank.GetParamFromName(param.Item1.GetKeyForParam(param.Item2));
                    return row =>
                    {
                        Param.Row vrow = vparam[row.Item2.ID];
                        Param.Cell? c = vrow[field];
                        if (c == null)
                        {
                            throw new Exception();
                        }

                        return Convert.ToDouble(c.Value.Value) >= floor && Convert.ToDouble(c.Value.Value) <= ceil;
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0 && CFG.Current.Param_AdvancedMassedit));
        filterList.Add("semijoin",
            newCmd(
                [
                    "this field internalName", "other param", "other param field internalName",
                    "other param row search"
                ],
                "Selects all rows where the value of a given field is any of the values in the second given field found in the given param using the given row selector",
                (args, lenient) =>
                {
                    var thisField = args[0];
                    var otherParam = args[1];
                    var otherField = args[2];
                    var otherSearchTerm = args[3];
                    Param otherParamReal;
                    if (!ParamBank.PrimaryBank.Params.TryGetValue(otherParam, out otherParamReal))
                    {
                        throw new Exception("Could not find param " + otherParam);
                    }

                    List<(string, Param.Row)> rows = rse.Search((ParamBank.PrimaryBank, otherParamReal), otherSearchTerm,
                        lenient, false);
                    (PseudoColumn, Param.Column) otherFieldReal = otherParamReal.GetCol(otherField);
                    if (!otherFieldReal.IsColumnValid())
                    {
                        throw new Exception("Could not find field " + otherField);
                    }

                    HashSet<string> possibleValues = rows.Select(x => x.Item2.Get(otherFieldReal).ToParamEditorString())
                        .Distinct().ToHashSet();
                    return param =>
                    {
                        (PseudoColumn, Param.Column) thisFieldReal = param.Item2.GetCol(thisField);
                        if (!thisFieldReal.IsColumnValid())
                        {
                            throw new Exception("Could not find field " + thisField);
                        }

                        return row =>
                        {
                            var toFind = row.Item2.Get(thisFieldReal).ToParamEditorString();
                            return possibleValues.Contains(toFind);
                        };
                    };
                }, () => CFG.Current.Param_AdvancedMassedit));
        filterList.Add("unique", newCmd(["field"], "Selects all rows where the value in the given field is unique", (args, lenient) =>
        {
            string field = args[0].Replace(@"\s", " ");
            return (param) =>
            {
                var col = param.Item2.GetCol(field);
                if (!col.IsColumnValid())
                    throw new Exception("Could not find field " + field);
                var distribution = ParamUtils.GetParamValueDistribution(param.Item2.Rows, col);
                var setOfDuped = distribution.Where((entry, linqi) => entry.Item2 > 1).Select((entry, linqi) => entry.Item1).ToHashSet();
                return (row) =>
                {
                    return !setOfDuped.Contains(row.Item2.Get(col));
                };
            };
        }, () => CFG.Current.Param_AdvancedMassedit));
        defaultFilter = newCmd(["row ID or Name (regex)"],
            "Selects rows where either the ID or Name matches the given regex, except in strict/massedit mode",
            (args, lenient) =>
            {
                if (!lenient)
                {
                    return noContext(row => false);
                }

                Regex rx = new(args[0], RegexOptions.IgnoreCase);
                return paramContext =>
                {
                    var category = FmgEntryCategory.None;
                    var paramName = paramContext.Item1.GetKeyForParam(paramContext.Item2);
                    foreach ((var param, FmgEntryCategory cat) in ParamBank.ParamToFmgCategoryList)
                    {
                        if (paramName != param)
                        {
                            continue;
                        }

                        category = cat;
                    }

                    if (category == FmgEntryCategory.None || !FMGBank.IsLoaded)
                    {
                        return row => rx.IsMatch(row.Item2.Name ?? "") || rx.IsMatch(row.Item2.ID.ToString());
                    }

                    List<FMG.Entry> fmgEntries = FMGBank.GetFmgEntriesByCategory(category, false);
                    Dictionary<int, FMG.Entry> _cache = new();
                    foreach (FMG.Entry fmgEntry in fmgEntries)
                    {
                        _cache[fmgEntry.ID] = fmgEntry;
                    }

                    return row =>
                    {
                        if (rx.IsMatch(row.Item2.Name ?? "") || rx.IsMatch(row.Item2.ID.ToString()))
                        {
                            return true;
                        }

                        if (!_cache.ContainsKey(row.Item2.ID))
                        {
                            return false;
                        }

                        FMG.Entry e = _cache[row.Item2.ID];
                        return e != null && rx.IsMatch(e.Text ?? "");
                    };
                };
            });
    }
}

internal class CellSearchEngine : SearchEngine<string, Param.Row, PseudoColumn, Param.Column>
{
    public static CellSearchEngine cse = new();

    internal override void Setup()
    {
        name = "cell/property";
        unpacker = row =>
        {
            List<(PseudoColumn, Param.Column)> list = new();
            list.Add((PseudoColumn.ID, null));
            list.Add((PseudoColumn.Name, null));
            list.AddRange(row.Item2.Columns.Select((cell, i) => (PseudoColumn.None, cell)));
            return list;
        };
        defaultFilter = newCmd(["field internalName (regex)"],
            "Selects cells/fields where the internal name of that field matches the given regex", (args, lenient) =>
            {
                var matchID = args[0] == "ID";
                var matchName = args[0] == "Name";
                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext(cell =>
                {
                    if (matchID && cell.Item1 == PseudoColumn.ID)
                    {
                        return true;
                    }

                    if (matchName && cell.Item1 == PseudoColumn.Name)
                    {
                        return true;
                    }

                    if (cell.Item2 != null)
                    {
                        FieldMetaData meta = lenient ? FieldMetaData.Get(cell.Item2.Def) : null;
                        if (lenient && meta?.AltName != null && rx.IsMatch(meta?.AltName))
                        {
                            return true;
                        }

                        if (rx.IsMatch(cell.Item2.Def.InternalName))
                        {
                            return true;
                        }
                    }

                    return false;
                });
            });
        filterList.Add("modified", newCmd([],
            "Selects cells/fields where the equivalent cell in the vanilla regulation or parambnd has a different value",
            (args, lenient) => row =>
            {
                if (row.Item1 == null)
                {
                    throw new Exception("Can't check if cell is modified - not part of a param");
                }

                Param vParam = ParamBank.VanillaBank.Params?[row.Item1];
                if (vParam == null)
                {
                    throw new Exception("Can't check if cell is modified - no vanilla param");
                }

                Param.Row r = vParam[row.Item2.ID];
                if (r == null)
                {
                    return col => true;
                }

                return col =>
                {
                    (PseudoColumn, Param.Column) vcol = (col.Item1, col.Item2).GetAs(vParam);
                    var valA = row.Item2.Get((col.Item1, col.Item2));
                    var valB = r.Get(vcol);
                    return ParamUtils.IsValueDiff(ref valA, ref valB, (col.Item1, col.Item2).GetColumnType());
                };
            }));
        filterList.Add("auxmodified", newCmd(["parambank name"],
            "Selects cells/fields where the equivalent cell in the specified regulation or parambnd has a different value",
            (args, lenient) =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(args[0]))
                {
                    throw new Exception("Can't check if cell is modified - parambank not found");
                }

                ParamBank bank = ParamBank.AuxBanks[args[0]];
                return row =>
                {
                    if (row.Item1 == null)
                    {
                        throw new Exception("Can't check if cell is modified - not part of a param");
                    }

                    Param auxParam = bank.Params?[row.Item1];
                    if (auxParam == null)
                    {
                        throw new Exception("Can't check if cell is modified - no aux param");
                    }

                    Param vParam = ParamBank.VanillaBank.Params?[row.Item1];
                    if (vParam == null)
                    {
                        throw new Exception("Can't check if cell is modified - no vanilla param");
                    }

                    Param.Row r = auxParam[row.Item2.ID];
                    Param.Row r2 = vParam[row.Item2.ID];
                    if (r == null)
                    {
                        return col => false;
                    }

                    if (r2 == null)
                    {
                        return col => true;
                    }

                    return col =>
                    {
                        (PseudoColumn, Param.Column) auxcol = (col.Item1, col.Item2).GetAs(auxParam);
                        (PseudoColumn, Param.Column) vcol = (col.Item1, col.Item2).GetAs(vParam);
                        var valA = r.Get(auxcol);
                        var valB = r2.Get(vcol);
                        return ParamUtils.IsValueDiff(ref valA, ref valB, (col.Item1, col.Item2).GetColumnType());
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        filterList.Add("sftype", newCmd(["paramdef type"],
            "Selects cells/fields where the field's data type, as enumerated by soulsformats, matches the given regex",
            (args, lenient) =>
            {
                Regex r = new('^' + args[0] + '$',
                    lenient ? RegexOptions.IgnoreCase : RegexOptions.None); //Leniency rules break from the norm
                return row => col => r.IsMatch((col.Item1, col.Item2).GetColumnSfType());
            }, () => CFG.Current.Param_AdvancedMassedit));
    }
}

internal class VarSearchEngine : SearchEngine<bool, bool, bool, string>
{
    public static VarSearchEngine vse = new();

    internal override void Setup()
    {
        name = "variable";
        unpacker = dummy =>
        {
            return MassParamEdit.massEditVars.Keys.Select(x => (true, x)).ToList();
        };
        filterList.Add("vars", newCmd(["variable names (regex)"],
            "Selects variables whose name matches the given regex", (args, lenient) =>
            {
                if (args[0].StartsWith('$'))
                {
                    args[0] = args[0].Substring(1);
                }

                Regex rx = lenient ? new Regex(args[0], RegexOptions.IgnoreCase) : new Regex($@"^{args[0]}$");
                return noContext(bname => rx.IsMatch(bname.Item2));
            }));
    }
}
