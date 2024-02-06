#nullable enable
using Andre.Formats;
using StudioCore.Editor;
using StudioCore.ParamEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StudioCore.Editor.MassEdit;

internal abstract class METypelessOperationDef
{
    internal string[] argNames;
    internal string wiki;
    internal Func<object, string[], object> function;
}
internal class MEOperationDef<T, O> : METypelessOperationDef
{
    internal MEOperationDef(string[] args, string tooltip, Func<T, string[], O> func)
    {
        argNames = args;
        wiki = tooltip;
        function = func as Func<object, string[], object>;
    }
}
public abstract class METypelessOperation
{
    private static Dictionary<Type, METypelessOperation> editOperations = new();
    internal static void AddEditOperation<I, O>(MEOperation<I, O> engine)
    {
        editOperations[typeof(I)] = engine;
    }
    internal static METypelessOperation GetEditOperation(Type t)
    {
        return editOperations.GetValueOrDefault(t);
    }

    internal abstract Dictionary<string, METypelessOperationDef> AllCommands();
    internal abstract string NameForHelpTexts();
}
public class MEOperation<T, O> : METypelessOperation
{
    internal Dictionary<string, METypelessOperationDef> operations = new();
    internal string name = "[Unnamed operation type]";

    internal MEOperation()
    {
        Setup();
    }

    internal virtual void Setup()
    {
    }

    internal bool HandlesCommand(string command)
    {
        return operations.ContainsKey(command);
    }
    internal override Dictionary<string, METypelessOperationDef> AllCommands()
    {
        return operations;
    }

    public List<(string, string[], string)> AvailableCommands()
    {
        List<(string, string[], string)> options = new();
        foreach (var op in operations.Keys)
        {
            options.Add((op, operations[op].argNames, operations[op].wiki));
        }

        return options;
    }
    internal MEOperationDef<T, O> newCmd(string[] args, string wiki, Func<T, string[], O> func)
    {
        return new MEOperationDef<T, O>(args, wiki, func);
    }
    internal MEOperationDef<T, O> newCmd(string wiki, Func<T, string[], O> func)
    {
        return new MEOperationDef<T, O>(Array.Empty<string>(), wiki, func);
    }

    internal override string NameForHelpTexts()
    {
        return name;
    }
}

public class MEGlobalOperation : MEOperation<ParamEditorSelectionState, bool>
{
    public static MEGlobalOperation globalOps = new();

    internal override void Setup()
    {
        name = "global";
        operations.Add("clear", newCmd(new string[0], "Clears clipboard param and rows", (selectionState, args) =>
        {
            ParamBank.ClipboardParam = null;
            ParamBank.ClipboardRows.Clear();
            return true;
        }
        ));
        operations.Add("newvar", newCmd(new[] { "variable name", "value" },
            "Creates a variable with the given value, and the type of that value", (selectionState, args) =>
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
            }
        ));
        operations.Add("clearvars", newCmd(new string[0], "Deletes all variables", (selectionState, args) =>
        {
            MassParamEdit.massEditVars.Clear();
            return true;
        }
        ));
    }
}

public class MERowOperation : MEOperation<(string, Param.Row), (Param, Param.Row)>
{
    public static MERowOperation rowOps = new();

    internal override void Setup()
    {
        name = "row";
        operations.Add("copy", newCmd(new string[0],
            "Adds the selected rows into clipboard. If the clipboard param is different, the clipboard is emptied first",
            (paramAndRow, args) =>
            {
                var paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                {
                    throw new Exception(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new Exception($@"Could not locate param {paramKey}");
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
        ));
        operations.Add("copyN", newCmd(new[] { "count" },
            "Adds the selected rows into clipboard the given number of times. If the clipboard param is different, the clipboard is emptied first",
            (paramAndRow, args) =>
            {
                var paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                {
                    throw new Exception(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new Exception($@"Could not locate param {paramKey}");
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
            }
        ));
        operations.Add("paste", newCmd(new string[0],
            "Adds the selected rows to the primary regulation or parambnd in the selected param",
            (paramAndRow, args) =>
            {
                var paramKey = paramAndRow.Item1;
                Param.Row row = paramAndRow.Item2;
                if (paramKey == null)
                {
                    throw new Exception(@"Could not locate param");
                }

                if (!ParamBank.PrimaryBank.Params.ContainsKey(paramKey))
                {
                    throw new Exception($@"Could not locate param {paramKey}");
                }

                Param p = ParamBank.PrimaryBank.Params[paramKey];
                return (p, new Param.Row(row, p));
            }
        ));
    }
}

public class MEValueOperation : MEOperation<object, object>
{
    public static MEValueOperation valueOps = new();

    internal override void Setup()
    {
        name = "value";
        operations.Add("=",
            newCmd(new[] { "number or text" },
                "Assigns the given value to the selected values. Will attempt conversion to the value's data type",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => args[0])));
        operations.Add("+", newCmd(new[] { "number or text" },
            "Adds the number to the selected values, or appends text if that is the data type of the values",
            (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v =>
            {
                double val;
                if (double.TryParse(args[0], out val))
                {
                    return v + val;
                }

                return v + args[0];
            })));
        operations.Add("-",
            newCmd(new[] { "number" }, "Subtracts the number from the selected values",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v - double.Parse(args[0]))));
        operations.Add("*",
            newCmd(new[] { "number" }, "Multiplies selected values by the number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v * double.Parse(args[0]))));
        operations.Add("/",
            newCmd(new[] { "number" }, "Divides the selected values by the number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v / double.Parse(args[0]))));
        operations.Add("%",
            newCmd(new[] { "number" }, "Gives the remainder when the selected values are divided by the number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v % double.Parse(args[0]))));
        operations.Add("scale", newCmd(new[] { "factor number", "center number" },
            "Multiplies the difference between the selected values and the center number by the factor number",
            (ctx, args) =>
            {
                var opp1 = double.Parse(args[0]);
                var opp2 = double.Parse(args[1]);
                return MassParamEdit.WithDynamicOf(ctx, v =>
                {
                    return ((v - opp2) * opp1) + opp2;
                });
            }
        ));
        operations.Add("replace",
            newCmd(new[] { "text to replace", "new text" },
                "Interprets the selected values as text and replaces all occurances of the text to replace with the new text",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => v.Replace(args[0], args[1]))));
        operations.Add("replacex", newCmd(new[] { "text to replace (regex)", "new text (w/ groups)" },
            "Interprets the selected values as text and replaces all occurances of the given regex with the replacement, supporting regex groups",
            (ctx, args) =>
            {
                Regex rx = new(args[0]);
                return MassParamEdit.WithDynamicOf(ctx, v => rx.Replace(v, args[1]));
            }
        ));
        operations.Add("max",
            newCmd(new[] { "number" }, "Returns the larger of the current value and number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => Math.Max(v, double.Parse(args[0])))));
        operations.Add("min",
            newCmd(new[] { "number" }, "Returns the smaller of the current value and number",
                (ctx, args) => MassParamEdit.WithDynamicOf(ctx, v => Math.Min(v, double.Parse(args[0])))));
    }
}

public class MEOperationArgument
{
    public static MEOperationArgument arg = new();
    private readonly Dictionary<string, OperationArgumentGetter> argumentGetters = new();
    private OperationArgumentGetter defaultGetter;

    private MEOperationArgument()
    {
        Setup();
    }
    private OperationArgumentGetter newGetter<P>(string[] args, string wiki,
        Func<string[], Func<int, P, object>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }
    private OperationArgumentGetter newGetter<P, R>(string[] args, string wiki,
        Func<string[], Func<int, P, Func<int, R, object>>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }
    private OperationArgumentGetter newGetter<P, R, C>(string[] args, string wiki,
        Func<string[], Func<int, P, Func<int, R, Func<int, C, object>>>>
            func, Func<bool> shouldShow = null)
    {
        return new OperationArgumentGetter(args, wiki, func, shouldShow);
    }

    private void Setup()
    {
        defaultGetter = newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new string[0], "Gives the specified value",
            value => (i, param) => (j, row) => (k, col) => value[0]);
        argumentGetters.Add("self", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new string[0], "Gives the value of the currently selected value",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return row.Get(col).ToParamEditorString();
            }));
        argumentGetters.Add("field", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName" },
            "Gives the value of the given cell/field for the currently selected row and param", field =>
                (i, param) =>
                {
                    (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                    if (!col.IsColumnValid())
                    {
                        throw new Exception($@"Could not locate field {field[0]}");
                    }

                    return (j, row) =>
                    {
                        var v = row.Get(col).ToParamEditorString();
                        return (k, c) => v;
                    };
                }));
        argumentGetters.Add("vanilla", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new string[0],
            "Gives the value of the equivalent cell/field in the vanilla regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added",
            empty =>
            {
                ParamBank bank = ParamBank.VanillaBank;
                return (i, param) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    return (j, row) =>
                    {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                        {
                            throw new Exception($@"Could not locate vanilla row {row.ID}");
                        }

                        return (k, col) =>
                        {
                            if (col.Item1 == PseudoColumn.None && col.Item2 == null)
                            {
                                throw new Exception(@"Could not locate given field or property");
                            }

                            return vRow.Get(col).ToParamEditorString();
                        };
                    };
                };
            }));
        argumentGetters.Add("aux", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "parambank name" },
            "Gives the value of the equivalent cell/field in the specified regulation or parambnd for the currently selected cell/field, row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*",
            bankName =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(bankName[0]))
                {
                    throw new Exception($@"Could not locate paramBank {bankName[0]}");
                }

                ParamBank bank = ParamBank.AuxBanks[bankName[0]];
                return (i, param) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    return (j, row) =>
                    {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                        {
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        }

                        return (k, col) =>
                        {
                            if (!col.IsColumnValid())
                            {
                                throw new Exception(@"Could not locate given field or property");
                            }

                            return vRow.Get(col).ToParamEditorString();
                        };
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        argumentGetters.Add("vanillafield", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName" },
            "Gives the value of the specified cell/field in the vanilla regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have a vanilla equivilent. Consider using && !added",
            field => (i, param) =>
            {
                var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                Param? vParam = ParamBank.VanillaBank.GetParamFromName(paramName);
                if (vParam == null)
                {
                    throw new Exception($@"Could not locate vanilla param for {param.ParamType}");
                }

                (PseudoColumn, Param.Column) col = vParam.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                return (j, row) =>
                {
                    Param.Row vRow = vParam?[row.ID];
                    if (vRow == null)
                    {
                        throw new Exception($@"Could not locate vanilla row {row.ID}");
                    }

                    var v = vRow.Get(col).ToParamEditorString();
                    return (k, c) => v;
                };
            }));
        argumentGetters.Add("auxfield", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "parambank name", "field internalName" },
            "Gives the value of the specified cell/field in the specified regulation or parambnd for the currently selected row and param.\nWill fail if a row does not have an aux equivilent. Consider using && auxprop ID .*",
            bankAndField =>
            {
                if (!ParamBank.AuxBanks.ContainsKey(bankAndField[0]))
                {
                    throw new Exception($@"Could not locate paramBank {bankAndField[0]}");
                }

                ParamBank bank = ParamBank.AuxBanks[bankAndField[0]];
                return (i, param) =>
                {
                    var paramName = ParamBank.PrimaryBank.GetKeyForParam(param);
                    if (!bank.Params.ContainsKey(paramName))
                    {
                        throw new Exception($@"Could not locate aux param for {param.ParamType}");
                    }

                    Param vParam = bank.Params[paramName];
                    (PseudoColumn, Param.Column) col = vParam.GetCol(bankAndField[1]);
                    if (!col.IsColumnValid())
                    {
                        throw new Exception($@"Could not locate field {bankAndField[1]}");
                    }

                    return (j, row) =>
                    {
                        Param.Row vRow = vParam?[row.ID];
                        if (vRow == null)
                        {
                            throw new Exception($@"Could not locate aux row {row.ID}");
                        }

                        var v = vRow.Get(col).ToParamEditorString();
                        return (k, c) => v;
                    };
                };
            }, () => ParamBank.AuxBanks.Count > 0));
        argumentGetters.Add("paramlookup", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "param name", "row id", "field name" },
            "Returns the specific value specified by the exact param, row and field.", address =>
            {
                Param param = ParamBank.PrimaryBank.Params[address[0]];
                if (param == null)
                    throw new Exception($@"Could not find param {address[0]}");
                var id = int.Parse(address[1]);
                (PseudoColumn, Param.Column) field = param.GetCol(address[2]);
                if (!field.IsColumnValid())
                    throw new Exception($@"Could not find field {address[2]} in param {address[0]}");
                var row = param[id];
                if (row == null)
                    throw new Exception($@"Could not find row {id} in param {address[0]}");
                var value = row.Get(field).ToParamEditorString();
                return (i, param) => (j, row) => (k, col) => value;
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("average", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName", "row selector" },
            "Gives the mean value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                Type colType = col.GetColumnType();
                if (colType == typeof(string) || colType == typeof(byte[]))
                {
                    throw new Exception($@"Cannot average field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                IEnumerable<object> vals = rows.Select((row, i) => row.Item2.Get(col));
                var avg = vals.Average(val => Convert.ToDouble(val));
                return (j, row) => (k, c) => avg.ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("median", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName", "row selector" },
            "Gives the median value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                IEnumerable<object> vals = rows.Select((row, i) => row.Item2.Get(col));
                var avg = vals.OrderBy(val => Convert.ToDouble(val)).ElementAt(vals.Count() / 2);
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("mode", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName", "row selector" },
            "Gives the most common value of the cells/fields found using the given selector, for the currently selected param",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var avg = ParamUtils.GetParamValueDistribution(rows.Select((x, i) => x.Item2), col).OrderByDescending(g => g.Item2)
                    .First().Item1;
                return (j, row) => (k, c) => avg.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("min", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName", "row selector" },
            "Gives the smallest value from the cells/fields found using the given param, row selector and field",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var min = rows.Min(r => r.Item2[field[0]].Value.Value);
                return (j, row) => (k, c) => min.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("max", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "field internalName", "row selector" },
            "Gives the largest value from the cells/fields found using the given param, row selector and field",
            field => (i, param) =>
            {
                (PseudoColumn, Param.Column) col = param.GetCol(field[0]);
                if (!col.IsColumnValid())
                {
                    throw new Exception($@"Could not locate field {field[0]}");
                }

                List<(string, Param.Row)>? rows =
                    RowSearchEngine.rse.Search((ParamBank.PrimaryBank, param), field[1], false, false);
                var max = rows.Max(r => r.Item2[field[0]].Value.Value);
                return (j, row) => (k, c) => max.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("random", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(
            new[] { "minimum number (inclusive)", "maximum number (exclusive)" },
            "Gives a random decimal number between the given values for each selected value", minAndMax =>
            {
                double min;
                double max;
                if (!double.TryParse(minAndMax[0], out min) || !double.TryParse(minAndMax[1], out max))
                {
                    throw new Exception(@"Could not parse min and max random values");
                }

                if (max <= min)
                {
                    throw new Exception(@"Random max must be greater than min");
                }

                var range = max - min;
                return (i, param) => (j, row) => (k, c) => ((Random.Shared.NextDouble() * range) + min).ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("randint", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(
            new[] { "minimum integer (inclusive)", "maximum integer (inclusive)" },
            "Gives a random integer between the given values for each selected value", minAndMax =>
            {
                int min;
                int max;
                if (!int.TryParse(minAndMax[0], out min) || !int.TryParse(minAndMax[1], out max))
                {
                    throw new Exception(@"Could not parse min and max randint values");
                }

                if (max <= min)
                {
                    throw new Exception(@"Random max must be greater than min");
                }

                return (i, param) => (j, row) => (k, c) => Random.Shared.NextInt64(min, max + 1).ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("randFrom", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new[] { "param name", "field internalName", "row selector" },
            "Gives a random value from the cells/fields found using the given param, row selector and field, for each selected value",
            paramFieldRowSelector =>
            {
                Param srcParam = ParamBank.PrimaryBank.Params[paramFieldRowSelector[0]];
                List<(string, Param.Row)> srcRows = RowSearchEngine.rse.Search((ParamBank.PrimaryBank, srcParam),
                    paramFieldRowSelector[2], false, false);
                var values = srcRows.Select((r, i) => r.Item2[paramFieldRowSelector[1]].Value.Value).ToArray();
                return (i, param) =>
                    (j, row) => (k, c) => values[Random.Shared.NextInt64(values.Length)].ToString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("paramIndex", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new string[0],
            "Gives an integer for the current selected param, beginning at 0 and increasing by 1 for each param selected",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return i.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("rowIndex", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new string[0],
            "Gives an integer for the current selected row, beginning at 0 and increasing by 1 for each row selected",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return j.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
        argumentGetters.Add("fieldIndex", newGetter<Param, Param.Row, (PseudoColumn, Param.Column)>(new string[0],
            "Gives an integer for the current selected cell/field, beginning at 0 and increasing by 1 for each cell/field selected",
            empty => (i, param) => (j, row) => (k, col) =>
            {
                return k.ToParamEditorString();
            }, () => CFG.Current.Param_AdvancedMassedit));
    }

    public List<(string, string[])> AllArguments()
    {
        List<(string, string[])> options = new();
        foreach (var op in argumentGetters.Keys)
        {
            options.Add((op, argumentGetters[op].args));
        }

        return options;
    }

    public List<(string, string, string[])> VisibleArguments()
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
        var opArgs = opData == null ? new string[0] : opData.Split(':', argumentCount);
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
            var opArgArgs = arg.Length > 1 ? arg[1].Split(" ", getter.args.Length) : new string[0];
            if (opArgArgs.Length != getter.args.Length)
            {
                throw new Exception(
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

        return defaultGetter.func(new[] { opArg });
    }
}

internal class OperationArgumentGetter
{
    public string[] args;

    internal Func<string[], object> func;

    internal Func<bool> shouldShow;
    public string wiki;

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
public static class OAGFuncExtension
{
    /*internal static object tryFold(this Func<object, object> func, object newContextInput)
    {
        Type t = newContextInput.GetType();
        if (func.Method.GetParameters()[0].ParameterType == t)
            return func(newContextInput);
        return func;
    }*/
    public static object tryFoldAsFunc(this object maybeFunc, int editIndex, object newContextInput)
    {
        if (maybeFunc is not Delegate)
            return maybeFunc;
        Delegate func = (Delegate)maybeFunc;
        var parameters = func.Method.GetParameters();
        if (parameters.Length == 2 && parameters[0].ParameterType == typeof(int) && parameters[1].ParameterType == newContextInput.GetType())
            return func.DynamicInvoke(editIndex, newContextInput);
        return func;
    }
    public static object assertCompleteContextOrThrow(this object maybeFunc)
    {
        if (maybeFunc is Delegate)
            throw new Exception("Argument getter did not have enough context to complete.");
        return maybeFunc;
    }
}
