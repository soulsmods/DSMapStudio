using static Andre.Native.ImGuiBindings;
using SoapstoneLib;
using StudioCore.Editor;
using StudioCore.MsbEditor;
using StudioCore.ParamEditor;
using StudioCore.Scene;
using StudioCore.TextEditor;
using StudioCore.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using Veldrid;
using StudioCore.Editor;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace StudioCore;

public class SettingsMenu
{
    private KeyBind _currentKeyBind;
    public bool MenuOpenState;
    public ModelEditorScreen ModelEditor;
    public MsbEditorScreen MsbEditor;
    public ParamEditorScreen ParamEditor;
    public ProjectSettings? ProjSettings = null;
    public TextEditorScreen TextEditor;
    private float _tempUiScale;

    public SettingsMenu()
    {
        _tempUiScale = CFG.Current.UIScale;
    }

    public void SaveSettings()
    {
        CFG.Save();
    }

    private void DisplaySettings_System()
    {
        if (ImGui.BeginTabItem("System"))
        {
            if (ImGui.CollapsingHeader("General", ImGuiTreeNodeFlags.DefaultOpen))
            {
                EditorDecorations.ShowHelpMarker("When enabled DSMS will automatically check for new versions upon program start.");
                ImGui.Checkbox("Check for new versions of DSMapStudio during startup",
                    ref CFG.Current.EnableCheckProgramUpdate);

                EditorDecorations.ShowHelpMarker("This is a tooltip.");
                ImGui.Checkbox("Show UI tooltips", ref CFG.Current.ShowUITooltips);

                EditorDecorations.ShowHelpMarker("Adjusts the scale of the user interface throughout all of DSMS.");
                ImGui.SliderFloat("UI scale", ref _tempUiScale, 0.5f, 4.0f);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    // Round to 0.05
                    CFG.Current.UIScale = (float)Math.Round(_tempUiScale * 20) / 20;
                    MapStudioNew.UIScaleChanged?.Invoke(null, EventArgs.Empty);
                    _tempUiScale = CFG.Current.UIScale;
                }

                ImGui.SameLine();
                if (ImGui.Button("Reset"))
                {
                    CFG.Current.UIScale = CFG.Default.UIScale;
                    _tempUiScale = CFG.Current.UIScale;
                    MapStudioNew.UIScaleChanged?.Invoke(null, EventArgs.Empty);
                }

                EditorDecorations.ShowHelpMarker("Multiplies the user interface scale by your monitor's DPI setting.");
                ImGui.Checkbox($"Multiply UI scale by DPI ({(MapStudioNew.Dpi / 96).ToString("P0", new NumberFormatInfo { PercentPositivePattern = 1, PercentNegativePattern = 1 })})", ref CFG.Current.UIScaleByDPI);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    MapStudioNew.UIScaleChanged?.Invoke(null, EventArgs.Empty);
                }
            }

            if (ImGui.CollapsingHeader("Soapstone Server"))
            {
                var running = SoapstoneServer.GetRunningPort() is int port
                    ? $"running on port {port}"
                    : "not running";
                ImGui.Text(
                    $"The server is {running}.\nIt is not accessible over the network, only to other programs on this computer.\nPlease restart the program for changes to take effect.");
                ImGui.Checkbox("Enable cross-editor features", ref CFG.Current.EnableSoapstone);
            }

            // Additional Language Fonts
            if (ImGui.CollapsingHeader("Additional Language Fonts"))
            {
                EditorDecorations.ShowHelpMarker("Include Chinese font.\nAdditional fonts take more VRAM and increase startup time.");
                if (ImGui.Checkbox("Chinese", ref CFG.Current.FontChinese))
                {
                    MapStudioNew.FontRebuildRequest = true;
                }

                EditorDecorations.ShowHelpMarker("Include Korean font.\nAdditional fonts take more VRAM and increase startup time.");
                if (ImGui.Checkbox("Korean", ref CFG.Current.FontKorean))
                {
                    MapStudioNew.FontRebuildRequest = true;
                }

                EditorDecorations.ShowHelpMarker("Include Thai font.\nAdditional fonts take more VRAM and increase startup time.");
                if (ImGui.Checkbox("Thai", ref CFG.Current.FontThai))
                {
                    MapStudioNew.FontRebuildRequest = true;
                }

                EditorDecorations.ShowHelpMarker("Include Vietnamese font.\nAdditional fonts take more VRAM and increase startup time.");
                if (ImGui.Checkbox("Vietnamese", ref CFG.Current.FontVietnamese))
                {
                    MapStudioNew.FontRebuildRequest = true;
                }

                EditorDecorations.ShowHelpMarker("Include Cyrillic font.\nAdditional fonts take more VRAM and increase startup time.");
                if (ImGui.Checkbox("Cyrillic", ref CFG.Current.FontCyrillic))
                {
                    MapStudioNew.FontRebuildRequest = true;
                }
            }

            if (ImGui.CollapsingHeader("Resources", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Checkbox("Alias Banks - Editor Mode", ref CFG.Current.AliasBank_EditorMode);
                Editor.EditorDecorations.ShowHelpMarker("If enabled, editing the name and tags for alias banks will commit the changes to the DSMS base version instead of the mod-specific version.");
            }

            if (ImGui.CollapsingHeader("Project", ImGuiTreeNodeFlags.DefaultOpen))
            {
                if (ProjSettings == null || ProjSettings.ProjectName == null)
                {
                    if (CFG.Current.ShowUITooltips)
                    {
                        EditorDecorations.ShowHelpMarker("No project has been loaded yet.");
                        ImGui.SameLine();
                    }
                    ImGui.Text("No project loaded");
                }
                else
                {
                    if (TaskManager.AnyActiveTasks())
                    {
                        EditorDecorations.ShowHelpMarker("DSMS must finished all program tasks before it can load a project.");
                        ImGui.Text("Waiting for program tasks to finish...");
                    }
                    else
                    {
                        EditorDecorations.ShowHelpMarker("This is the currently loaded project.");
                        ImGui.Text($@"Project: {ProjSettings.ProjectName}");

                        if (ImGui.Button("Open project settings file"))
                        {
                            var projectPath = CFG.Current.LastProjectFile;
                            Process.Start("explorer.exe", projectPath);
                        }

                        var useLoose = ProjSettings.UseLooseParams;
                        if (ProjSettings.GameType is GameType.DarkSoulsIISOTFS or GameType.DarkSoulsIII)
                        {
                            EditorDecorations.ShowHelpMarker("Loose params means the .PARAM files will be saved outside of the regulation.bin file.\n\nFor Dark Souls II: Scholar of the First Sin, it is recommended that you enable this if add any additional rows.");
                            if (ImGui.Checkbox("Use loose params", ref useLoose))
                            {
                                ProjSettings.UseLooseParams = useLoose;
                            }
                        }
                    }
                }
            }

            ImGui.EndTabItem();
        }
    }

    private void DisplaySettings_Keybinds()
    {
        if (ImGui.BeginTabItem("Keybinds"))
        {
            if (ImGui.IsAnyItemActive())
            {
                _currentKeyBind = null;
            }

            FieldInfo[] binds = KeyBindings.Current.GetType().GetFields();
            foreach (FieldInfo bind in binds)
            {
                var bindVal = (KeyBind)bind.GetValue(KeyBindings.Current);
                ImGui.Text(bind.Name);

                ImGui.SameLine();
                ImGui.Indent(250f);

                var keyText = bindVal.HintText;
                if (keyText == "")
                {
                    keyText = "[None]";
                }

                if (_currentKeyBind == bindVal)
                {
                    ImGui.Button("Press Key <Esc - Clear>");
                    if (InputTracker.GetKeyDown(Key.Escape))
                    {
                        bind.SetValue(KeyBindings.Current, new KeyBind());
                        _currentKeyBind = null;
                    }
                    else
                    {
                        KeyBind newkey = InputTracker.GetNewKeyBind();
                        if (newkey != null)
                        {
                            bind.SetValue(KeyBindings.Current, newkey);
                            _currentKeyBind = null;
                        }
                    }
                }
                else if (ImGui.Button($"{keyText}##{bind.Name}"))
                {
                    _currentKeyBind = bindVal;
                }

                ImGui.Indent(-250f);
            }

            ImGui.Separator();

            if (ImGui.Button("Restore defaults"))
            {
                KeyBindings.ResetKeyBinds();
            }

            ImGui.EndTabItem();
        }
    }
    private void DisplaySettings_AssetBrowser()
    {
        if (ImGui.BeginTabItem("Asset Browser"))
        {
            // General
            if (ImGui.CollapsingHeader("General", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Checkbox("Display aliases in browser list", ref CFG.Current.AssetBrowser_ShowAliasesInBrowser);
                Editor.EditorDecorations.ShowHoverTooltip("Show the aliases for each entry within the browser list as part of their displayed name.");

                ImGui.Checkbox("Display tags in browser list", ref CFG.Current.AssetBrowser_ShowTagsInBrowser);
                Editor.EditorDecorations.ShowHoverTooltip("Show the tags for each entry within the browser list as part of their displayed name.");

                ImGui.Checkbox("Display low-detail parts in browser list", ref CFG.Current.AssetBrowser_ShowLowDetailParts);
                Editor.EditorDecorations.ShowHoverTooltip("Show the _l (low-detail) part entries in the Model Editor instance of the Asset Browser.");
            }

            ImGui.EndTabItem();
        }
    }

    public void Display(IEnumerable<EditorScreen> editorScreens)
    {
        var scale = MapStudioNew.GetUIScale();
        if (!MenuOpenState)
        {
            return;
        }

        ImGui.SetNextWindowSize(new Vector2(900.0f, 800.0f) * scale, ImGuiCond.FirstUseEver);
        ImGui.PushStyleColorVec4(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0.98f));
        ImGui.PushStyleColorVec4(ImGuiCol.TitleBgActive, new Vector4(0.25f, 0.25f, 0.25f, 1.0f));
        ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowPadding, new Vector2(10.0f, 10.0f) * scale);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.ItemSpacing, new Vector2(20.0f, 10.0f) * scale);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.IndentSpacing, 20.0f * scale);

        if (ImGui.Begin("Settings Menu##Popup", ref MenuOpenState, ImGuiWindowFlags.NoDocking))
        {
            ImGui.BeginTabBar("#SettingsMenuTabBar");
            ImGui.PushStyleColorVec4(ImGuiCol.Header, new Vector4(0.3f, 0.3f, 0.6f, 0.4f));
            ImGui.PushItemWidth(300f);

            // Settings Order
            DisplaySettings_System();
            DisplaySettings_AssetBrowser();
            foreach (EditorScreen scr in editorScreens)
            {
                if (ImGui.BeginTabItem(scr.EditorName))
                {
                    scr.SettingsMenu();
                    ImGui.EndTabItem();
                }
            }
            DisplaySettings_Keybinds();

            ImGui.PopItemWidth();
            ImGui.PopStyleColor(1);
            ImGui.EndTabBar();
        }

        ImGui.End();

        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(2);
    }
}
