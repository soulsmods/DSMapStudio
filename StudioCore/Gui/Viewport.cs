using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.Utilities;
using ImGuiNET;

namespace StudioCore.Gui
{
    /// <summary>
    /// A viewport is a virtual (i.e. render to texture/render target) view of a scene. It can receive input events to transform
    /// the view within a virtual canvas, or it can be manually configured for say rendering thumbnails
    /// </summary>
    public class Viewport
    {
        private bool DebugRayCastDraw = false;

        /// <summary>
        /// The camera in this scene
        /// </summary>
        public WorldView WorldView;
        private Scene.RenderScene RenderScene;
        private Scene.SceneRenderPipeline ViewPipeline;

        private int PrevWidth;
        private int PrevHeight;
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool DrawGrid { get; set; } = true;

        private DebugPrimitives.DbgPrimWireGrid ViewportGrid;

        private Veldrid.Viewport RenderViewport;

        private BoundingFrustum Frustum;

        private Matrix4x4 ProjectionMat;

        private DebugPrimitives.DbgPrimWire RayDebug = null;

        //private DebugPrimitives.DbgPrimGizmoTranslate TranslateGizmo = null;
        private MsbEditor.Gizmos Gizmos;

        private Scene.Renderer.RenderQueue DebugRenderer;

        private MsbEditor.ActionManager ActionManager;

        private GraphicsDevice Device;

        private bool CanInteract = false;

        private Scene.FullScreenQuad ClearQuad;

        private bool _vpvisible = false;

        //public RenderTarget2D SceneRenderTarget = null;

        public Viewport(GraphicsDevice device, Scene.RenderScene scene, MsbEditor.ActionManager am, int width, int height)
        {
            PrevWidth = width;
            PrevHeight = height;
            Width = width;
            Height = height;
            Device = device;
            float depth = device.IsDepthRangeZeroToOne ? 1 : 0;
            RenderViewport = new Veldrid.Viewport(0, 0, Width, Height, depth, 1.0f - depth);

            RenderScene = scene;

            WorldView = new WorldView(new Veldrid.Rectangle(0, 0, Width, Height));
            ViewPipeline = new Scene.SceneRenderPipeline(scene, device, width, height);
            ViewportGrid = new DebugPrimitives.DbgPrimWireGrid(Color.Green, Color.DarkGreen, 50, 5.0f);
            //ViewportGrid.CreateDeviceObjects(device, null, ViewPipeline);

            //RenderScene.AddObject(ViewportGrid);

            ProjectionMat = Utils.CreatePerspective(device, false, 60.0f * (float)Math.PI / 180.0f, (float)width / (float)height, 0.1f, 2000.0f);
            Frustum = new BoundingFrustum(ProjectionMat);
            ActionManager = am;

            ViewPipeline.SetViewportSetupAction((d, cl) =>
            {
                cl.SetFramebuffer(device.SwapchainFramebuffer);
                cl.SetViewport(0, RenderViewport);
                if (_vpvisible)
                {
                    ClearQuad.Render(d, cl);
                }
                _vpvisible = false;
                //cl.SetFullViewports();
                //cl.SetScissorRect(0, (uint)RenderViewport.X, (uint)RenderViewport.Y, (uint)RenderViewport.Width, (uint)RenderViewport.Height);
                //cl.ClearColorTarget(0, new RgbaFloat(0.5f, 0.5f, 0.5f, 1.0f));
            });

            DebugRenderer = new Scene.Renderer.RenderQueue("Editor Overlays", device, ViewPipeline);
            DebugRenderer.SetPredrawSetupAction((d, cl) =>
            {
                cl.SetFramebuffer(device.SwapchainFramebuffer);
                cl.SetViewport(0, RenderViewport);
                //cl.SetFullViewports();
                cl.ClearDepthStencil(0);
            });
            Scene.Renderer.RegisterRenderQueue(DebugRenderer);

            // Create gizmos
            //TranslateGizmo = new DebugPrimitives.DbgPrimGizmoTranslate();
            Gizmos = new MsbEditor.Gizmos(ActionManager);
            Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
            {
                //TranslateGizmo.CreateDeviceObjects(d, cl, ViewPipeline);
                Gizmos.CreateDeviceObjects(d, cl, ViewPipeline);
            });

            ClearQuad = new Scene.FullScreenQuad();
            Scene.Renderer.AddBackgroundUploadTask((gd, cl) =>
            {
                ClearQuad.CreateDeviceObjects(gd, cl);
            });
        }

        public Ray GetRay(float sx, float sy)
        {
            float x = (2.0f * sx) / Width - 1.0f;
            float y = 1.0f - (2.0f * sy) / Height;
            float z = 1.0f;

            Vector3 deviceCoords = new Vector3(x, y, z);

            // Clip Coordinates
            Vector4 clipCoords = new Vector4(deviceCoords.X, deviceCoords.Y, -1.0f, 1.0f);

            // View Coordinates
            Matrix4x4 invProj;
            Matrix4x4.Invert(ProjectionMat, out invProj);
            Vector4 viewCoords = Vector4.Transform(clipCoords, invProj);
            viewCoords.Z = 1.0f;
            viewCoords.W = 0.0f;

            Matrix4x4 invView;
            Matrix4x4.Invert(WorldView.CameraTransform.CameraViewMatrixLH, out invView);
            Vector3 worldCoords = Vector4.Transform(viewCoords, invView).XYZ();
            worldCoords = Vector3.Normalize(worldCoords);
            //worldCoords.X = -worldCoords.X;

            return new Ray(WorldView.CameraTransform.Position, worldCoords);
        }

        private bool MouseInViewport()
        {
            var mp = InputTracker.MousePosition;
            if ((int)mp.X < X || (int)mp.X >= X + Width)
            {
                return false;
            }
            if ((int)mp.Y < Y || (int)mp.Y >= Y + Height)
            {
                return false;
            }
            return true;
        }

        public void OnGui()
        {
            if (ImGui.Begin("Viewport", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoNav))
            {
                var p = ImGui.GetWindowPos();
                var s = ImGui.GetWindowSize();
                var newvp = new Veldrid.Rectangle((int)p.X, (int)p.Y + 3, (int)s.X, (int)s.Y - 3);
                ResizeViewport(Device, newvp);
                if (InputTracker.GetMouseButtonDown(MouseButton.Right) && MouseInViewport())
                {
                    ImGui.SetWindowFocus();
                }
                CanInteract = ImGui.IsWindowFocused();
                ImGui.End();
                _vpvisible = true;
            }

            if (ImGui.Begin("Profiling"))
            {
                ImGui.Text($@"Cull time: {RenderScene.OctreeCullTime} ms");
                ImGui.Text($@"Work creation time: {RenderScene.CPUDrawTime} ms");
                ImGui.Text($@"Scene Render CPU time: {ViewPipeline.CPURenderTime} ms");
                ImGui.Text($@"Visible objects: {RenderScene.RenderObjectCount}");
                ImGui.Text($@"Vertex Buffers Size: {Scene.Renderer.GeometryBufferAllocator.TotalVertexFootprint / 1024 / 1024} MB");
                ImGui.Text($@"Index Buffers Size: {Scene.Renderer.GeometryBufferAllocator.TotalIndexFootprint / 1024 / 1024} MB");
                ImGui.Text($@"FLVER Read Caches: {Resource.FlverResource.CacheCount}");
                ImGui.Text($@"FLVER Read Caches Size: {Resource.FlverResource.CacheFootprint / 1024 / 1024} MB");
                ImGui.End();
            }
        }

        public void SceneParamsGui()
        {
            ImGui.SliderFloat4("Light Direction", ref ViewPipeline.SceneParams.LightDirection, -1, 1);
            ImGui.SliderFloat("Direct Light Mult", ref ViewPipeline.SceneParams.DirectLightMult, 0, 3);
            ImGui.SliderFloat("Indirect Light Mult", ref ViewPipeline.SceneParams.IndirectLightMult, 0, 3);
            ImGui.SliderFloat("Brightness", ref ViewPipeline.SceneParams.SceneBrightness, 0, 5);
        }

        public void ResizeViewport(GraphicsDevice device, Veldrid.Rectangle newvp)
        {
            PrevWidth = Width;
            PrevHeight = Height;
            Width = newvp.Width;
            Height = newvp.Height;
            X = newvp.X;
            Y = newvp.Y;
            WorldView.UpdateBounds(newvp);
            float depth = device.IsDepthRangeZeroToOne ? 0 : 1;
            RenderViewport = new Veldrid.Viewport(newvp.X, newvp.Y, Width, Height, depth, 1.0f - depth);
        }

        public bool Update(Sdl2Window window, float dt)
        {
            var pos = InputTracker.MousePosition;
            var ray = GetRay((float)pos.X - (float)X, (float)pos.Y - (float)Y);

            Gizmos.Update(ray, CanInteract && MouseInViewport());

            bool kbbusy = false;

            if (!Gizmos.IsMouseBusy() && CanInteract && MouseInViewport())
            {
                kbbusy = WorldView.UpdateInput(window, dt);
                if (InputTracker.GetMouseButtonDown(MouseButton.Left))
                {
                    var hit = RenderScene.CastRay(ray);
                    if (hit != null && hit.Selectable != null)
                    {
                        if (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                        {
                            Scene.ISelectable sel;
                            var b = hit.Selectable.TryGetTarget(out sel);
                            if (b)
                            {
                                MsbEditor.Selection.AddSelection(sel);
                            }
                        }
                        else
                        {
                            Scene.ISelectable sel;
                            var b = hit.Selectable.TryGetTarget(out sel);
                            if (b)
                            {
                                MsbEditor.Selection.ClearSelection();
                                MsbEditor.Selection.AddSelection(sel);
                            }
                        }
                    }
                    else
                    {
                        MsbEditor.Selection.ClearSelection();
                    }
                    if (DebugRayCastDraw)
                    {
                        RayDebug = new DebugPrimitives.DbgPrimWireRay(Transform.Default, ray.Origin, ray.Origin + ray.Direction * 50.0f, Color.Blue);
                        Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                        {
                            RayDebug.CreateDeviceObjects(d, cl, ViewPipeline);
                        });
                    }
                }
            }

            //Gizmos.DebugGui();
            return kbbusy;
        }

        public void Draw(GraphicsDevice device, CommandList cl)
        {
            ProjectionMat = Utils.CreatePerspective(device, true, 60.0f * (float)Math.PI / 180.0f, (float)Width / (float)Height, 0.1f, 2000.0f);
            Frustum = new BoundingFrustum(WorldView.CameraTransform.CameraViewMatrixLH * ProjectionMat);
            ViewPipeline.TestUpdateView(ProjectionMat, WorldView.CameraTransform.CameraViewMatrixLH, WorldView.CameraTransform.Position);
            ViewPipeline.RenderScene(Frustum);

            if (RayDebug != null)
            {
                DebugRenderer.Add(RayDebug, new Scene.RenderKey(0));
            }

            if (DrawGrid)
            {
                //DebugRenderer.Add(ViewportGrid, new Scene.RenderKey(0));
                //ViewportGrid.UpdatePerFrameResources(device, cl, ViewPipeline);
                //ViewportGrid.Render(device, cl, ViewPipeline);
            }

            Gizmos.CameraPosition = WorldView.CameraTransform.Position;
            DebugRenderer.Add(Gizmos, new Scene.RenderKey(0));
            //RenderScene.Render(device, cl, ViewPipeline);
        }

        public void SetEnvMap(uint index)
        {
            ViewPipeline.EnvMapTexture = index;
        }

        /// <summary>
        /// Moves the camera position such that it is directly looking at the center of a
        /// bounding box. Camera will face the same direction as before.
        /// </summary>
        /// <param name="box">The bounding box to frame</param>
        public void FrameBox(BoundingBox box)
        {
            var camdir = Vector3.Transform(Vector3.UnitZ, WorldView.CameraTransform.RotationMatrix);
            var pos = box.GetCenter();
            var radius = Vector3.Distance(box.Max, box.Min);
            WorldView.CameraTransform.Position = pos - (camdir * radius);
        }
    }
}
