using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// Editor used to bake new navmeshes for cols
    /// </summary>
    public class NavmeshEditor
    {
        private Scene.RenderScene Scene;
        private Selection _selection;
        private bool DidBuild = false;
        private bool BuildSuccess = false;
        private int vcount = 0;
        private int icount = 0;

        private float Cellsize = 0.3f;
        private float Cellheight = 0.3f;
        private float SlopeAngle = 30.0f;
        private float AgentHeight = 2.0f;
        private float AgentClimb = 0.1f;
        private float AgentRadius = 0.5f;
        private int MinRegionArea = 3;

        private Scene.Mesh BakeResultPreview = null;

        public NavmeshEditor(Scene.RenderScene scene, Selection sel)
        {
            Scene = scene;
            _selection = sel;
        }

        public void OnGui(GameType game)
        {
            if (ImGui.Begin("Navmesh Build"))
            {
                if (game != GameType.DarkSoulsPTDE)
                {
                    ImGui.Text("Navmesh building only supported for DS1");
                    ImGui.End();
                    return;
                }

                var sel = _selection.GetSingleFilteredSelection<Entity>();
                if (sel == null || !(sel.RenderSceneMesh is Scene.CollisionMesh))
                {
                    ImGui.Text("Select a single collision mesh to generate a navmesh");
                    ImGui.End();
                    return;
                }

                ImGui.LabelText("value", "lable");
                ImGui.DragFloat("Cell size", ref Cellsize, 0.005f, 0.0f);
                ImGui.DragFloat("Cell height", ref Cellheight, 0.005f, 0.0f);
                ImGui.DragFloat("Slope Angle", ref SlopeAngle, 0.5f, 0.0f, 85.0f);
                ImGui.DragFloat("Agent Height", ref AgentHeight, 0.005f, 0.0f);
                ImGui.DragFloat("Agent Climb", ref AgentClimb, 0.005f, 0.0f);
                ImGui.DragFloat("Agent Radius", ref AgentRadius, 0.005f, 0.0f);
                ImGui.DragInt("Min Region Area", ref MinRegionArea, 1, 0);

                if (ImGui.Button("Build Navmesh"))
                {
                    /*FIX:var col = sel.RenderSceneMesh as Scene.CollisionMesh;
                    var buildverts = new List<Vector3>();
                    var buildindices = new List<int>();
                    int vbase = 0;
                    foreach (var sm in col.Resource.Get().GPUMeshes)
                    {
                        buildverts.AddRange(sm.PickingVertices);
                        foreach (var i in sm.PickingIndices)
                        {
                            buildindices.Add(i + vbase);
                        }
                        vbase += sm.PickingVertices.Length;
                    }
                    //var sm = col.Resource.Get().GPUMeshes[0];
                    var bv = buildverts.ToArray();
                    buildindices.Reverse();
                    var bi = buildindices.ToArray();

                    foreach (var i in bi)
                    {
                        var x = bv[i];
                    }

                    NavGen.SetNavmeshBuildParams(Cellsize, Cellheight, SlopeAngle, AgentHeight, AgentClimb, AgentRadius, MinRegionArea);
                    BuildSuccess = NavGen.BuildNavmeshForMesh(bv, bv.Length, bi, bi.Length);
                    DidBuild = true;
                    if (BuildSuccess)
                    {
                        vcount = NavGen.GetMeshVertCount();
                        icount = NavGen.GetMeshTriCount();

                        if (icount > 0)
                        {
                            // Make preview mesh
                            ushort[] verts = new ushort[vcount * 3];
                            ushort[] indices = new ushort[icount * 3 * 2];
                            NavGen.GetMeshVerts(verts);
                            NavGen.GetMeshTris(indices);

                            Vector3[] bounds = new Vector3[2];
                            NavGen.GetBoundingBox(bounds);

                            if (BakeResultPreview != null)
                            {
                                BakeResultPreview.UnregisterWithScene();
                                BakeResultPreview = null;
                            }

                            var nvm = new Scene.NvmRenderer(bounds[0], bounds[1], Cellsize, Cellheight, verts, indices);
                            BakeResultPreview = new Scene.Mesh(Scene, nvm.Bounds, nvm);
                            BakeResultPreview.Highlighted = true;
                        }
                    }*/
                }

                if (DidBuild)
                {
                    if (BuildSuccess)
                    {
                        ImGui.Text("Successfully built navmesh");
                        ImGui.Text($@"Vertex count: {vcount}");
                        ImGui.Text($@"Triangle count: {icount}");
                    }
                    else
                    {
                        ImGui.Text("Navmesh build failed");
                    }
                }

                ImGui.End();
            }
        }
    }
}
