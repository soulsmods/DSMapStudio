using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class DisplayGroupsEditor
    {
        private Scene.RenderScene Scene;

        public DisplayGroupsEditor(Scene.RenderScene scene)
        {
            Scene = scene;
        }

        public void OnGui(GameType game)
        {
            uint[] sdrawgroups = null;
            uint[] sdispgroups = null;
            var sel = Selection.GetSingleFilteredSelection<MapObject>();
            if (sel != null)
            {
                if (sel.UseDrawGroups)
                {
                    sdrawgroups = sel.Drawgroups;
                }
                sdispgroups = sel.Dispgroups;
            }

            ImGui.SetNextWindowSize(new Vector2(100, 100));
            if (ImGui.Begin("Display Groups"))
            {
                var dg = Scene.DisplayGroup;
                var count = (game == GameType.DemonsSouls || game == GameType.DarkSoulsPTDE) ? 4 : 8;
                if (dg.AlwaysVisible || dg.Drawgroups.Length != count)
                {
                    dg.Drawgroups = new uint[count];
                    for (int i = 0; i < count; i++)
                    {
                        dg.Drawgroups[i] = 0xFFFFFFFF;
                    }
                    dg.AlwaysVisible = false;
                }

                if (ImGui.Button("Check All"))
                {
                    for (int i = 0; i < count; i++)
                    {
                        dg.Drawgroups[i] = 0xFFFFFFFF;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Uncheck All"))
                {
                    for (int i = 0; i < count; i++)
                    {
                        dg.Drawgroups[i] = 0;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Set from Selected") && sdispgroups != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        dg.Drawgroups[i] = sdispgroups[i];
                    }
                }

                for (int g = 0; g < dg.Drawgroups.Length; g++)
                {
                    ImGui.Text($@"Display Group {g}: ");
                    for (int i = 0; i < 32; i++)
                    {
                        bool check = ((dg.Drawgroups[g] >> i) & 0x1) > 0;
                        ImGui.SameLine();
                        bool red = sdrawgroups != null && (((sdrawgroups[g] >> i) & 0x1) > 0);
                        bool green = sdispgroups != null && (((sdispgroups[g] >> i) & 0x1) > 0);
                        if (red)
                        {
                            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.4f, 0.06f, 0.06f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
                        }
                        else if (green)
                        {
                            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.02f, 0.3f, 0.02f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(0.2f, 1.0f, 0.2f, 1.0f));
                        }
                        if (ImGui.Checkbox($@"##dispgroup{g}{i}", ref check))
                        {
                            if (check)
                            {
                                dg.Drawgroups[g] |= (1u << i);
                            }
                            else
                            {
                                dg.Drawgroups[g] &= ~(1u << i);
                            }
                        }
                        if (red || green)
                        {
                            ImGui.PopStyleColor(2);
                        }
                    }
                    ImGui.NewLine();
                }
            }
            ImGui.End();
        }
    }
}
