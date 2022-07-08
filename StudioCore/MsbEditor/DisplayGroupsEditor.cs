using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class DisplayGroupsEditor
    {
        private Scene.RenderScene _scene;
        private Selection _selection;

        public DisplayGroupsEditor(Scene.RenderScene scene, Selection sel)
        {
            _scene = scene;
            _selection = sel;
        }

        public void OnGui(int dispCount)
        {
            uint[] sdrawgroups = null;
            uint[] sdispgroups = null;
            var sel = _selection.GetSingleFilteredSelection<Entity>();
            if (sel != null)
            {
                if (sel.UseDrawGroups)
                {
                    sdrawgroups = sel.Drawgroups; //Will be CollisionName values (if reference is valid)
                }
                sdispgroups = sel.Dispgroups;
                //sdispgroups = sel.FakeDispgroups;
            }

            ImGui.SetNextWindowSize(new Vector2(100, 100));
            if (ImGui.Begin("Display Groups"))
            {
                var dg = _scene.DisplayGroup;
                if (dg.AlwaysVisible || dg.Drawgroups.Length != dispCount)
                {
                    dg.Drawgroups = new uint[dispCount];
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.Drawgroups[i] = 0xFFFFFFFF;
                    }
                    dg.AlwaysVisible = false;
                }

                if (ImGui.Button("Show All"))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.Drawgroups[i] = 0xFFFFFFFF;
                    }
                }
                ImGui.SameLine();
                if (ImGui.Button("Hide All"))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.Drawgroups[i] = 0;
                    }
                }
                ImGui.SameLine();

                if (sdispgroups == null)
                    ImGui.BeginDisabled();
                if (ImGui.Button("Get Selection DispGroups"))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.Drawgroups[i] = sdispgroups[i];
                    }
                }
                if (sdispgroups == null)
                    ImGui.EndDisabled();

                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.6f, 0.6f, 1.0f, 1.0f), "Help");
                if (ImGui.BeginPopupContextWindow("Render Group Help"))
                {
                    ImGui.Text(
                        "Display Groups: Determines which DrawGroups should render.\n" +
                        "Draw Groups: Determines if things will render when a DispGroup is active.\n" +
                        "When a Display Group is active, Map Objects with that Draw Group will render.\n" +
                        "\n" +
                        "If a Map Object uses the CollisionName field, they will inherit Draw Groups from the referenced Map Object.\n" +
                        "When a character walks on top of a piece of collision, they will use its DispGroups and DrawGroups.\n" +
                        "\n" +
                        "Color indicates which Render Groups selected Map Object is using.\n" +
                        "Red = Selection uses this Display Group.\n" +
                        "Green = Selection uses this Draw Group.\n" +
                        "Yellow = Selection uses both.\n" +
                        "\n" +
                        "POTENTIALLY INACCURATE FOR SEKIRO / ELDEN RING!");

                    ImGui.EndPopup();
                }

                for (int g = 0; g < dg.Drawgroups.Length; g++)
                {
                    //row (groups)
                    ImGui.Text($@"Display Group {g}: ");
                    for (int i = 0; i < 32; i++)
                    {
                        //column (bits)
                        bool check = ((dg.Drawgroups[g] >> i) & 0x1) > 0;
                        ImGui.SameLine();

                        bool drawActive = sdrawgroups != null && (((sdrawgroups[g] >> i) & 0x1) > 0);
                        bool dispActive = sdispgroups != null && (((sdispgroups[g] >> i) & 0x1) > 0);

                        if (drawActive && dispActive)
                        {
                            //selection dispgroup and drawgroup is ticked
                            //yellow
                            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.4f, 0.4f, 0.06f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(1f, 1f, 0.02f, 1.0f));
                        }
                        else if (drawActive)
                        {
                            //selection drawgroup is ticked
                            //green
                            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.02f, 0.3f, 0.02f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(0.2f, 1.0f, 0.2f, 1.0f));
                        }
                        else if (dispActive)
                        {
                            //selection dispGroup is ticked
                            //red
                            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.4f, 0.06f, 0.06f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.CheckMark, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
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

                        if (drawActive || dispActive)
                        {
                            ImGui.PopStyleColor(2);
                        }
                    }
                }
            }
            ImGui.End();
        }
    }
}
