using Andre.Formats;
using StudioCore.Editor;
using static Andre.Native.ImGuiBindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Octokit;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using DotNext.Collections.Generic;

namespace StudioCore.Editor.MassEdit;

internal class AutoFillSearchEngine
{
    private readonly string[] _autoFillArgs;
    private readonly SearchEngine engine;
    private readonly string id;
    private AutoFillSearchEngine _additionalCondition;
    private bool _autoFillNotToggle;
    private bool _useAdditionalCondition;

    internal AutoFillSearchEngine(SearchEngine searchEngine)
    {
        AutoFill._imguiID++;
        id = "e" + AutoFill._imguiID;
        engine = searchEngine;
        _autoFillArgs = Enumerable.Repeat("", engine.AllCommands().Sum(x => x.Item2.Length)).ToArray();
        _autoFillNotToggle = false;
        _useAdditionalCondition = false;
        _additionalCondition = null;
        AutoFill.autoFillersSE[searchEngine] = this;
    }
    private AutoFillSearchEngine(AutoFillSearchEngine parentEngine)
    {
        id = parentEngine.id + "+";
        engine = parentEngine.engine;
        _autoFillArgs = Enumerable.Repeat("", engine.AllCommands().Sum(x => x.Item2.Length)).ToArray();
        _autoFillNotToggle = false;
        _useAdditionalCondition = false;
        _additionalCondition = null;
    }
    internal string Menu(bool enableComplexToggles, bool enableDefault, string suffix, string inheritedCommand, bool terminal)
    {
        var currentArgIndex = 0;
        if (enableComplexToggles)
        {
            ImGui.Checkbox("Invert selection?##meautoinputnottoggle" + id, ref _autoFillNotToggle);
            ImGui.SameLine();
            ImGui.Checkbox("Add another condition?##meautoinputadditionalcondition" + id,
                ref _useAdditionalCondition);
        }

        if (_useAdditionalCondition && _additionalCondition == null)
        {
            _additionalCondition = new AutoFillSearchEngine(this);
        }
        else if (!_useAdditionalCondition)
        {
            _additionalCondition = null;
        }

        foreach ((string, string[], string) cmd in engine.VisibleCommands(enableDefault))
        {
            var argIndices = new int[cmd.Item2.Length];
            var valid = true;
            for (var i = 0; i < argIndices.Length; i++)
            {
                argIndices[i] = currentArgIndex;
                currentArgIndex++;
                if (string.IsNullOrEmpty(_autoFillArgs[argIndices[i]]))
                {
                    valid = false;
                }
            }

            string subResult = null;
            var wiki = cmd.Item3;
            UIHints.AddImGuiHintButton(cmd.Item1 == null ? "hintdefault" : "hint" + cmd.Item1, ref wiki, false,
                true);
            if (ImGui.BeginMenu(cmd.Item1 == null ? "Default filter..." : cmd.Item1, valid))
            {
                var curResult = inheritedCommand + getCurrentStepText(valid, cmd.Item1, argIndices, _additionalCondition != null ? " && " : suffix);
                if (_additionalCondition != null)
                {
                    subResult = _additionalCondition.Menu(enableComplexToggles, enableDefault, suffix, curResult, terminal);
                }
                else
                {
                    subResult = SubMenu(curResult, suffix);
                }

                ImGui.EndMenu();
            }
            /*
            else
            {
                subResult = ImGui.Selectable(cmd.Item1 == null ? "Default filter..." : cmd.Item1, false, valid ? ImGuiSelectableFlags.None : ImGuiSelectableFlags.Disabled) ? suffix : null;
            }*/

            //ImGui.Indent();
            for (var i = 0; i < argIndices.Length; i++)
            {
                if (i != 0)
                {
                    ImGui.SameLine();
                }

                ImGui.InputTextWithHint("##meautoinput" + argIndices[i], cmd.Item2[i],
                    ref _autoFillArgs[argIndices[i]], 256);
                var var = AutoFill.MassEditAutoFillForVars(argIndices[i]);
                if (var != null)
                {
                    _autoFillArgs[argIndices[i]] = var;
                }
            }

            //ImGui.Unindent();
            var currentResult = getCurrentStepText(valid, cmd.Item1, argIndices,
                _additionalCondition != null ? " && " : suffix);
            if (subResult != null && valid)
            {
                return currentResult + subResult;
            }
        }

        return null;
    }

    private string SubMenu(string inheritedCommand, string suffix)
    {
        if (inheritedCommand != null)
        {
            ImGui.TextColored(AutoFill.PREVIEWCOLOUR, inheritedCommand);
        }

        IEnumerable<AutoFillSearchEngine> afses = getChildAutoFillSearchEngines();
        string res1 = null;
        foreach (AutoFillSearchEngine afse in afses)
        {
            ImGui.Separator();
            ImGui.TextColored(AutoFill.HINTCOLOUR, "Select fields...");
            res1 = afse.Menu(true, true, ": ", inheritedCommand, false);
            if (res1 != null)
            {
                return res1;
            }
        }
        AutoFillOperation afo = getChildAutoFillOperation();
        if (afo != null)
        {
            ImGui.Separator();
            ImGui.TextColored(AutoFill.HINTCOLOUR, "Select row operation...");
            var res2 = afo.Menu(";");
            if (res2 != null)
            {
                return res2;
            }
        }

        return res1;
    }

    private AutoFillOperation getChildAutoFillOperation()
    {
        var nextOp = engine.NextOperation();
        if (nextOp == null)
            return null;
        return AutoFill.autoFillersO.GetValueOrDefault(nextOp);
    }

    private IEnumerable<AutoFillSearchEngine> getChildAutoFillSearchEngines()
    {
       return engine.NextSearchEngines().Select(x => AutoFill.autoFillersSE.GetValueOrDefault(x.Item1)).Where(x => x != null);
    }

    internal string getCurrentStepText(bool valid, string command, int[] argIndices, string suffixToUse)
    {
        if (!valid)
        {
            return null;
        }

        if (command != null)
        {
            var cmdText = _autoFillNotToggle ? '!' + command : command;
            for (var i = 0; i < argIndices.Length; i++)
            {
                cmdText += " " + _autoFillArgs[argIndices[i]];
            }

            return cmdText + suffixToUse;
        }

        if (argIndices.Length > 0)
        {
            var argText = _autoFillArgs[argIndices[0]];
            for (var i = 1; i < argIndices.Length; i++)
            {
                argText += " " + _autoFillArgs[argIndices[i]];
            }

            return argText + suffixToUse;
        }

        return null;
    }
}

internal class AutoFillOperation
{
    private readonly string[] _autoFillArgs;
    private readonly OperationCategory op;
    private readonly string id;

    internal AutoFillOperation(OperationCategory operation)
    {
        AutoFill._imguiID++;
        id = "e" + AutoFill._imguiID;
        op = operation;
        _autoFillArgs = Enumerable.Repeat("", op.AllCommands().Sum(x => x.Value.argNames.Length)).ToArray();
        AutoFill.autoFillersO[operation] = this;
    }

    internal string Menu(string suffix)
    {
        var currentArgIndex = 0;
        string result = null;
        foreach (KeyValuePair<string, METypelessOperationDef> cmd in op.AllCommands())
        {
            string cmdName = cmd.Key;
            METypelessOperationDef cmdData = cmd.Value;
            var argIndices = new int[cmdData.argNames.Length];
            var valid = true;
            for (var i = 0; i < argIndices.Length; i++)
            {
                argIndices[i] = currentArgIndex;
                currentArgIndex++;
                if (string.IsNullOrEmpty(_autoFillArgs[argIndices[i]]))
                {
                    valid = false;
                }
            }
            if (cmdData.shouldShow != null && !cmdData.shouldShow())
                continue;

            var wiki = cmdData.wiki;
            UIHints.AddImGuiHintButton(cmdName, ref wiki, false, true);
            result = ImGui.Selectable(cmdName, false,
                valid ? ImGuiSelectableFlags.None : ImGuiSelectableFlags.Disabled)
                ? suffix
                : null;

            ImGui.Indent();
            for (var i = 0; i < argIndices.Length; i++)
            {
                if (i != 0)
                {
                    ImGui.SameLine();
                }

                ImGui.InputTextWithHint("##meautoinputop" + argIndices[i], cmdData.argNames[i],
                    ref _autoFillArgs[argIndices[i]], 256);
                ImGui.SameLine();
                ImGui.Button($@"{ForkAwesome.CaretDown}");
                if (ImGui.BeginPopupContextItem("##meautoinputoapopup" + argIndices[i],
                        ImGuiPopupFlags.MouseButtonLeft))
                {
                    var opargResult = AutoFill.MassEditAutoFillForArguments();
                    if (opargResult != null)
                    {
                        _autoFillArgs[argIndices[i]] = opargResult;
                    }

                    ImGui.EndPopup();
                }
            }

            ImGui.Unindent();
            if (result != null && valid)
            {
                var argText = argIndices.Length > 0 ? _autoFillArgs[argIndices[0]] : null;
                for (var i = 1; i < argIndices.Length; i++)
                {
                    argText += ":" + _autoFillArgs[argIndices[i]];
                }

                result = cmdName + (argText != null ? " " + argText + result : result);
                return result;
            }
        }

        return result;
    }
}

internal class AutoFill
{
    internal static int _imguiID = 0;

    static AutoFill()
    {
        autoFillPrsse = new(SearchEngine.paramRowSelection);
        autoFillPrcse = new(SearchEngine.paramRowClipboard);
        autoFillPse = new(SearchEngine.param);
        autoFillRse = new(SearchEngine.row);
        autoFillCse = new(SearchEngine.cell);
        autoFillVse = new(SearchEngine.var);

        autoFillGo = new(OperationCategory.global);
        autoFillRo = new(OperationCategory.row);
        autoFillCo = new(OperationCategory.cell);
        autoFillVo = new(OperationCategory.var);
    }
    private static readonly AutoFillSearchEngine autoFillPrsse;
    private static readonly AutoFillSearchEngine autoFillPrcse;
    private static readonly AutoFillSearchEngine autoFillPse;
    private static readonly AutoFillSearchEngine autoFillRse;
    private static readonly AutoFillSearchEngine autoFillCse;
    private static readonly AutoFillSearchEngine autoFillVse;

    private static readonly AutoFillOperation autoFillGo;
    private static readonly AutoFillOperation autoFillRo;
    private static readonly AutoFillOperation autoFillCo;
    private static readonly AutoFillOperation autoFillVo;

    private static string[] _autoFillArgsOa = Enumerable.Repeat("", OperationArguments.arg.AllArguments().Sum(x => x.Item2.Length) + 1).ToArray();

    internal static Dictionary<SearchEngine, AutoFillSearchEngine> autoFillersSE = new();
    internal static Dictionary<OperationCategory, AutoFillOperation> autoFillersO = new();

    internal static Vector4 HINTCOLOUR = new(0.3f, 0.5f, 1.0f, 1.0f);
    internal static Vector4 PREVIEWCOLOUR = new(0.65f, 0.75f, 0.65f, 1.0f);

    public static string ParamSearchBarAutoFill()
    {
        ImGui.SameLine();
        ImGui.Button($@"{ForkAwesome.CaretDown}");
        if (ImGui.BeginPopupContextItem("##psbautoinputoapopup", ImGuiPopupFlags.MouseButtonLeft))
        {
            ImGui.TextColored(HINTCOLOUR, "Select params...");
            var result = autoFillPse.Menu(true, false, "", null, true);
            ImGui.EndPopup();
            return result;
        }

        return null;
    }

    public static string RowSearchBarAutoFill()
    {
        ImGui.SameLine();
        ImGui.Button($@"{ForkAwesome.CaretDown}");
        if (ImGui.BeginPopupContextItem("##rsbautoinputoapopup", ImGuiPopupFlags.MouseButtonLeft))
        {
            ImGui.TextColored(HINTCOLOUR, "Select rows...");
            var result = autoFillRse.Menu(true, false, "", null, true);
            ImGui.EndPopup();
            return result;
        }

        return null;
    }

    public static string ColumnSearchBarAutoFill()
    {
        ImGui.SameLine();
        ImGui.Button($@"{ForkAwesome.CaretDown}");
        if (ImGui.BeginPopupContextItem("##csbautoinputoapopup", ImGuiPopupFlags.MouseButtonLeft))
        {
            ImGui.TextColored(HINTCOLOUR, "Select fields...");
            var result = autoFillCse.Menu(true, false, "", null, true);
            ImGui.EndPopup();
            return result;
        }

        return null;
    }

    public static string MassEditCompleteAutoFill()
    {
        ImGui.TextUnformatted("Add command...");
        ImGui.SameLine();
        ImGui.Button($@"{ForkAwesome.CaretDown}");
        if (ImGui.BeginPopupContextItem("##meautoinputoapopup", ImGuiPopupFlags.MouseButtonLeft))
        {
            /*special case*/
            ImGui.PushID("paramrowselection");
            ImGui.TextColored(HINTCOLOUR, "Select param and rows...");
            var result0 = autoFillPrsse.Menu(false, false, ": ", null, false);/*, inheritedCommand =>
            {
                if (inheritedCommand != null)
                {
                    ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand);
                }

                ImGui.TextColored(HINTCOLOUR, "Select fields...");
                var res1 = autoFillCse.Menu(true, true, ": ", inheritedCommand, inheritedCommand2 =>
                {
                    if (inheritedCommand2 != null)
                    {
                        ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand2);
                    }

                    ImGui.TextColored(HINTCOLOUR, "Select field operation...");
                    return autoFillCo.Menu(";");
                });
                ImGui.Separator();
                ImGui.TextColored(HINTCOLOUR, "Select row operation...");
                var res2 = autoFillRo.Menu(";");
                if (res1 != null)
                {
                    return res1;
                }

                return res2;
            });*/
            ImGui.PopID();
            ImGui.PushID("paramrowclipboard");
            var result1 = autoFillPrcse.Menu(false, false, ": ", null, false);/*, inheritedCommand =>
            {
                if (inheritedCommand != null)
                {
                    ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand);
                }

                ImGui.TextColored(HINTCOLOUR, "Select fields...");
                var res1 = autoFillCse.Menu(true, true, ": ", inheritedCommand, inheritedCommand2 =>
                {
                    if (inheritedCommand2 != null)
                    {
                        ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand2);
                    }

                    ImGui.TextColored(HINTCOLOUR, "Select field operation...");
                    return autoFillCo.Menu(";");
                });
                ImGui.Separator();
                ImGui.TextColored(HINTCOLOUR, "Select row operation...");
                var res2 = autoFillRo.Menu(";");
                if (res1 != null)
                {
                    return res1;
                }

                return res2;
            });*/
            ImGui.PopID();
            /*end special case*/
            ImGui.Separator();
            ImGui.PushID("param");
            ImGui.TextColored(HINTCOLOUR, "Select params...");
            var result2 = autoFillPse.Menu(true, false, ": ", null, false);/*, inheritedCommand =>
            {
                if (inheritedCommand != null)
                {
                    ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand);
                }

                ImGui.TextColored(HINTCOLOUR, "Select rows...");
                return autoFillRse.Menu(true, false, ": ", inheritedCommand, inheritedCommand2 =>
                {
                    if (inheritedCommand2 != null)
                    {
                        ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand2);
                    }

                    ImGui.TextColored(HINTCOLOUR, "Select fields...");
                    var res1 = autoFillCse.Menu(true, true, ": ", inheritedCommand2, inheritedCommand3 =>
                    {
                        if (inheritedCommand3 != null)
                        {
                            ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand3);
                        }

                        ImGui.TextColored(HINTCOLOUR, "Select field operation...");
                        return autoFillCo.Menu(";");
                    });
                    string res2 = null;
                    if (CFG.Current.Param_AdvancedMassedit)
                    {
                        ImGui.Separator();
                        ImGui.TextColored(HINTCOLOUR, "Select row operation...");
                        res2 = autoFillRo.Menu(";");
                    }

                    if (res1 != null)
                    {
                        return res1;
                    }

                    return res2;
                });
            });
            ImGui.PopID();
            string result3 = null;
            string result4 = null;
            if (CFG.Current.Param_AdvancedMassedit)
            {
                ImGui.Separator();
                ImGui.PushID("globalop");
                ImGui.TextColored(HINTCOLOUR, "Select global operation...");
                result3 = autoFillGo.Menu(";");
                ImGui.PopID();
                if (MassParamEdit.massEditVars.Count != 0)
                {
                    ImGui.Separator();
                    ImGui.PushID("var");
                    ImGui.TextColored(HINTCOLOUR, "Select variables...");
                    result4 = autoFillVse.Menu(false, false, ": ", null, inheritedCommand =>
                    {
                        if (inheritedCommand != null)
                        {
                            ImGui.TextColored(PREVIEWCOLOUR, inheritedCommand);
                        }

                        ImGui.TextColored(HINTCOLOUR, "Select value operation...");
                        return autoFillVo.Menu(";");
                    });
                }
            }*/

            ImGui.EndPopup();
            if (result0 != null)
            {
                return result0;
            }
            if (result1 != null)
            {
                return result1;
            }
            if (result2 != null)
            {
                return result2;
            }

            /*if (result3 != null)
            {
                return result3;
            }

            return result4;*/
        }

        return null;
    }

    public static string MassEditOpAutoFill()
    {
        return autoFillCo.Menu(";");
    }

    internal static string MassEditAutoFillForArguments()
    {
        var currentArgIndex = 0;
        string result = null;
        foreach ((string, string, string[]) arg in OperationArguments.arg.VisibleArguments())
        {
            var argIndices = new int[arg.Item3.Length];
            var valid = true;
            for (var i = 0; i < argIndices.Length; i++)
            {
                argIndices[i] = currentArgIndex;
                currentArgIndex++;
                if (string.IsNullOrEmpty(_autoFillArgsOa[argIndices[i]]))
                {
                    valid = false;
                }
            }

            var selected = false;
            var wiki = arg.Item2;
            UIHints.AddImGuiHintButton(arg.Item1, ref wiki, false, true);
            if (ImGui.Selectable(arg.Item1, selected,
                    valid ? ImGuiSelectableFlags.None : ImGuiSelectableFlags.Disabled))
            {
                result = arg.Item1;
                var argText = "";
                for (var i = 0; i < argIndices.Length; i++)
                {
                    argText += " " + _autoFillArgsOa[argIndices[i]];
                }

                return arg.Item1 + argText;
            }

            ImGui.Indent();
            for (var i = 0; i < argIndices.Length; i++)
            {
                if (i != 0)
                {
                    ImGui.SameLine();
                }

                ImGui.InputTextWithHint("##meautoinputoa" + argIndices[i], arg.Item3[i],
                    ref _autoFillArgsOa[argIndices[i]], 256);
                var var = MassEditAutoFillForVars(argIndices[i]);
                if (var != null)
                {
                    _autoFillArgsOa[argIndices[i]] = var;
                }
            }

            ImGui.Unindent();
        }

        if (MassParamEdit.massEditVars.Count != 0)
        {
            ImGui.Separator();
            ImGui.TextUnformatted("Defined variables...");
            foreach (KeyValuePair<string, object> pair in MassParamEdit.massEditVars)
            {
                if (ImGui.Selectable(pair.Key + "(" + pair.Value + ")"))
                {
                    return '$' + pair.Key;
                }
            }
        }

        ImGui.Separator();
        if (ImGui.Selectable("Exactly..."))
        {
            result = '"' + _autoFillArgsOa[_autoFillArgsOa.Length-1] + '"';
        }

        ImGui.InputTextWithHint("##meautoinputoaExact", "literal value...", ref _autoFillArgsOa[_autoFillArgsOa.Length-1], 256);
        return result;
    }

    internal static string MassEditAutoFillForVars(int id)
    {
        if (MassParamEdit.massEditVars.Count == 0)
        {
            return null;
        }

        ImGui.SameLine();
        ImGui.Button("$");
        if (ImGui.BeginPopupContextItem("##meautoinputvarpopup" + id, ImGuiPopupFlags.MouseButtonLeft))
        {
            ImGui.TextUnformatted("Defined variables...");
            ImGui.Separator();
            foreach (KeyValuePair<string, object> pair in MassParamEdit.massEditVars)
            {
                if (ImGui.Selectable(pair.Key + "(" + pair.Value + ")"))
                {
                    ImGui.EndPopup();
                    return '$' + pair.Key;
                }
            }

            ImGui.EndPopup();
        }

        return null;
    }
}
