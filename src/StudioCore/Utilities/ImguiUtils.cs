using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Andre.Native.ImGuiBindings;

namespace StudioCore.Utilities;
public static class ImguiUtils
{
    public static void ShowHelpButton(string title, string desc, string id)
    {
        if (ImGui.Button($"{title}"))
            ImGui.OpenPopup($"##{id}HelpPopup");

        if (ImGui.BeginPopup($"##{id}HelpPopup"))
        {
            ImGui.Text($"{desc}");
            ImGui.EndPopup();
        }
    }

    public static void ShowHelpMarker(string desc)
    {
        if (CFG.Current.ShowUITooltips)
        {
            ImGui.SameLine();
            ImGui.TextDisabled("(?)");
            if (ImGui.IsItemHovered(0))
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(450.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }

    public static void ShowButtonTooltip(string desc)
    {
        if (CFG.Current.ShowUITooltips)
        {
            if (ImGui.IsItemHovered(0))
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(450.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }

    public static void ShowHoverTooltip(string desc)
    {
        if (CFG.Current.ShowUITooltips)
        {
            if (ImGui.IsItemHovered(0))
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(450.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }

    public static void ShowWideHoverTooltip(string desc)
    {
        if (CFG.Current.ShowUITooltips)
        {
            if (ImGui.IsItemHovered(0))
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(800.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }
    }

    public static string GetKeybindHint(string hint)
    {
        if (hint == "")
            return "None";
        else
            return hint;
    }
}
