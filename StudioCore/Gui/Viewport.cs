﻿using System;
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
    public class Viewport : IViewport
    {
        private bool DebugRayCastDraw = false;

        /// <summary>
        /// The camera in this scene
        /// </summary>
        public WorldView WorldView { get; private set; }
        
        private Scene.RenderScene _renderScene;
        private Scene.SceneRenderPipeline _viewPipeline;
        private MsbEditor.Selection _selection;
        
        public int X;
        public int Y;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public float NearClip { get; set; } = 0.1f;
        public float FarClip { get; set; } = CFG.Current.GFX_RenderDistance_Max;

        public bool DrawGrid { get; set; } = true;

        private DebugPrimitives.DbgPrimWireGrid ViewportGrid;

        private Veldrid.Viewport _renderViewport;

        private BoundingFrustum _frustum;

        private Matrix4x4 _projectionMat;

        private DebugPrimitives.DbgPrimWire _rayDebug = null;

        //private DebugPrimitives.DbgPrimGizmoTranslate TranslateGizmo = null;
        private MsbEditor.Gizmos _gizmos;

        private MsbEditor.ActionManager _actionManager;

        private GraphicsDevice _device;

        private bool _canInteract = false;

        private Scene.FullScreenQuad _clearQuad;

        private bool _vpvisible = false;

        private string _vpid = "";

        private int _cursorX = 0;
        private int _cursorY = 0;

        //public RenderTarget2D SceneRenderTarget = null;

        public Viewport(string id, GraphicsDevice device, Scene.RenderScene scene, MsbEditor.ActionManager am, MsbEditor.Selection sel, int width, int height)
        {
            _vpid = id;
            Width = width;
            Height = height;
            _device = device;
            float depth = device.IsDepthRangeZeroToOne ? 1 : 0;
            _renderViewport = new Veldrid.Viewport(0, 0, Width, Height, depth, 1.0f - depth);

            _renderScene = scene;
            _selection = sel;

            WorldView = new WorldView(new Veldrid.Rectangle(0, 0, Width, Height));
            _viewPipeline = new Scene.SceneRenderPipeline(scene, device, width, height);
            ViewportGrid = new DebugPrimitives.DbgPrimWireGrid(Color.Green, Color.DarkGreen, 50, 5.0f);

            _projectionMat = Utils.CreatePerspective(device, false, CFG.Current.GFX_Camera_FOV * (float)Math.PI / 180.0f, (float)width / (float)height, NearClip, FarClip);
            _frustum = new BoundingFrustum(_projectionMat);
            _actionManager = am;

            _viewPipeline.SetViewportSetupAction((d, cl) =>
            {
                cl.SetFramebuffer(device.SwapchainFramebuffer);
                cl.SetViewport(0, _renderViewport);
                if (_vpvisible)
                {
                    _clearQuad.Render(d, cl);
                }
                _vpvisible = false;
            });

            _viewPipeline.SetOverlayViewportSetupAction((d, cl) =>
            {
                cl.SetFramebuffer(device.SwapchainFramebuffer);
                cl.SetViewport(0, _renderViewport);
                cl.ClearDepthStencil(0);
            });

            // Create gizmos
            _gizmos = new MsbEditor.Gizmos(_actionManager, _selection, _renderScene.OverlayRenderables);

            _clearQuad = new Scene.FullScreenQuad();
            Scene.Renderer.AddBackgroundUploadTask((gd, cl) =>
            {
                _clearQuad.CreateDeviceObjects(gd, cl);
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
            Matrix4x4.Invert(_projectionMat, out invProj);
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

        public bool ViewportSelected { get; private set; } = false;

        public void OnGui()
        {
            if (ImGui.Begin($@"Viewport##{_vpid}", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoNav))
            {
                var p = ImGui.GetWindowPos();
                var s = ImGui.GetWindowSize();
                var newvp = new Veldrid.Rectangle((int)p.X, (int)p.Y + 3, (int)s.X, (int)s.Y - 3);
                ResizeViewport(_device, newvp);
                if (InputTracker.GetMouseButtonDown(MouseButton.Right) && MouseInViewport())
                {
                    ImGui.SetWindowFocus();
                    ViewportSelected = true;
                }
                else if(!InputTracker.GetMouseButton(MouseButton.Right))
                {
                    ViewportSelected = false;
                }
                _canInteract = ImGui.IsWindowFocused();
                _vpvisible = true;
                var proj = Matrix4x4.Transpose(_projectionMat);
                var view = Matrix4x4.Transpose(WorldView.CameraTransform.CameraViewMatrixLH);
                var identity = Matrix4x4.Identity;
                //ImGui.DrawGrid(ref view.M11, ref proj.M11, ref identity.M11, 100.0f);
            }
            ImGui.End();

            if (ImGui.Begin($@"Profiling##{_vpid}"))
            {
                ImGui.Text($@"Cull time: {_renderScene.OctreeCullTime} ms");
                ImGui.Text($@"Work creation time: {_renderScene.CPUDrawTime} ms");
                ImGui.Text($@"Scene Render CPU time: {_viewPipeline.CPURenderTime} ms");
                ImGui.Text($@"Visible objects: {_renderScene.RenderObjectCount}");
                ImGui.Text($@"Vertex Buffers Size: {Scene.Renderer.GeometryBufferAllocator.TotalVertexFootprint / 1024 / 1024} MB");
                ImGui.Text($@"Index Buffers Size: {Scene.Renderer.GeometryBufferAllocator.TotalIndexFootprint / 1024 / 1024} MB");
                ImGui.Text($@"FLVER Read Caches: {Resource.FlverResource.CacheCount}");
                ImGui.Text($@"FLVER Read Caches Size: {Resource.FlverResource.CacheFootprint / 1024 / 1024} MB");
                //ImGui.Text($@"Selected renderable:  { _viewPipeline._pickingEntity }");
            }
            ImGui.End();
        }

        public void SceneParamsGui()
        {
            ImGui.SliderFloat4("Light Direction", ref _viewPipeline.SceneParams.LightDirection, -1, 1);
            ImGui.SliderFloat("Direct Light Mult", ref _viewPipeline.SceneParams.DirectLightMult, 0, 3);
            ImGui.SliderFloat("Indirect Light Mult", ref _viewPipeline.SceneParams.IndirectLightMult, 0, 3);
            ImGui.SliderFloat("Brightness", ref _viewPipeline.SceneParams.SceneBrightness, 0, 5);
        }

        public void ResizeViewport(GraphicsDevice device, Veldrid.Rectangle newvp)
        {
            Width = newvp.Width;
            Height = newvp.Height;
            X = newvp.X;
            Y = newvp.Y;
            WorldView.UpdateBounds(newvp);
            float depth = device.IsDepthRangeZeroToOne ? 0 : 1;
            _renderViewport = new Veldrid.Viewport(newvp.X, newvp.Y, Width, Height, depth, 1.0f - depth);
        }

        public bool Update(Sdl2Window window, float dt)
        {
            var pos = InputTracker.MousePosition;
            var ray = GetRay((float)pos.X - (float)X, (float)pos.Y - (float)Y);

            _cursorX = (int)pos.X;// - X;
            _cursorY = (int)pos.Y;// - Y;

            _gizmos.Update(ray, _canInteract && MouseInViewport());

            bool kbbusy = false;

            if (!_gizmos.IsMouseBusy() && _canInteract && MouseInViewport())
            {
                kbbusy = WorldView.UpdateInput(window, dt);
                if (InputTracker.GetMouseButtonDown(MouseButton.Left))
                {
                    _viewPipeline.CreateAsyncPickingRequest();
                }
                if (_viewPipeline.PickingResultsReady)
                {
                    var sel = _viewPipeline.GetSelection();
                    if (InputTracker.GetKey(Key.ControlLeft) || InputTracker.GetKey(Key.ControlRight))
                    {
                        // Toggle selection
                        if (sel != null)
                        {
                            if (_selection.GetSelection().Contains(sel))
                            {
                                _selection.RemoveSelection(sel);
                            }
                            else
                            {
                                _selection.AddSelection(sel);
                            }
                        }
                    }
                    else if (InputTracker.GetKey(Key.ShiftLeft) || InputTracker.GetKey(Key.ShiftRight))
                    {
                        // Add to selection
                        if (sel != null)
                        {
                            _selection.AddSelection(sel);
                        }
                    }
                    else
                    {
                        // Exclusive selection
                        _selection.ClearSelection();
                        if (sel != null)
                        {
                            _selection.AddSelection(sel);
                        }
                    }
                }
            }

            //Gizmos.DebugGui();
            return kbbusy;
        }

        public void Draw(GraphicsDevice device, CommandList cl)
        {
            _projectionMat = Utils.CreatePerspective(device, true, CFG.Current.GFX_Camera_FOV * (float)Math.PI / 180.0f, (float)Width / (float)Height, NearClip, FarClip);
            _frustum = new BoundingFrustum(WorldView.CameraTransform.CameraViewMatrixLH * _projectionMat);
            _viewPipeline.TestUpdateView(_projectionMat, WorldView.CameraTransform.CameraViewMatrixLH, WorldView.CameraTransform.Position, _cursorX, _cursorY);
            _viewPipeline.RenderScene(_frustum);

            if (_rayDebug != null)
            {
                //TODO:_debugRenderer.Add(_rayDebug, new Scene.RenderKey(0));
            }

            if (DrawGrid)
            {
                //DebugRenderer.Add(ViewportGrid, new Scene.RenderKey(0));
                //ViewportGrid.UpdatePerFrameResources(device, cl, ViewPipeline);
                //ViewportGrid.Render(device, cl, ViewPipeline);
            }

            _gizmos.CameraPosition = WorldView.CameraTransform.Position;
        }

        public void SetEnvMap(uint index)
        {
            _viewPipeline.EnvMapTexture = index;
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

        /// <summary>
        /// Moves the camera position such that it is directly looking at a position.
        public void FramePosition(Vector3 pos, float dist)
        {
            var camdir = Vector3.Transform(Vector3.UnitZ, WorldView.CameraTransform.RotationMatrix);
            WorldView.CameraTransform.Position = pos - (camdir * dist);
        }
    }
}
