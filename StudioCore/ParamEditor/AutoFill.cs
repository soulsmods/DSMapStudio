using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FSParam;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;
using SoulsFormats;
using StudioCore;
using StudioCore.TextEditor;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using ActionManager = StudioCore.Editor.ActionManager;
using AddParamsAction = StudioCore.Editor.AddParamsAction;
using CompoundAction = StudioCore.Editor.CompoundAction;
using DeleteParamsAction = StudioCore.Editor.DeleteParamsAction;
using EditorScreen = StudioCore.Editor.EditorScreen;

namespace StudioCore.ParamEditor
{
    public class AutoFill
    {

        private static string[] _autoFillArgsParse = Enumerable.Repeat("", ParamAndRowSearchEngine.parse.AvailableCommands().Sum((x) => x.Item2.Length) + ParamAndRowSearchEngine.parse.defaultFilter.Item1.Length).ToArray();
        private static bool _autoFillNotToggleParse = false;
        private static string[] _autoFillArgsPse = Enumerable.Repeat("", ParamSearchEngine.pse.AvailableCommands().Sum((x) => x.Item2.Length) + ParamSearchEngine.pse.defaultFilter.Item1.Length).ToArray();
        private static bool _autoFillNotTogglePse = false;
        private static string[] _autoFillArgsRse = Enumerable.Repeat("", RowSearchEngine.rse.AvailableCommands().Sum((x) => x.Item2.Length) + RowSearchEngine.rse.defaultFilter.Item1.Length).ToArray();
        private static bool _autoFillNotToggleRse = false;
        private static string[] _autoFillArgsCse = Enumerable.Repeat("", CellSearchEngine.cse.AvailableCommands().Sum((x) => x.Item2.Length) + CellSearchEngine.cse.defaultFilter.Item1.Length).ToArray();
        private static bool _autoFillNotToggleCse = false;
        private static string[] _autoFillArgsRop = Enumerable.Repeat("", MERowOperation.rowOps.AvailableCommands().Sum((x) => x.Item2.Length)).ToArray();
        private static string[] _autoFillArgsCop = Enumerable.Repeat("", MECellOperation.cellOps.AvailableCommands().Sum((x) => x.Item2.Length)).ToArray();
        private static string[] _autoFillArgsOa = Enumerable.Repeat("", MEOperationArgument.arg.AvailableArguments().Sum((x) => x.Item2.Length)).ToArray();
        
        public static string ParamSearchBarAutoFill()
        {
            ImGui.SameLine();
            ImGui.Button($@"{ForkAwesome.CaretDown}");
            if (ImGui.BeginPopupContextItem("##rsbautoinputoapopup", ImGuiPopupFlags.MouseButtonLeft))
            {
                ImGui.TextUnformatted("Select params...");
                var result = MassEditAutoFillForSearchEngine(ParamSearchEngine.pse, ref _autoFillArgsPse, ref _autoFillNotTogglePse, false, "", null);
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
                ImGui.TextUnformatted("Select rows...");
                var result = MassEditAutoFillForSearchEngine(RowSearchEngine.rse, ref _autoFillArgsRse, ref _autoFillNotToggleRse, false, "", null);
                ImGui.EndPopup();
                return result;
            }
            return null;
        }

        public static string MassEditAutoFill()
        {
            ImGui.TextUnformatted("Add command...");
            ImGui.SameLine();
            ImGui.Button($@"{ForkAwesome.CaretDown}");
            if (ImGui.BeginPopupContextItem("##meautoinputoapopup", ImGuiPopupFlags.MouseButtonLeft))
            {
                ImGui.TextUnformatted("Select param and rows...");
                string result1 = MassEditAutoFillForSearchEngine(ParamAndRowSearchEngine.parse, ref _autoFillArgsParse, ref _autoFillNotToggleParse, false, ": ", () => 
                {
                    ImGui.TextUnformatted("Select fields...");
                    string res1 = MassEditAutoFillForSearchEngine(CellSearchEngine.cse, ref _autoFillArgsCse, ref _autoFillNotToggleCse, true, ": ", () => 
                    {
                        ImGui.TextUnformatted("Select field operation...");
                        return MassEditAutoFillForOperation(MECellOperation.cellOps, ref _autoFillArgsCop, ";", null);
                    });
                    ImGui.Separator();
                    ImGui.TextUnformatted("Select row operation...");
                    string res2 = MassEditAutoFillForOperation(MERowOperation.rowOps, ref _autoFillArgsRop, ";", null);
                    if (res1 != null)
                        return res1;
                    return res2;
                });
                ImGui.Separator();
                ImGui.TextUnformatted("Select params...");
                string result2 = MassEditAutoFillForSearchEngine(ParamSearchEngine.pse, ref _autoFillArgsPse, ref _autoFillNotTogglePse, false, ": ", () =>
                {
                    ImGui.TextUnformatted("Select rows...");
                    return MassEditAutoFillForSearchEngine(RowSearchEngine.rse, ref _autoFillArgsRse, ref _autoFillNotToggleRse, false, ": ", () => 
                    {
                        ImGui.TextUnformatted("Select fields...");
                        string res1 = MassEditAutoFillForSearchEngine(CellSearchEngine.cse, ref _autoFillArgsCse, ref _autoFillNotToggleCse, true, ": ", () => 
                        {
                            ImGui.TextUnformatted("Select field operation...");
                            return MassEditAutoFillForOperation(MECellOperation.cellOps, ref _autoFillArgsCop, ";", null);
                        });
                        ImGui.Separator();
                        ImGui.TextUnformatted("Select row operation...");
                        string res2 = MassEditAutoFillForOperation(MERowOperation.rowOps, ref _autoFillArgsRop, ";", null);
                        if (res1 != null)
                            return res1;
                        return res2;
                    });
                });
                ImGui.EndPopup();
                if (result1 != null)
                    return result1;
                return result2;
            }
            return null;
        }

        private static string MassEditAutoFillForSearchEngine<A, B> (SearchEngine<A, B> se, ref string[] staticArgs, ref bool notToggle, bool enableDefault, string suffix, Func<string> subMenu)
        {
            int currentArgIndex = 0;
            string result = null;
            ImGui.Checkbox("Invert selection?##meautoinputnottoggle", ref notToggle);
            foreach (var cmd in enableDefault ? se.AvailableCommands().Append((null, se.defaultFilter.Item1)).ToList() : se.AvailableCommands())
            {
                int[] argIndices = new int[cmd.Item2.Length];
                bool valid = true;
                for (int i = 0; i < argIndices.Length; i++)
                {
                    argIndices[i] = currentArgIndex;
                    currentArgIndex++;
                    if (string.IsNullOrEmpty(staticArgs[argIndices[i]]))
                        valid = false;
                }
                if (subMenu != null)
                {
                    if (ImGui.BeginMenu(cmd.Item1 == null ? "Default filter..." : cmd.Item1, valid))
                    {
                        result = subMenu();
                        ImGui.EndMenu();
                    }
                }
                else
                {
                    result = ImGui.Selectable(cmd.Item1 == null ? "Default filter..." : cmd.Item1, false, valid ? ImGuiSelectableFlags.None : ImGuiSelectableFlags.Disabled) ? suffix : null;
                }
                //ImGui.Indent();
                for (int i = 0; i < argIndices.Length; i++)
                {
                    if (i != 0)
                        ImGui.SameLine();
                    ImGui.InputTextWithHint("##meautoinput"+argIndices[i], cmd.Item2[i], ref staticArgs[argIndices[i]], 256);
                }
                //ImGui.Unindent();
                if (result != null && valid)
                {
                    if (cmd.Item1 != null)
                    {
                        string cmdText = notToggle ? '!' + cmd.Item1 : cmd.Item1;
                        for (int i = 0; i < argIndices.Length; i++)
                            cmdText += " " + staticArgs[argIndices[i]];
                        result = cmdText + suffix + result;
                    }
                    else if (argIndices.Length > 0)
                    {
                        string argText = staticArgs[argIndices[0]];
                        for (int i = 1; i < argIndices.Length; i++)
                            argText += " " + staticArgs[argIndices[i]];
                        result = argText + suffix + result;
                    }
                    return result;
                }
            }
            return result;
        }
        private static string MassEditAutoFillForOperation<A, B> (MEOperation<A, B> ops, ref string[] staticArgs, string suffix, Func<string> subMenu)
        {
            int currentArgIndex = 0;
            string result = null;
            foreach (var cmd in ops.AvailableCommands())
            {
                int[] argIndices = new int[cmd.Item2.Length];
                bool valid = true;
                for (int i = 0; i < argIndices.Length; i++)
                {
                    argIndices[i] = currentArgIndex;
                    currentArgIndex++;
                    if (string.IsNullOrEmpty(staticArgs[argIndices[i]]))
                        valid = false;
                }
                if (subMenu != null)
                {
                    if (ImGui.BeginMenu(cmd.Item1, valid))
                    {
                        result = subMenu();
                        ImGui.EndMenu();
                    }
                }
                else
                {
                    result = ImGui.Selectable(cmd.Item1, false, valid ? ImGuiSelectableFlags.None : ImGuiSelectableFlags.Disabled) ? suffix : null;
                }
                ImGui.Indent();
                for (int i = 0; i < argIndices.Length; i++)
                {
                    if (i != 0)
                        ImGui.SameLine();
                    ImGui.InputTextWithHint("##meautoinputop"+argIndices[i], cmd.Item2[i], ref staticArgs[argIndices[i]], 256);
                    ImGui.SameLine();
                    ImGui.Button($@"{ForkAwesome.CaretDown}");
                    if (ImGui.BeginPopupContextItem("##meautoinputoapopup"+argIndices[i], ImGuiPopupFlags.MouseButtonLeft))
                    {
                        string opargResult = MassEditAutoFillForArguments(MEOperationArgument.arg, ref _autoFillArgsOa);
                        if (opargResult != null)
                            staticArgs[argIndices[i]] = opargResult;
                        ImGui.EndPopup();
                    }

                }
                ImGui.Unindent();
                if (result != null && valid)
                {
                    string argText = argIndices.Length > 0 ? staticArgs[argIndices[0]] : null;
                    for (int i = 1; i < argIndices.Length; i++)
                        argText += ":" + staticArgs[argIndices[i]];
                    result = cmd.Item1 + (argText != null ? (" " + argText + result) : result);
                    return result;
                }
            }
            return result;
        }

        private static string MassEditAutoFillForArguments(MEOperationArgument oa, ref string[] staticArgs)
        {
            int currentArgIndex = 0;
            string result = null;
            foreach (var arg in oa.AvailableArguments())
            {
                int[] argIndices = new int[arg.Item2.Length];
                bool valid = true;
                for (int i = 0; i < argIndices.Length; i++)
                {
                    argIndices[i] = currentArgIndex;
                    currentArgIndex++;
                    if (string.IsNullOrEmpty(staticArgs[argIndices[i]]))
                        valid = false;
                }
                bool selected = false;
                if (ImGui.Selectable(arg.Item1, selected, valid ? ImGuiSelectableFlags.None : ImGuiSelectableFlags.Disabled))
                {
                    result = arg.Item1;
                    string argText = "";
                    for (int i = 0; i < argIndices.Length; i++)
                        argText += " " + staticArgs[argIndices[i]];
                    return arg.Item1 + argText;
                }
                ImGui.Indent();
                for (int i = 0; i < argIndices.Length; i++)
                {
                    if (i != 0)
                        ImGui.SameLine();
                    ImGui.InputTextWithHint("##meautoinputoa"+argIndices[i], arg.Item2[i], ref staticArgs[argIndices[i]], 256);
                }
                ImGui.Unindent();
            }
            return result;
        }
    }
}
