using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using StudioCore.Scene;
using Veldrid;
using Veldrid.Utilities;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class Gizmos
    {
        public enum GizmosMode
        {
            Translate,
            Rotate,
            Scale
        }

        /// <summary>
        /// The rotation space that the gizmos works in
        /// </summary>
        public enum GizmosSpace
        {
            /// <summary>
            /// The gizmos is rotated by the local rotation
            /// </summary>
            Local,

            /// <summary>
            /// The gizmos rotation always originates at identity
            /// </summary>
            World
        }

        /// <summary>
        /// The origin where the gizmos is located relative to the object
        /// </summary>
        public enum GizmosOrigin
        {
            /// <summary>
            /// The gizmos originates at the selected's world space position
            /// </summary>
            World,

            /// <summary>
            /// The gizmos originates at the selected's bounding box center
            /// </summary>
            BoundingBox,

            /// <summary>
            /// The gizmos originates at the selected's parent if one exists
            /// </summary>
            Parent
        }

        public enum Axis
        {
            None,
            PosX,
            PosY,
            PosZ
        }

        public static GizmosMode Mode = GizmosMode.Translate;
        public static GizmosSpace Space = GizmosSpace.Local;
        public static GizmosOrigin Origin = GizmosOrigin.World;

        private DebugPrimitives.DbgPrimGizmoTranslateArrow TranslateGizmoX;
        private DebugPrimitives.DbgPrimGizmoTranslateArrow TranslateGizmoY;
        private DebugPrimitives.DbgPrimGizmoTranslateArrow TranslateGizmoZ;
        private DebugPrimitives.DbgPrimGizmoRotateRing RotateGizmoX;
        private DebugPrimitives.DbgPrimGizmoRotateRing RotateGizmoY;
        private DebugPrimitives.DbgPrimGizmoRotateRing RotateGizmoZ;

        private DebugPrimitiveRenderableProxy TranslateGizmoXProxy;
        private DebugPrimitiveRenderableProxy TranslateGizmoYProxy;
        private DebugPrimitiveRenderableProxy TranslateGizmoZProxy;
        private DebugPrimitiveRenderableProxy RotateGizmoXProxy;
        private DebugPrimitiveRenderableProxy RotateGizmoYProxy;
        private DebugPrimitiveRenderableProxy RotateGizmoZProxy;

        private Transform OriginalTransform = Transform.Default;
        private Transform CurrentTransform = Transform.Default;
        private Vector3 OriginProjection = new Vector3();
        private bool IsTransforming = false;
        private Axis TransformAxis = Axis.None;
        private ActionManager ActionManager;
        private Selection _selection;

        public Vector3 CameraPosition { get; set; }

        public Gizmos(ActionManager am, Selection selection, MeshRenderables renderlist)
        {
            ActionManager = am;
            TranslateGizmoX = new DebugPrimitives.DbgPrimGizmoTranslateArrow(Axis.PosX);
            TranslateGizmoY = new DebugPrimitives.DbgPrimGizmoTranslateArrow(Axis.PosY);
            TranslateGizmoZ = new DebugPrimitives.DbgPrimGizmoTranslateArrow(Axis.PosZ);

            TranslateGizmoXProxy = new DebugPrimitiveRenderableProxy(renderlist, TranslateGizmoX);
            TranslateGizmoXProxy.BaseColor = Color.FromArgb(0xF3, 0x36, 0x53);
            TranslateGizmoXProxy.HighlightedColor = Color.FromArgb(0xFF, 0x66, 0x83);
            TranslateGizmoYProxy = new DebugPrimitiveRenderableProxy(renderlist, TranslateGizmoY);
            TranslateGizmoYProxy.BaseColor = Color.FromArgb(0x86, 0xC8, 0x15);
            TranslateGizmoYProxy.HighlightedColor = Color.FromArgb(0xB6, 0xF8, 0x45);
            TranslateGizmoZProxy = new DebugPrimitiveRenderableProxy(renderlist, TranslateGizmoZ);
            TranslateGizmoZProxy.BaseColor = Color.FromArgb(0x38, 0x90, 0xED);
            TranslateGizmoZProxy.HighlightedColor = Color.FromArgb(0x68, 0xB0, 0xFF);

            RotateGizmoX = new DebugPrimitives.DbgPrimGizmoRotateRing(Axis.PosX);
            RotateGizmoY = new DebugPrimitives.DbgPrimGizmoRotateRing(Axis.PosY);
            RotateGizmoZ = new DebugPrimitives.DbgPrimGizmoRotateRing(Axis.PosZ);

            RotateGizmoXProxy = new DebugPrimitiveRenderableProxy(renderlist, RotateGizmoX);
            RotateGizmoXProxy.BaseColor = Color.FromArgb(0xF3, 0x36, 0x53);
            RotateGizmoXProxy.HighlightedColor = Color.FromArgb(0xFF, 0x66, 0x83);
            RotateGizmoYProxy = new DebugPrimitiveRenderableProxy(renderlist, RotateGizmoY);
            RotateGizmoYProxy.BaseColor = Color.FromArgb(0x86, 0xC8, 0x15);
            RotateGizmoYProxy.HighlightedColor = Color.FromArgb(0xB6, 0xF8, 0x45);
            RotateGizmoZProxy = new DebugPrimitiveRenderableProxy(renderlist, RotateGizmoZ);
            RotateGizmoZProxy.BaseColor = Color.FromArgb(0x38, 0x90, 0xED);
            RotateGizmoZProxy.HighlightedColor = Color.FromArgb(0x68, 0xB0, 0xFF);

            _selection = selection;
        }

        // Code referenced from
        // https://github.com/nem0/LumixEngine/blob/master/src/editor/gizmo.cpp
        Vector3 GetSingleAxisProjection(Ray ray, Transform t, Axis axis)
        {
            Vector3 axisvec = Vector3.Zero;
            switch (axis)
            {
                case Axis.PosX:
                    axisvec = Vector3.Transform(new Vector3(1.0f, 0.0f, 0.0f), t.Rotation);
                    break;
                case Axis.PosY:
                    axisvec = Vector3.Transform(new Vector3(0.0f, 1.0f, 0.0f), t.Rotation);
                    break;
                case Axis.PosZ:
                    axisvec = Vector3.Transform(new Vector3(0.0f, 0.0f, 1.0f), t.Rotation);
                    break;
            }
            var pos = t.Position;
            var normal = Vector3.Cross(Vector3.Cross(ray.Direction, axisvec), ray.Direction);
            float d = Vector3.Dot(ray.Origin - pos, normal) / Vector3.Dot(axisvec, normal);
            return pos + axisvec * d;
        }

        Vector3 GetAxisPlaneProjection(Ray ray, Transform t, Axis axis)
        {
            Vector3 planeNormal = Vector3.Zero;
            switch (axis)
            {
                case Axis.PosX:
                    planeNormal = Vector3.Transform(new Vector3(1.0f, 0.0f, 0.0f), t.Rotation);
                    break;
                case Axis.PosY:
                    planeNormal = Vector3.Transform(new Vector3(0.0f, 1.0f, 0.0f), t.Rotation);
                    break;
                case Axis.PosZ:
                    planeNormal = Vector3.Transform(new Vector3(0.0f, 0.0f, 1.0f), t.Rotation);
                    break;
            }
            float dist;
            Vector3 relorigin = (ray.Origin - t.Position);
            if (Utils.RayPlaneIntersection(relorigin, ray.Direction, Vector3.Zero, planeNormal, out dist))
            {
                return ray.Origin + ray.Direction * dist;
            }
            return ray.Origin;
        }

        public bool IsMouseBusy()
        {
            return IsTransforming;
        }

        Vector3 DebugAxis = Vector3.Zero;
        float DebugAngle = 0.0f;

        private Transform ProjectTransformDelta(Transform objt)
        {
            Quaternion deltaRot = CurrentTransform.Rotation * Quaternion.Inverse(OriginalTransform.Rotation);
            Vector3 deltaPos = CurrentTransform.Position - OriginalTransform.Position;
            objt.Rotation = deltaRot * objt.Rotation;
            var posdif = objt.Position - OriginalTransform.Position;
            posdif = Vector3.Transform(Vector3.Transform(posdif, Quaternion.Conjugate(OriginalTransform.Rotation)), CurrentTransform.Rotation);
            objt.Position = OriginalTransform.Position + posdif;
            objt.Position += deltaPos;
            return objt;
        }

        public void Update(Ray ray, bool canCaptureMouse)
        {
            if (IsTransforming)
            {
                if (!InputTracker.GetMouseButton(MouseButton.Left))
                {
                    IsTransforming = false;
                    var actlist = new List<Action>();
                    foreach (var sel in _selection.GetFilteredSelection<Entity>((o) => o.HasTransform))
                    {
                        sel.ClearTemporaryTransform(false);
                        actlist.Add(sel.GetUpdateTransformAction(ProjectTransformDelta(sel.GetLocalTransform())));
                    }
                    var action = new CompoundAction(actlist);
                    ActionManager.ExecuteAction(action);
                }
                else
                {
                    if (Mode == GizmosMode.Translate)
                    {
                        var delta = GetSingleAxisProjection(ray, OriginalTransform, TransformAxis) - OriginProjection;
                        CurrentTransform.Position = OriginalTransform.Position + delta;
                    }
                    else if (Mode == GizmosMode.Rotate)
                    {
                        Vector3 axis = Vector3.Zero;
                        switch (TransformAxis)
                        {
                            case Axis.PosX:
                                axis = Vector3.Transform(new Vector3(1.0f, 0.0f, 0.0f), OriginalTransform.Rotation);
                                break;
                            case Axis.PosY:
                                axis = Vector3.Transform(new Vector3(0.0f, 1.0f, 0.0f), OriginalTransform.Rotation);
                                break;
                            case Axis.PosZ:
                                axis = Vector3.Transform(new Vector3(0.0f, 0.0f, 1.0f), OriginalTransform.Rotation);
                                break;
                        }
                        axis = Vector3.Normalize(axis);
                        var newproj = GetAxisPlaneProjection(ray, OriginalTransform, TransformAxis);
                        var delta = Vector3.Normalize(newproj - OriginalTransform.Position);
                        var deltaorig = Vector3.Normalize(OriginProjection - OriginalTransform.Position);
                        var side = Vector3.Cross(axis, deltaorig);
                        float y = Math.Max(-1.0f, Math.Min(1.0f, Vector3.Dot(delta, deltaorig)));
                        float x = Math.Max(-1.0f, Math.Min(1.0f, Vector3.Dot(delta, side)));
                        float angle = (float)Math.Atan2(x, y);
                        DebugAxis = axis;
                        DebugAngle = angle;
                        //CurrentTransform.EulerRotation.Y = OriginalTransform.EulerRotation.Y + angle;
                        CurrentTransform.Rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, angle) * OriginalTransform.Rotation);
                    }
                    if (_selection.IsSelection())
                    {
                        //Selection.GetSingleSelection().SetTemporaryTransform(CurrentTransform);
                        foreach (var sel in _selection.GetFilteredSelection<Entity>((o) => o.HasTransform))
                        {
                            sel.SetTemporaryTransform(ProjectTransformDelta(sel.GetLocalTransform()));
                        }
                    }
                }
            }
            if (!IsTransforming)
            {
                if (_selection.IsFilteredSelection<Entity>((o) => o.HasTransform))
                {
                    if (_selection.IsSingleFilteredSelection<Entity>((o) => o.HasTransform))
                    {
                        var sel = _selection.GetSingleFilteredSelection<Entity>((o) => o.HasTransform);
                        OriginalTransform = sel.GetLocalTransform();
                        if (Origin == GizmosOrigin.BoundingBox && sel.RenderSceneMesh != null)
                        {
                            OriginalTransform.Position = sel.RenderSceneMesh.GetBounds().GetCenter();
                        }
                        if (Space == GizmosSpace.World)
                        {
                            OriginalTransform.Rotation = Quaternion.Identity;
                        }
                    }
                    else
                    {
                        // Average the positions of the selections and use a neutral rotation
                        Vector3 accumPos = Vector3.Zero;
                        foreach (var sel in _selection.GetFilteredSelection<Entity>((o) => o.HasTransform))
                        {
                            accumPos += sel.GetLocalTransform().Position;
                        }
                        OriginalTransform = new Transform(accumPos / (float)_selection.GetSelection().Count, Vector3.Zero);
                    }

                    Axis hoveredAxis = Axis.None;
                    switch (Mode)
                    {
                        case GizmosMode.Translate:
                            if (TranslateGizmoX.GetRaycast(ray, TranslateGizmoXProxy.World))
                            {
                                hoveredAxis = Axis.PosX;
                            }
                            else if (TranslateGizmoY.GetRaycast(ray, TranslateGizmoYProxy.World))
                            {
                                hoveredAxis = Axis.PosY;
                            }
                            else if (TranslateGizmoZ.GetRaycast(ray, TranslateGizmoZProxy.World))
                            {
                                hoveredAxis = Axis.PosZ;
                            }
                            TranslateGizmoXProxy.RenderSelectionOutline = (hoveredAxis == Axis.PosX);
                            TranslateGizmoYProxy.RenderSelectionOutline = (hoveredAxis == Axis.PosY);
                            TranslateGizmoZProxy.RenderSelectionOutline = (hoveredAxis == Axis.PosZ);
                            break;
                        case GizmosMode.Rotate:
                            if (RotateGizmoX.GetRaycast(ray, RotateGizmoXProxy.World))
                            {
                                hoveredAxis = Axis.PosX;
                            }
                            else if (RotateGizmoY.GetRaycast(ray, RotateGizmoYProxy.World))
                            {
                                hoveredAxis = Axis.PosY;
                            }
                            else if (RotateGizmoZ.GetRaycast(ray, RotateGizmoZProxy.World))
                            {
                                hoveredAxis = Axis.PosZ;
                            }
                            RotateGizmoXProxy.RenderSelectionOutline = (hoveredAxis == Axis.PosX);
                            RotateGizmoYProxy.RenderSelectionOutline = (hoveredAxis == Axis.PosY);
                            RotateGizmoZProxy.RenderSelectionOutline = (hoveredAxis == Axis.PosZ);
                            break;
                    }
                    

                    if (canCaptureMouse && InputTracker.GetMouseButtonDown(MouseButton.Left))
                    {
                        if (hoveredAxis != Axis.None)
                        {
                            IsTransforming = true;
                            TransformAxis = hoveredAxis;
                            CurrentTransform = OriginalTransform;
                            if (Mode == GizmosMode.Rotate)
                            {
                                OriginProjection = GetAxisPlaneProjection(ray, OriginalTransform, TransformAxis);
                            }
                            else
                            {
                                OriginProjection = GetSingleAxisProjection(ray, OriginalTransform, TransformAxis);
                            }
                        }
                    }
                }
            }

            // Update gizmos transform and visibility
            if (_selection.IsSelection())
            {
                //var selected = MsbEditor.Selection.Selected;
                //var center = selected.RenderSceneMesh.GetBounds().GetCenter();
                //var center = selected.RenderSceneMesh.WorldMatrix.Translation;
                Vector3 center;
                Quaternion rot;
                if (IsTransforming)
                {
                    center = CurrentTransform.Position;
                    rot = CurrentTransform.Rotation;
                }
                else
                {
                    center = OriginalTransform.Position;
                    rot = OriginalTransform.Rotation;
                }
                float dist = (center - CameraPosition).Length();
                Vector3 scale = new Vector3(dist * 0.04f);
                TranslateGizmoXProxy.World = new Transform(center, rot, scale).WorldMatrix;
                TranslateGizmoYProxy.World = new Transform(center, rot, scale).WorldMatrix;
                TranslateGizmoZProxy.World = new Transform(center, rot, scale).WorldMatrix;
                RotateGizmoXProxy.World = new Transform(center, rot, scale).WorldMatrix;
                RotateGizmoYProxy.World = new Transform(center, rot, scale).WorldMatrix;
                RotateGizmoZProxy.World = new Transform(center, rot, scale).WorldMatrix;

                if (Mode == GizmosMode.Translate)
                {
                    TranslateGizmoXProxy.Visible = true;
                    TranslateGizmoYProxy.Visible = true;
                    TranslateGizmoZProxy.Visible = true;
                    RotateGizmoXProxy.Visible = false;
                    RotateGizmoYProxy.Visible = false;
                    RotateGizmoZProxy.Visible = false;
                }
                else
                {
                    TranslateGizmoXProxy.Visible = false;
                    TranslateGizmoYProxy.Visible = false;
                    TranslateGizmoZProxy.Visible = false;
                    RotateGizmoXProxy.Visible = true;
                    RotateGizmoYProxy.Visible = true;
                    RotateGizmoZProxy.Visible = true;
                }
            }
            else
            {
                TranslateGizmoXProxy.Visible = false;
                TranslateGizmoYProxy.Visible = false;
                TranslateGizmoZProxy.Visible = false;
                RotateGizmoXProxy.Visible = false;
                RotateGizmoYProxy.Visible = false;
                RotateGizmoZProxy.Visible = false;
            }
        }

        public void DebugGui()
        {
            ImGui.Begin("GizmosDebug");
            ImGui.Text($@"Axis: {DebugAxis}");
            ImGui.Text($@"Angle: {DebugAngle}");
            ImGui.End();
        }

        public void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            TranslateGizmoX.CreateDeviceObjects(gd, cl, sp);
            TranslateGizmoY.CreateDeviceObjects(gd, cl, sp);
            TranslateGizmoZ.CreateDeviceObjects(gd, cl, sp);
            RotateGizmoX.CreateDeviceObjects(gd, cl, sp);
            RotateGizmoY.CreateDeviceObjects(gd, cl, sp);
            RotateGizmoZ.CreateDeviceObjects(gd, cl, sp);
        }

        public void DestroyDeviceObjects()
        {
            TranslateGizmoX.DestroyDeviceObjects();
            TranslateGizmoY.DestroyDeviceObjects();
            TranslateGizmoZ.DestroyDeviceObjects();
            RotateGizmoX.DestroyDeviceObjects();
            RotateGizmoY.DestroyDeviceObjects();
            RotateGizmoZ.DestroyDeviceObjects();
        }

        /*public void Render(Renderer.IndirectDrawEncoder encoder, SceneRenderPipeline sp)
        {
            if (_selection.IsSelection())
            {
                switch (Mode)
                {
                    case GizmosMode.Translate:
                        TranslateGizmoX.Render(encoder, sp);
                        TranslateGizmoY.Render(encoder, sp);
                        TranslateGizmoZ.Render(encoder, sp);
                        break;
                    case GizmosMode.Rotate:
                        RotateGizmoX.Render(encoder, sp);
                        RotateGizmoY.Render(encoder, sp);
                        RotateGizmoZ.Render(encoder, sp);
                        break;
                }
            }
        }

        public void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (_selection.IsSelection())
            {
                //var selected = MsbEditor.Selection.Selected;
                //var center = selected.RenderSceneMesh.GetBounds().GetCenter();
                //var center = selected.RenderSceneMesh.WorldMatrix.Translation;
                Vector3 center;
                Quaternion rot;
                if (IsTransforming)
                {
                    center = CurrentTransform.Position;
                    rot = CurrentTransform.Rotation;
                }
                else
                {
                    center = OriginalTransform.Position;
                    rot = OriginalTransform.Rotation;
                }
                float dist = (center - CameraPosition).Length();
                Vector3 scale = new Vector3(dist * 0.04f);
                TranslateGizmoX.Transform = new Transform(center, rot, scale);
                TranslateGizmoY.Transform = new Transform(center, rot, scale);
                TranslateGizmoZ.Transform = new Transform(center, rot, scale);
                RotateGizmoX.Transform = new Transform(center, rot, scale);
                RotateGizmoY.Transform = new Transform(center, rot, scale);
                RotateGizmoZ.Transform = new Transform(center, rot, scale);
            }
            TranslateGizmoX.UpdatePerFrameResources(gd, cl, sp);
            TranslateGizmoY.UpdatePerFrameResources(gd, cl, sp);
            TranslateGizmoZ.UpdatePerFrameResources(gd, cl, sp);
            RotateGizmoX.UpdatePerFrameResources(gd, cl, sp);
            RotateGizmoY.UpdatePerFrameResources(gd, cl, sp);
            RotateGizmoZ.UpdatePerFrameResources(gd, cl, sp);
        }*/
    }
}
