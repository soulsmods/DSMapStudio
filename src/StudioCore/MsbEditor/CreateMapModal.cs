using static Andre.Native.ImGuiBindings;
using StudioCore.MsbEditor;
using System.Numerics;
using System.Text.RegularExpressions;

namespace StudioCore.Renderer.Scene;

internal class CreateMapModal : IModal
{
    private bool _open;
    private bool _showErrorMessage;

    private string _mapId = "";
    Regex mapRegex = new(@"^m\d{2}_\d{2}_\d{2}_\d{2}$");

    private Universe _universe;

    public CreateMapModal(Universe universe)
    {
        _universe = universe;
    }

    public bool IsClosed => !_open;

    public void OpenModal()
    {
        ImGui.OpenPopup("Create Map");
        _open = true;
    }

    public void ShowHelpMarker(string desc)
    {
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

    private void CreateBlankMSBE() { 
        Map map = new Map(_universe, _mapId);
        _universe.LoadedObjectContainers.Add(_mapId, map);
        _universe.SaveMap(map);
    }

    public void OnGui()
    {
        var scale = MapStudioNew.GetUIScale();

        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowRounding, 7.0f * scale);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowBorderSize, 1.0f);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f) * scale);
        if (ImGui.BeginPopupModal("Create Map", ref _open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.AlignTextToFramePadding();
            if (CFG.Current.ShowUITooltips)
            {
                ShowHelpMarker("Map Id must match the following convention: m##_##_##_##. Must be unique.");
                ImGui.SameLine();
            }
            ImGui.Text("Map Id:        ");
            ImGui.SameLine();
            var mId = _mapId;
            if (ImGui.InputText("##mId", ref mId, 255))
            {
                _mapId = mId;
            }

            if (_showErrorMessage)
            {
                ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), "Invalid Map Id, see tooltip.");
            }
            else
            {
                ImGui.NewLine();
            }

            if (ImGui.Button("Create", new Vector2(120, 0) * scale))
            {
                bool isUniqueId = !_universe.LoadedObjectContainers.Keys.Contains(mId);
                bool isValidId = mapRegex.IsMatch(mId);

                if (isUniqueId && isValidId)
                {
                    CreateBlankMSBE();
                    _universe.UpdateWorldMsbList();
                    _showErrorMessage = false;
                    ImGui.CloseCurrentPopup();
                }
                else
                { 
                    _showErrorMessage = true;
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel", new Vector2(120, 0) * scale))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGui.PopStyleVar(3);
    }
}
