#nullable enable
using Andre.Formats;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StudioCore.Editor;
using StudioCore.ParamEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace StudioCore.Editor.MassEdit;

internal abstract class METypelessOperationDef
{
    internal string[] argNames;
    internal string wiki;
    internal Func<object, object, string[], object> function;
    internal Func<bool> shouldShow;
}
internal class MEOperationDef<TMECategory, TInputObject, TInputValue, TOutput> : METypelessOperationDef
{
    internal MEOperationDef(string[] args, string tooltip, Func<TInputValue, string[], TOutput> func, Func<bool> show = null)
    {
        argNames = args;
        wiki = tooltip;
        function = (dummy, v, str) => func((TInputValue)v, str); //Shitty wrapping perf loss.
        shouldShow = show;
    }
    internal MEOperationDef(string[] args, string tooltip, Func<TInputObject, TInputValue, string[], TOutput> func, Func<bool> show = null)
    {
        argNames = args;
        wiki = tooltip;
        function = (o, v, str) => func((TInputObject) o, (TInputValue)v, str); //Shitty wrapping perf loss.
        shouldShow = show;
    }
}
internal abstract class METypelessOperation
{
    private static Dictionary<Type, METypelessOperation> editOperations;

    internal static MEGlobalOperation global;
    internal static MERowOperation row;
    internal static MECellOperation cell;
    internal static MEVarOperation var;
    static METypelessOperation()
    {
        editOperations = new();
        global = new();
        row = new();
        cell = new();
        var = new();
    }

    internal static void AddEditOperation<TMECategory, TInputObject, TInputValue, TOutput>(MEOperation<TMECategory, TInputObject, TInputValue, TOutput> engine)
    {
        editOperations[typeof(TMECategory)] = engine;
    }
    internal static METypelessOperation GetEditOperation(Type t)
    {
        return editOperations.GetValueOrDefault(t);
    }

    internal abstract Dictionary<string, METypelessOperationDef> AllCommands();
    internal abstract string NameForHelpTexts();
    internal abstract object GetElementValue((object, object) currentObject, Dictionary<Type, (object, object)> contextObjects);
    internal abstract bool ValidateResult(object res);
    internal abstract void UseResult(List<EditorAction> actionList, (object, object) currentObject, Dictionary<Type, (object, object)> contextObjects, object res);
    internal abstract bool HandlesCommand(string command);
}
internal abstract class MEOperation<TMECategory, TInputObject, TInputValue, TOutput> : METypelessOperation
{
    internal Dictionary<string, METypelessOperationDef> operations = new();
    internal string name = "[Unnamed operation type]";

    internal MEOperation()
    {
        Setup();
        AddEditOperation(this);
    }

    internal virtual void Setup()
    {
    }

    internal override bool HandlesCommand(string command)
    {
        return operations.ContainsKey(command);
    }
    internal override Dictionary<string, METypelessOperationDef> AllCommands()
    {
        return operations;
    }
    internal void NewCmd(string command, string[] args, string wiki, Func<TInputValue, string[], TOutput> func, Func<bool> show = null)
    {
        operations.Add(command, new MEOperationDef<TMECategory, TInputObject, TInputValue, TOutput>(args, wiki, func, show));
    }
    internal void NewCmd(string command, string[] args, string wiki, Func<TInputObject, TInputValue, string[], TOutput> func, Func<bool> show = null)
    {
        operations.Add(command, new MEOperationDef<TMECategory, TInputObject, TInputValue, TOutput>(args, wiki, func, show));
    }

    internal override string NameForHelpTexts()
    {
        return name;
    }
}

internal class MEGlobalOperation : MEOperation<(bool, bool), bool, bool, bool>
{
    internal override void Setup()
    {
        name = "global";
        NewCmd("clear", [], "Clears clipboard param and rows", (dummy, args) =>
        {
            ParamBank.ClipboardParam = null;
            ParamBank.ClipboardRows.Clear();
            return true;
        });
        NewCmd("newvar", ["variable name", "value"],
            "Creates a variable with the given value, and the type of that value", (dummy, args) =>
            {
                int asInt;
                double asDouble;
                if (int.TryParse(args[1], out asInt))
                {
                    MassParamEdit.massEditVars[args[0]] = asInt;
                }
                else if (double.TryParse(args[1], out asDouble))
                {
                    MassParamEdit.massEditVars[args[0]] = asDouble;
                }
                else
                {
                    MassParamEdit.massEditVars[args[0]] = args[1];
                }

                return true;
            }, () => CFG.Current.Param_AdvancedMassedit);
        NewCmd("clearvars", [], "Deletes all variables", (dummy, args) =>
        {
            MassParamEdit.massEditVars.Clear();
            return true;
        }, () => CFG.Current.Param_AdvancedMassedit);
    }
    internal override object GetElementValue((object, object) currentObject, Dictionary<Type, (object, object)> contextObjects)
    {
        return true; //Global op technically has no context / uses the dummy context of boolean
    }

    internal override bool ValidateResult(object res)
    {
        return true;
    }

    internal override void UseResult(List<EditorAction> actionList, (object, object) currentObject, Dictionary<Type, (object, object)> contextObjects, object res)
    {
        return; //Global ops, for now, don't use actions and simply execute effects themselves
    }
}

internal class MERowOperation : MEOperation<(string, Param.Row), string, Param.Row, (Param, Param.Row)> //technically we're still using string as the containing object in place of Param
{
    internal override void Setup()
    {
        name = "row";
        NewCmd("copy", [],
            "Adds the selected rows into clipboard. If the clipboard param is different, the clipboard is emptied first",
            (paramKey, row, args) =>
            {
                if (paramKey == null)
                {
                    throw new MEOperationException(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new MEOperationException($@"Could not locate param {paramKey}");
                }

                Param p = ParamBank.PrimaryBank.Params[paramKey];
                // Only supporting single param in clipboard
                if (ParamBank.ClipboardParam != paramKey)
                {
                    ParamBank.ClipboardParam = paramKey;
                    ParamBank.ClipboardRows.Clear();
                }

                ParamBank.ClipboardRows.Add(new Param.Row(row, p));
                return (p, null);
            }
        );
        NewCmd("copyN", ["count"],
            "Adds the selected rows into clipboard the given number of times. If the clipboard param is different, the clipboard is emptied first",
            (paramKey, row, args) =>
            {
                if (paramKey == null)
                {
                    throw new MEOperationException(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new MEOperationException($@"Could not locate param {paramKey}");
                }

                var count = uint.Parse(args[0]);
                Param p = ParamBank.PrimaryBank.Params[paramKey];
                // Only supporting single param in clipboard
                if (ParamBank.ClipboardParam != paramKey)
                {
                    ParamBank.ClipboardParam = paramKey;
                    ParamBank.ClipboardRows.Clear();
                }

                for (var i = 0; i < count; i++)
                {
                    ParamBank.ClipboardRows.Add(new Param.Row(row, p));
                }

                return (p, null);
            }, () => CFG.Current.Param_AdvancedMassedit);
        NewCmd("paste", [],
            "Adds the selected rows to the primary regulation or parambnd in the selected param",
            (paramKey, row, args) =>
            {
                if (paramKey == null)
                {
                    throw new MEOperationException(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new MEOperationException($@"Could not locate param {paramKey}");
                }

                Param p = ParamBank.PrimaryBank.Params[paramKey];
                return (p, new Param.Row(row, p));
            }
        );
    }
    internal override object GetElementValue((object, object) currentObject, Dictionary<Type, (object, object)> contextObjects)
    {
        return currentObject.Item2;
    }

    internal override bool ValidateResult(object res)
    {
        if (res.GetType() != typeof((Param, Param.Row)))
            return false;
        (Param, Param.Row) r2 = ((Param, Param.Row))res;
        if (r2.Item1 == null)
            return false;
        return true;
    }

    internal override void UseResult(List<EditorAction> actionList, (object, object) currentObject, Dictionary<Type, (object, object)> contextObjects, object res)
    {
        //use Param from result as this may be different to original Param obj
        (Param p2, Param.Row rs) = ((Param, Param.Row))res;
        actionList.Add(new AddParamsAction(p2, "FromMassEdit", new List<Param.Row> { rs }, false, true));
    }
}

internal abstract class MEValueOperation<TMECategory> : MEOperation<TMECategory, TMECategory, object, object>
{
    internal override void Setup()
    {
        name = "value";
        NewCmd("=",
            ["number or text"],
                "Assigns the given value to the selected values. Will attempt conversion to the value's data type",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => args[0]));
        NewCmd("+", ["number or text"],
            "Adds the number to the selected values, or appends text if that is the data type of the values",
            (value, args) => MassParamEdit.WithDynamicOf(value, v =>
            {
                if (double.TryParse(args[0], out double val))
                {
                    return v + val;
                }

                return v + args[0];
            }));
        NewCmd("-",
            ["number"], "Subtracts the number from the selected values",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => v - double.Parse(args[0])));
        NewCmd("*",
            ["number"], "Multiplies selected values by the number",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => v * double.Parse(args[0])));
        NewCmd("/",
            ["number"], "Divides the selected values by the number",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => v / double.Parse(args[0])));
        NewCmd("%",
            ["number"], "Gives the remainder when the selected values are divided by the number",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => v % double.Parse(args[0])), () => CFG.Current.Param_AdvancedMassedit);
        NewCmd("scale", ["factor number", "center number"],
            "Multiplies the difference between the selected values and the center number by the factor number",
            (value, args) =>
            {
                var opp1 = double.Parse(args[0]);
                var opp2 = double.Parse(args[1]);
                return MassParamEdit.WithDynamicOf(value, v =>
                {
                    return ((v - opp2) * opp1) + opp2;
                });
            }
        );
        NewCmd("replace",
            ["text to replace", "new text"],
                "Interprets the selected values as text and replaces all occurances of the text to replace with the new text",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => v.Replace(args[0], args[1])));
        NewCmd("replacex", ["text to replace (regex)", "new text (w/ groups)"],
            "Interprets the selected values as text and replaces all occurances of the given regex with the replacement, supporting regex groups",
            (value, args) =>
            {
                Regex rx = new(args[0]);
                return MassParamEdit.WithDynamicOf(value, v => rx.Replace(v, args[1]));
            }, () => CFG.Current.Param_AdvancedMassedit);
        NewCmd("max",
            ["number"], "Returns the larger of the current value and number",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => Math.Max(v, double.Parse(args[0]))), () => CFG.Current.Param_AdvancedMassedit);
        NewCmd("min",
            ["number"], "Returns the smaller of the current value and number",
                (value, args) => MassParamEdit.WithDynamicOf(value, v => Math.Min(v, double.Parse(args[0]))), () => CFG.Current.Param_AdvancedMassedit);
    }

    internal override bool ValidateResult(object res)
    {
        if (res == null)
            return false;
        return true;
    }
}
internal class MECellOperation : MEValueOperation<(PseudoColumn, Param.Column)>
{
    internal override object GetElementValue((object, object) currentObject, Dictionary<Type, (object, object)> contextObjects)
    {
        (string param, Param.Row row) = ((string, Param.Row))contextObjects[typeof((string, Param.Row))];
        (PseudoColumn, Param.Column) col = ((PseudoColumn, Param.Column))currentObject;
        return row.Get(col);
    }
    internal override void UseResult(List<EditorAction> actionList, (object, object) currentObject, Dictionary<Type, (object, object)> contextObjects, object res)
    {
        (string param, Param.Row row) = ((string, Param.Row))contextObjects[typeof((string, Param.Row))];
        (PseudoColumn, Param.Column) col = ((PseudoColumn, Param.Column))currentObject;
        actionList.AppendParamEditAction(row, col, res);
    }
}
internal class MEVarOperation : MEValueOperation<string>
{
    internal override object GetElementValue((object, object) currentObject, Dictionary<Type, (object, object)> contextObjects)
    {
        return MassParamEdit.massEditVars[(string)currentObject.Item2];
    }

    internal override void UseResult(List<EditorAction> actionList, (object, object) currentObject, Dictionary<Type, (object, object)> contextObjects, object res)
    {
        MassParamEdit.massEditVars[(string)currentObject.Item2] = res;
    }
}

internal class MEOperationArgument
{
    internal static MEOperationArgument arg = new();
    private readonly Dictionary<string, OperationArgumentGetter> argumentGetters = new();
    private OperationArgumentGetter defaultGetter;

    private MEOperationArgument()
    {
        Setup();
    }
    private OperationArgumentGetter newGetter<TContextInput1Object, TContextInput1Field>(string[] args, string wiki,
        Func<string[], Func<int, TContextInput1Object, TContextInput1Field, object>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }
    private OperationArgumentGetter newGetter<TContextInput1Object, TContextInput1Field, TContextInput2Object, TContextInput2Field>(string[] args, string wiki,
        Func<string[], Func<int, TContextInput1Object, TContextInput1Field, Func<int, TContextInput2Object, TContextInput2Field, object>>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }
    private OperationArgumentGetter newGetter<TContextInput1Object, TContextInput1Field, TContextInput2Object, TContextInput2Field, TContextInput3Object, TContextInput3Field>(string[] args, string wiki,
        Func<string[], Func<int, TContextInput1Object, TContextInput1Field, Func<int, TContextInput2Object, TContextInput2Field, Func<int, TContextInput3Object, TContextInput3Field, object>>>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }

    private void Setup()
    {
        defaultGetter = newGetter<bool, bool>([], "Gives the specified value",
            value => (i, c, c2) => value[0]);
        argumentGetters.Add("self", newGetter<string, Param.Row, PseudoColumn, Param.Column>([], "Gives the value of the currently selected value",
            empty => (j, rowP, rowR) => (k, colE, colC) =>
            {
                return rowR.Get((colE, colC)).ToParamEditorString();
            }));
        argumentGetters.Add("field", newGetter<ParamBank, Param, string, Param.Row>(["field internalName"],
            "Gives the value of the given cell/field for the currently selected row and param", field =>
                (i, paramB, paramP) =>
                {
                    (PseudoColumn, Param.Column) col = paramP.GetCol(field[0]);
                    if (!col.IsColumnValid())
                    {
                        throw new MEOperationException($@"Could not locate field {field[0]}");
                    }

                    return (j, rowP, rowR) =>
                    {
                        var v = rowR.Get(col).ToParamEditorString();
                        return v;
                    };
                }));
        argumentGetters.Add("vanilla", newGetter<ParamBank, Param, string, Param.Row, PseudoColumn, Param.Column>([],
            "Gives the value of the equivalent cell/field in the vanilla regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added",
            empty =>
            {
                ParamBank bank = ParamBank.VanillaBank;
                return (i, paramB, paramP) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(paramP);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new MEOperationException($@"Could not locate vanilla param for {paramP.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    return (j, rowP, rowR) =>
                    {
                        Param.Row vRow = vParam?[rowR.ID];
                        if (vRow == null)
                        {
                            throw new MEOperationException($@"Could not locate vanilla row {rowR.ID}");
                        }

                        return (k, colE, colC) =>
                        {
                            if (colE == PseudoColumn.None && colC == null)
                            {
                                throw new MEOperationException(@"Could not locate given field or property");
                            }

                            return vRow.Get((colE, colC)).ToParamEditorString();
                        };
                    };
                };
            }));
        argumentGetters.Add("aux", newGetter<ParamBank, Param, string, Param.Row, PseudoColumn, Param.Column>(["parambank name"],
            "Gives the value of the equivalent cell/field in the specified regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*",
            bankName =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(bankName[0]))
                {
                    throw new MEOperationException($@"Could not locate paramBank {bankName[0]}");
                }

                ParamBank bank = ParamBank.AuxBanks[bankName[0]];
                return (i, paramB, paramP) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(paramP);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new MEOperationException($@"Could not locate aux param for {paramP.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    return (j, rowP, rowR) =>
                    {
                        Param.Row vRow = vParam?[rowR.ID];
                        if (vRow == null)
                        {
                            throw new MEOperationException($@"Could not locate aux row {rowR.ID}");
                        }

                        return (k, colE, colC) =>
                        {
                            if (!(colE, colC).IsColumnValid())
                            {
                                throw new MEOperationException(@"Could not locate given field or property");
                            }

                            return vRow.Get((colE, colC)).ToParamEditorString();
                        };
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        argumentGetters.Add("vanillafield", newGetter<ParamBank, Param, string, Param.Row>(["field internalName"],
            "Gives the value of the specified cell/field in the vanilla regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added",
            field => (i, paramB, paramP) =>
            {
                var paramName = ParamBank.PrimaryBank.GetKeyForParam(paramP);
                Param? vParam = ParamBank.VanillaBank.GetParamFromName(paramName);
                if (vParam == null)
                {
                    throw new MEOperationException($@"Could not locate vanilla param for {paramP.ParamType}");
                }

                (PseudoColumn, Param.Column) col = vParam.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new MEOperationException($@"Could not locate field {field[0]}");
                }

                return (j, rowP, rowR) =>
                {
                    Param.Row vRow = vParam?[rowR.ID];
                    if (vRow == null)
                    {
                        throw new MEOperationException($@"Could not locate vanilla row {rowR.ID}");
                    }

                    var v = vRow.Get(col).ToParamEditorString();
                    return v;
                };
            }));
        argumentGetters.Add("auxfield", newGetter<ParamBank, Param, string, Param.Row>(["parambank name", "field internalName"],
            "Gives the value of the specified cell/field in the specified regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*",
            bankAndField =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(bankAndField[0]))
                {
                    throw new MEOperationException($@"Could not locate paramBank {bankAndField[0]}");
                }

                ParamBank bank = ParamBank.AuxBanks[bankAndField[0]];
                return (i, paramB, paramP) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(paramP);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new MEOperationException($@"Could not locate aux param for {paramP.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    (PseudoColumn, Param.Column) col = vParam.GetCol(bankAndField[1]);
                    if (!col.IsColumnValid())
                    {
                        throw new MEOperationException($@"Could not locate field {bankAndField[1]}");
                    }

                    return (j, rowP, rowR) =>
                    {
                        Param.Row vRow = vParam?[rowR.ID];
                        if (vRow == null)
                        {
                            throw new MEOperationException($@"Could not locate aux row {rowR.ID}");
                        }

                        var v = vRow.Get(col).ToParamEditorString();
                        return v;
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        argumentGetters.Add("paramlookup", newGetter<bool, bool>(["param name", "row id", "field name"],
            "Returns the specific value specified by the exact param, row and field.", address =>
            {
                Param param = ParamBank.PrimaryBank.Params[address[0]];
                if (param == null)
                    throw new MEOperationException($@"Could not find param {address[0]}");
                var id = int.Parse(address[1]);
                (PseudoColumn, Param.Column) field = param.GetCol(address[2]);
                if (!field.IsColumnValid())
                    throw new MEOperationException($@"Could not find field {address[2]} in param {address[0]}");
                var row = param[id];
                if (row == null)
                    throw new MEOperationException($@"Could not find row {id} in param {address[0]}");
                var value = row.Get(field).ToParamEditorString();
                return (i, c, c2) => value;
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("average", newGetter<ParamBank, Param>(["field internalName", "row selector"],
            "Gives the mean value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, paramB, paramP) =>
            {
                (PseudoColumn, Param.Column) col = paramP.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new MEOperationException($@"Could not locate field {field[0]}");
                }

                Type colType = col.GetColumnType();
                if (colType == typeof(string) || colType == typeof(byte[]))
                {
                    throw new MEOperationException($@"Cannot average field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    SearchEngine.row.Search((paramB, paramP), field[1], false, false);
                IEnumerable<object> vals = rows.Select((row, i) => row.Item2.Get(col));
                var avg = vals.Average(val => Convert.ToDouble(val));
                return avg.ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("median", newGetter<ParamBank, Param>(["field internalName", "row selector"],
            "Gives the median value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, paramB, paramP) =>
            {
                (PseudoColumn, Param.Column) col = paramP.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new MEOperationException($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    SearchEngine.row.Search((paramB, paramP), field[1], false, false);
                IEnumerable<object> vals = rows.Select((row, i) => row.Item2.Get(col));
                var avg = vals.OrderBy(val => Convert.ToDouble(val)).ElementAt(vals.Count() / 2);
                return avg.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("mode", newGetter<ParamBank, Param>(["field internalName", "row selector"],
            "Gives the most common value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, paramB, paramP) =>
            {
                (PseudoColumn, Param.Column) col = paramP.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new MEOperationException($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    SearchEngine.row.Search((paramB, paramP), field[1], false, false);
                var avg = ParamUtils.GetParamValueDistribution(rows.Select((x, i) => x.Item2), col).OrderByDescending(g => g.Item2)
                    .First().Item1;
                return avg.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("min", newGetter<ParamBank, Param>(["field internalName", "row selector"],
            "Gives the smallest value from the cells/fields found using the given param, row selector and field",
            field => (i, paramB, paramP) =>
            {
                (PseudoColumn, Param.Column) col = paramP.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new MEOperationException($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    SearchEngine.row.Search((paramB, paramP), field[1], false, false);
                var min = rows.Min(r => r.Item2[field[0]].Value.Value);
                return min.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("max", newGetter<ParamBank, Param>(["field internalName", "row selector"],
            "Gives the largest value from the cells/fields found using the given param, row selector and field",
            field => (i, paramB, paramP) =>
            {
                (PseudoColumn, Param.Column) col = paramP.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new MEOperationException($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    SearchEngine.row.Search((paramB, paramP), field[1], false, false);
                var max = rows.Max(r => r.Item2[field[0]].Value.Value);
                return max.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("random", newGetter<bool, bool>(
            ["minimum number (inclusive)", "maximum number (exclusive)"],
            "Gives a random decimal number between the given values for each selected value", minAndMax =>
            {
                double min;
                double max;
                if (!double.TryParse(minAndMax[0], out min) || !double.TryParse(minAndMax[1], out max))
                {
                    throw new MEOperationException(@"Could not parse min and max random values");
                }

                if (max <= min)
                {
                    throw new MEOperationException(@"Random max must be greater than min");
                }

                var range = max - min;
                return (i, c, c2) => ((Random.Shared.NextDouble() * range) + min).ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("randint", newGetter<bool, bool>(
            ["minimum integer (inclusive)", "maximum integer (inclusive)"],
            "Gives a random integer between the given values for each selected value", minAndMax =>
            {
                int min;
                int max;
                if (!int.TryParse(minAndMax[0], out min) || !int.TryParse(minAndMax[1], out max))
                {
                    throw new MEOperationException(@"Could not parse min and max randint values");
                }

                if (max <= min)
                {
                    throw new MEOperationException(@"Random max must be greater than min");
                }

                return (i, c, c2) => Random.Shared.NextInt64(min, max + 1).ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("randFrom", newGetter<bool, bool>(["param name", "field internalName", "row selector"],
            "Gives a random value from the cells/fields found using the given param, row selector and field, for each selected value",
            paramFieldRowSelector =>
            {
                Param srcParam = ParamBank.PrimaryBank.Params[paramFieldRowSelector[0]];
                List<(string, Param.Row)> srcRows = SearchEngine.row.Search((ParamBank.PrimaryBank, srcParam),
                    paramFieldRowSelector[2], false, false);
                var values = srcRows.Select((r, i) => r.Item2[paramFieldRowSelector[1]].Value.Value).ToArray();
                return (i, c, c2) => values[Random.Shared.NextInt64(values.Length)].ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("paramIndex", newGetter<ParamBank, Param>([],
            "Gives an integer for the current selected param, beginning at 0 and increasing by 1 for each param selected",
            empty => (i, paramB, paramP) =>
            {
                return i.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("rowIndex", newGetter<string, Param.Row>([],
            "Gives an integer for the current selected row, beginning at 0 and increasing by 1 for each row selected",
            empty => (j, rowP, rowR) =>
            {
                return j.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("fieldIndex", newGetter<PseudoColumn, Param.Column>([],
            "Gives an integer for the current selected cell/field, beginning at 0 and increasing by 1 for each cell/field selected",
            empty => (k, colE, colC) =>
            {
                return k.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
    }

    internal List<(string, string[])> AllArguments()
    {
        List<(string, string[])> options = new();
        foreach (var op in argumentGetters.Keys)
        {
            options.Add((op, argumentGetters[op].args));
        }

        return options;
    }

    internal List<(string, string, string[])> VisibleArguments()
    {
        List<(string, string, string[])> options = new();
        foreach (var op in argumentGetters.Keys)
        {
            OperationArgumentGetter oag = argumentGetters[op];
            if (oag.shouldShow == null || oag.shouldShow())
            {
                options.Add((op, oag.wiki, oag.args));
            }
        }

        return options;
    }

    internal object[] getContextualArguments(int argumentCount, string opData)
    {
        var opArgs = opData == null ? [] : opData.Split(':', argumentCount);
        var contextualArgs = new object[opArgs.Length];
        for (var i = 0; i < opArgs.Length; i++)
        {
            contextualArgs[i] = getContextualArgumentFromArgs(opArgs[i]);
        }

        return contextualArgs;
    }

    internal object getContextualArgumentFromArgs(string opArg)
    {
        if (opArg.StartsWith('"') && opArg.EndsWith('"'))
        {
            return opArg.Substring(1, opArg.Length - 2);
        }

        if (opArg.StartsWith('$'))
        {
            opArg = MassParamEdit.massEditVars[opArg.Substring(1)].ToString();
        }

        var arg = opArg.Split(" ", 2);
        if (argumentGetters.ContainsKey(arg[0].Trim()))
        {
            OperationArgumentGetter getter = argumentGetters[arg[0]];
            var opArgArgs = arg.Length > 1 ? arg[1].Split(" ", getter.args.Length) : [];
            if (opArgArgs.Length != getter.args.Length)
            {
                throw new MEOperationException(
                    @$"Contextual value {arg[0]} has wrong number of arguments. Expected {opArgArgs.Length}");
            }

            for (var i = 0; i < opArgArgs.Length; i++)
            {
                if (opArgArgs[i].StartsWith('$'))
                {
                    opArgArgs[i] = MassParamEdit.massEditVars[opArgArgs[i].Substring(1)].ToString();
                }
            }

            return getter.func(opArgArgs);
        }

        return defaultGetter.func([opArg]);
    }
}

internal class OperationArgumentGetter
{
    internal string[] args;

    internal Func<string[], object> func;

    internal Func<bool> shouldShow;
    internal string wiki;

    internal OperationArgumentGetter(string[] args, string wiki,
        Func<string[], object>
            func, Func<bool> shouldShow)
    {
        this.args = args;
        this.wiki = wiki;
        this.func = func;
        this.shouldShow = shouldShow;
    }



}
internal static class OAGFuncExtension
{
    /*internal static object tryFold(this Func<object, object> func, object newContextInput)
    {
        Type t = newContextInput.GetType();
        if (func.Method.GetParameters()[0].ParameterType == t)
            return func(newContextInput);
        return func;
    }*/
    internal static object tryFoldAsFunc(this object maybeFunc, int editIndex, (object, object) newContextInput)
    {
        if (maybeFunc is not Delegate)
            return maybeFunc;
        Delegate func = (Delegate)maybeFunc;
        var parameters = func.Method.GetParameters();
        var a = newContextInput.Item1.GetType();
        var b = newContextInput.Item2.GetType();
        if (parameters.Length == 3 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == newContextInput.Item1.GetType() && parameters[2].ParameterType == newContextInput.Item2.GetType())
            return func.DynamicInvoke(editIndex, newContextInput.Item1, newContextInput.Item2);
        return func;
    }
    internal static object assertCompleteContextOrThrow(this object maybeFunc, int editIndex)
    {
        if (maybeFunc is Delegate)
        {
            Delegate func = (Delegate)maybeFunc;
            var parameters = func.Method.GetParameters();
            /* bool is provided as a special context argument for no context */
            if (parameters.Length == 2 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == typeof(bool))
                return assertCompleteContextOrThrow(func.DynamicInvoke(editIndex, false), editIndex);
            else
                throw new MEOperationException("Argument getter did not have enough context to determine the value to use.");
        }
        return maybeFunc;
    }
}
