using static Andre.Native.ImGuiBindings;
using StudioCore.MsbEditor;
using System.Numerics;

namespace StudioCore.Renderer.Scene;

internal class CreatePrefabModal : IModal
{
    private bool _open;
    private string _prefabCategory = "";

    private string _prefabName = "";
    private Entity _referenceEntity;

    private Universe _universe;

    public CreatePrefabModal(Universe universe, Entity reference)
    {
        _universe = universe;
        _referenceEntity = reference;
    }

    public bool IsClosed => !_open;

    public void OpenModal()
    {
        ImGui.OpenPopup("Create Prefab");
        _open = true;
    }

    public void OnGui()
    {
        var scale = MapStudioNew.GetUIScale();

        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowRounding, 7.0f * scale);
        ImGui.PushStyleVarFloat(ImGuiStyleVar.WindowBorderSize, 1.0f);
        ImGui.PushStyleVarVec2(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f) * scale);
        if (ImGui.BeginPopupModal("Create Prefab", ref _open, ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Prefab Name:      ");
            ImGui.SameLine();
            var pname = _prefabName;
            if (ImGui.InputText("##pname", ref pname, 255))
            {
                _prefabName = pname;
            }

            ImGui.NewLine();

            if (ImGui.Button("Create", new Vector2(120, 0) * scale))
            {
                var validated = true;

                if (validated)
                {
                    ImGui.CloseCurrentPopup();
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
