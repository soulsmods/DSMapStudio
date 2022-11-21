using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGuiNET;
using StudioCore.MsbEditor;
using Newtonsoft.Json;

namespace StudioCore.Scene
{
    class CreatePrefabModal : IModal
    {
        private bool _open = false;
        public bool IsClosed => !_open;

        private Universe _universe;
        private Entity _referenceEntity;

        private string _prefabName = "";
        private string _prefabCategory = "";

        public CreatePrefabModal(Universe universe, Entity reference)
        {
            _universe = universe;
            _referenceEntity = reference;
        }

        public void OpenModal()
        {
            ImGui.OpenPopup("Create Prefab");
            _open = true;
        }

        public void OnGui()
        {
            float scale = ImGuiRenderer.GetUIScale();

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 7.0f * scale);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(14.0f, 8.0f) * scale);
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
                    bool validated = true;

                    if (validated)
                    {
                        string json = JsonConvert.SerializeObject(_referenceEntity.WrappedObject);
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
}
