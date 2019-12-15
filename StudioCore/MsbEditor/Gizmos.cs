using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using StudioCore.Scene;
using Veldrid;
using Veldrid.Utilities;
using ImGuiNET;

namespace StudioCore.MsbEditor
{
    public class Gizmos : Scene.RenderObject
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

        private DebugPrimitives.DbgPrimGizmoTranslate TranslateGizmo;
        private DebugPrimitives.DbgPrimGizmoRotate RotateGizmo;

        private Transform OriginalTransform = Transform.Default;
        private Transform CurrentTransform = Transform.Default;
        private Vector3 OriginProjection = new Vector3();
        private bool IsTransforming = false;
        private Axis TransformAxis = Axis.None;
        private ActionManager ActionManager;

        public Vector3 CameraPosition { get; set; }

        public Gizmos(ActionManager am)
        {
            ActionManager = am;
            TranslateGizmo = new DebugPrimitives.DbgPrimGizmoTranslate();
            RotateGizmo = new DebugPrimitives.DbgPrimGizmoRotate();
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

        public void Update(Ray ray)
        {
            if (!IsTransforming)
            {
                if (Selection.IsFilteredSelection<MapObject>())
                {
                    if (Selection.IsSingleFilteredSelection<MapObject>())
                    {
                        var sel = Selection.GetSingleFilteredSelection<MapObject>();
                        OriginalTransform = sel.GetTransform();
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
                        foreach (var sel in Selection.GetFilteredSelection<MapObject>())
                        {
                            accumPos += sel.GetTransform().Position;
                        }
                        OriginalTransform = new Transform(accumPos / (float)Selection.GetSelection().Count, Vector3.Zero);
                    }
                    if (!ImGuiNET.ImGui.GetIO().WantCaptureMouse && InputTracker.GetMouseButtonDown(MouseButton.Left))
                    {
                        Axis res = Axis.None;
                        switch (Mode)
                        {
                            case GizmosMode.Translate:
                                res = TranslateGizmo.GetRaycast(ray);
                                break;
                            case GizmosMode.Rotate:
                                res = RotateGizmo.GetRaycast(ray);
                                break;
                        }
                        if (res != Axis.None)
                        {
                            IsTransforming = true;
                            TransformAxis = res;
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
            else
            {
                if (!InputTracker.GetMouseButton(MouseButton.Left))
                {
                    IsTransforming = false;
                    /*if (Selection.IsSingleSelection())
                    {
                        Selection.GetSingleSelection().ClearTemporaryTransform();
                        var act = Selection.GetSingleSelection().GetUpdateTransformAction(CurrentTransform);
                        ActionManager.ExecuteAction(act);
                    }*/
                    var actlist = new List<Action>();
                    foreach (var sel in Selection.GetFilteredSelection<MapObject>())
                    {
                        sel.ClearTemporaryTransform();
                        actlist.Add(sel.GetUpdateTransformAction(ProjectTransformDelta(sel.GetTransform())));
                    }
                    var action = new CompoundAction(actlist);
                    ActionManager.ExecuteAction(action);
                    return;
                }
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
                if (Selection.IsSelection())
                {
                    //Selection.GetSingleSelection().SetTemporaryTransform(CurrentTransform);
                    foreach (var sel in Selection.GetFilteredSelection<MapObject>())
                    {
                        sel.SetTemporaryTransform(ProjectTransformDelta(sel.GetTransform()));
                    }
                }
            }
        }

        public void DebugGui()
        {
            ImGui.Begin("GizmosDebug");
            ImGui.Text($@"Axis: {DebugAxis}");
            ImGui.Text($@"Angle: {DebugAngle}");
            ImGui.End();
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            TranslateGizmo.CreateDeviceObjects(gd, cl, sp);
            RotateGizmo.CreateDeviceObjects(gd, cl, sp);
        }

        public override void DestroyDeviceObjects()
        {
            TranslateGizmo.DestroyDeviceObjects();
            RotateGizmo.DestroyDeviceObjects();
        }

        public override void Render(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (MsbEditor.Selection.IsSelection())
            {
                switch (Mode)
                {
                    case GizmosMode.Translate:
                        TranslateGizmo.Render(gd, cl, sp);
                        break;
                    case GizmosMode.Rotate:
                        RotateGizmo.Render(gd, cl, sp);
                        break;
                }
            }
        }

        public override void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (MsbEditor.Selection.IsSelection())
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
                TranslateGizmo.Transform = new Transform(center, rot, scale);
                RotateGizmo.Transform = new Transform(center, rot, scale);
            }
            TranslateGizmo.UpdatePerFrameResources(gd, cl, sp);
            RotateGizmo.UpdatePerFrameResources(gd, cl, sp);
        }
    }
}
