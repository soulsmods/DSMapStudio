using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Andre.Native.ImGuiBindings;

namespace StudioCore.Interface;
public static class ImguiUtils
{
    public static unsafe void ShowMenuIcon(string iconStr)
    {
        ImGui.PushStyleVarVec2(ImGuiStyleVar.ItemSpacing, new Vector2(0, ImGui.GetStyle()->ItemSpacing.Y));
        ImGui.TextUnformatted(iconStr);
        ImGui.PopStyleVar(1);
        ImGui.SameLine();
    }

    public static unsafe void ShowActiveStatus(bool isActive)
    {
        if (isActive)
        {
            ImGui.SameLine();
            ImGui.PushStyleVarVec2(ImGuiStyleVar.ItemSpacing, new Vector2(0, ImGui.GetStyle()->ItemSpacing.Y));
            ImGui.TextUnformatted($"{ForkAwesome.CheckSquare}");
            ImGui.PopStyleVar(1);
        }
        else
        {
            ImGui.SameLine();
            ImGui.PushStyleVarVec2(ImGuiStyleVar.ItemSpacing, new Vector2(0, ImGui.GetStyle()->ItemSpacing.Y));
            ImGui.TextUnformatted($"{ForkAwesome.Square}");
            ImGui.PopStyleVar(1);
        }
    }

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

    public static void WrappedText(string text)
    {
        var size = ImGui.GetWindowSize();

        ImGui.PushTextWrapPos(size.X);
        ImGui.TextUnformatted(text);
        ImGui.PopTextWrapPos();
    }

    public static void WrappedTextColored(Vector4 color, string text)
    {
        var size = ImGui.GetWindowSize();

        ImGui.PushTextWrapPos(size.X);
        ImGui.PushStyleColorVec4(ImGuiCol.Text, color);
        ImGui.TextUnformatted(text);
        ImGui.PopStyleColor(1);
        ImGui.PopTextWrapPos();
    }
}
