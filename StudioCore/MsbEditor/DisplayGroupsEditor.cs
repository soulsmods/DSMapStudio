using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Veldrid;
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

            if (InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GetDisp)
            || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GetDraw)
            || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GiveDisp)
            || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GiveDraw)
            || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_ShowAll)
            || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_HideAll))
            {
                ImGui.SetNextWindowFocus();
            }

            if (ImGui.Begin("Render Groups"))
            {

                var dg = _scene.DisplayGroup;
                if (dg.AlwaysVisible || dg.RenderGroups.Length != dispCount)
                {
                    dg.RenderGroups = new uint[dispCount];
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.RenderGroups[i] = 0xFFFFFFFF;
                    }
                    dg.AlwaysVisible = false;
                }

                if (ImGui.Button($"Show All <{KeyBindings.Current.Map_RenderGroup_ShowAll.HintText}>") || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_ShowAll))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.RenderGroups[i] = 0xFFFFFFFF;
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button("Hide All"))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.RenderGroups[i] = 0;
                    }
                }

                ImGui.SameLine(0, 14f);
                if (sdispgroups == null)
                    ImGui.BeginDisabled();
                if (ImGui.Button($"Get DispGroups <{KeyBindings.Current.Map_RenderGroup_GetDisp.HintText}>") || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GetDisp) && sdispgroups != null)
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.RenderGroups[i] = sdispgroups[i];
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button($"Get DrawGroups <{KeyBindings.Current.Map_RenderGroup_GetDraw.HintText}>") || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GetDraw))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        dg.RenderGroups[i] = sdrawgroups[i];
                    }
                }

                ImGui.SameLine(0, 14f);
                if (ImGui.Button($"Give as DrawGroups <{KeyBindings.Current.Map_RenderGroup_GiveDraw.HintText}>") || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GiveDraw))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        sel.Drawgroups[i] = dg.RenderGroups[i];
                    }
                }

                ImGui.SameLine();
                if (ImGui.Button($"Give as DispGroups <{KeyBindings.Current.Map_RenderGroup_GiveDisp.HintText}>") || InputTracker.GetKeyDown(KeyBindings.Current.Map_RenderGroup_GiveDisp))
                {
                    for (int i = 0; i < dispCount; i++)
                    {
                        sel.Dispgroups[i] = dg.RenderGroups[i];
                    }
                }
                if (sdispgroups == null)
                    ImGui.EndDisabled();

                ImGui.SameLine(0, 60f);
                if (ImGui.Button("Help"))
                {
                    ImGui.OpenPopup("##RenderHelp");
                }
                if (ImGui.BeginPopup("##RenderHelp"))
                {
                    ImGui.Text(
                        "Render Groups are used by the game to determine what should render and what shouldn't.\n" +
                        "They consist of Display Groups and Draw Groups.\n" +
                        //"Display Groups: Determines which DrawGroups should render.\n" +
                        //"Draw Groups: Determines if things will render when a DispGroup is active.\n" +
                        "When a Display Group is active, Map Objects with that Draw Group will render.\n" +
                        "\n" +
                        "If a Map Object uses the CollisionName field, they will inherit Draw Groups from the referenced Map Object.\n" +
                        "Also, CollisionName references will be targeted by DSMapStudio when using `Set Selection`/`Get Selection` instead of your actual selection.\n" +
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

                ImGui.Separator();
                ImGui.BeginChild("##DispTicks");
                for (int g = 0; g < dg.RenderGroups.Length; g++)
                {
                    //row (groups)
                    
                    //add spacing every 4 rows
                    if (g % 4 == 0 && g > 0)
                    {
                        ImGui.Spacing();
                    }

                    ImGui.Text($@"Render Group {g}:");
                    for (int i = 0; i < 32; i++)
                    {
                        //column (bits)
                        bool check = ((dg.RenderGroups[g] >> i) & 0x1) > 0;

                        //add spacing every 4 boxes
                        if (i % 4 == 0)
                        {
                            ImGui.SameLine();
                            ImGui.Spacing();
                        }

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
                                dg.RenderGroups[g] |= (1u << i);
                            }
                            else
                            {
                                dg.RenderGroups[g] &= ~(1u << i);
                            }
                        }

                        if (drawActive || dispActive)
                        {
                            ImGui.PopStyleColor(2);
                        }
                    }
                }
                ImGui.EndChild();
            }
            ImGui.End();
        }
    }
}
