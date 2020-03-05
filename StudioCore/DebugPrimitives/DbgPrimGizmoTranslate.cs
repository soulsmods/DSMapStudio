using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;
using Veldrid.Utilities;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimGizmoTranslate : DbgPrimGizmo
    {
        public const int Segments = 12;
        public const float TotalLength = 5.0f;
        public const float TipLength = 1.25f;
        public const float TipRadius = 0.25f;
        public const float StemRadius = 0.075f;

        private DbgPrimGeometryData GeometryData = null;

        private List<Vector3> XTris = new List<Vector3>();
        private List<Vector3> YTris = new List<Vector3>();
        private List<Vector3> ZTris = new List<Vector3>();

        public enum Axis
        {
            None,
            PosX,
            PosY,
            PosZ
        }

        private void AddColTri(List<Vector3> list, Vector3 a, Vector3 b, Vector3 c)
        {
            list.Add(a);
            list.Add(b);
            list.Add(c);
        }

        private void AddColQuad(List<Vector3> list, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            AddColTri(list, a, b, c);
            AddColTri(list, a, c, d);
        }

        private List<Vector3> AxisToList(MsbEditor.Gizmos.Axis axis)
        {
            switch (axis)
            {
                case MsbEditor.Gizmos.Axis.PosX:
                    return XTris;
                case MsbEditor.Gizmos.Axis.PosY:
                    return YTris;
                case MsbEditor.Gizmos.Axis.PosZ:
                    return ZTris;
            }
            return null;
        }

        public DbgPrimGizmoTranslate()
        {
            //BackfaceCulling = false;

            Transform = Transform.Default;
            //NameColor = color;
            //Name = name;

            if (GeometryData != null)
            {
                SetBuffers(GeometryData.GeomBuffer);
            }
            else
            {
                Vector3 GetPoint(MsbEditor.Gizmos.Axis axis, int segmentIndex, float radius, float depth)
                {
                    float horizontalAngle = (1.0f * segmentIndex / Segments) * Utils.Pi * 2.0f;
                    float x = (float)Math.Cos(horizontalAngle);
                    float y = (float)Math.Sin(horizontalAngle);
                    if (axis == MsbEditor.Gizmos.Axis.PosZ)
                    {
                        return new Vector3(x * radius, y * radius, depth);
                    }
                    if (axis == MsbEditor.Gizmos.Axis.PosX)
                    {
                        return new Vector3(depth, x * radius, y * radius);
                    }
                    return new Vector3(x * radius, depth, y * radius);
                }

                void Arrow(MsbEditor.Gizmos.Axis axis, Color color)
                {

                    Vector3 tip = Vector3.Zero;
                    if (axis == MsbEditor.Gizmos.Axis.PosX)
                    {
                        tip = Vector3.UnitX * TotalLength;
                    }
                    if (axis == MsbEditor.Gizmos.Axis.PosY)
                    {
                        tip = Vector3.UnitY * TotalLength;
                    }
                    if (axis == MsbEditor.Gizmos.Axis.PosZ)
                    {
                        tip = Vector3.UnitZ * TotalLength;
                    }

                    var collist = AxisToList(axis);

                    for (int i = 1; i <= Segments; i++)
                    {
                        var ptBase = GetPoint(axis, i, StemRadius, 0);

                        var prevPtBase = GetPoint(axis, i - 1, StemRadius, 0);

                        //Face: Part of Bottom
                        AddTri(Vector3.Zero, prevPtBase, ptBase, color);
                        AddColTri(collist, Vector3.Zero, prevPtBase, ptBase);
                    }

                    for (int i = 1; i <= Segments; i++)
                    {
                        var ptBase = GetPoint(axis, i, StemRadius, 0);
                        var ptTipStartInner = GetPoint(axis, i, StemRadius, TotalLength - TipLength);
                        var ptBaseCol = GetPoint(axis, i, TipRadius * 1.25f, 0);
                        var ptTipStartInnerCol = GetPoint(axis, i, TipRadius * 1.25f, TotalLength - TipLength);

                        var prevPtBase = GetPoint(axis, i - 1, StemRadius, 0);
                        var prevPtTipStartInner = GetPoint(axis, i - 1, StemRadius, TotalLength - TipLength);
                        var prevPtBaseCol = GetPoint(axis, i - 1, TipRadius * 1.25f, 0);
                        var prevPtTipStartInnerCol = GetPoint(axis, i - 1, TipRadius * 1.25f, TotalLength - TipLength);

                        // Face: Base to Tip Inner
                        AddQuad(prevPtBase, prevPtTipStartInner, ptTipStartInner, ptBase, color);
                        AddColQuad(collist, prevPtBaseCol, prevPtTipStartInnerCol, ptTipStartInnerCol, ptBaseCol);
                    }

                    for (int i = 1; i <= Segments; i++)
                    {
                        var ptTipStartInner = GetPoint(axis, i, StemRadius, TotalLength - TipLength);
                        var ptTipStartOuter = GetPoint(axis, i, TipRadius, TotalLength - TipLength);

                        var prevPtTipStartInner = GetPoint(axis, i - 1, StemRadius, TotalLength - TipLength);
                        var prevPtTipStartOuter = GetPoint(axis, i - 1, TipRadius, TotalLength - TipLength);

                        // Face: Tip Start Inner to Tip Start Outer
                        AddQuad(prevPtTipStartInner, prevPtTipStartOuter, ptTipStartOuter, ptTipStartInner, color);
                        AddColQuad(collist, prevPtTipStartInner, prevPtTipStartOuter, ptTipStartOuter, ptTipStartInner);
                    }

                    for (int i = 1; i <= Segments; i++)
                    {
                        var ptTipStartOuter = GetPoint(axis, i, TipRadius, TotalLength - TipLength);
                        var prevPtTipStartOuter = GetPoint(axis, i - 1, TipRadius, TotalLength - TipLength);
                        var ptTipStartOuterCol = GetPoint(axis, i, TipRadius * 1.25f, TotalLength - TipLength);
                        var prevPtTipStartOuterCol = GetPoint(axis, i - 1, TipRadius * 1.25f, TotalLength - TipLength);

                        // Face: Tip Start to Tip
                        AddTri(prevPtTipStartOuter, tip, ptTipStartOuter, color);
                        AddColTri(collist, prevPtTipStartOuterCol, tip * 1.10f, ptTipStartOuterCol);
                    }
                }

                Arrow(MsbEditor.Gizmos.Axis.PosX, Color.FromArgb(255, 60, 60));
                Arrow(MsbEditor.Gizmos.Axis.PosZ, Color.FromArgb(60, 60, 255));
                Arrow(MsbEditor.Gizmos.Axis.PosY, Color.FromArgb(60, 255, 60));

                //FinalizeBuffers(true);

                GeometryData = new DbgPrimGeometryData()
                {
                    GeomBuffer = GeomBuffer
                };
            }
        }

        public MsbEditor.Gizmos.Axis GetRaycast(Ray ray)
        {
            for (int i = 0; i < XTris.Count() / 3; i++)
            {
                var a = Vector3.Transform(XTris[i * 3], Transform.WorldMatrix);
                var b = Vector3.Transform(XTris[i * 3 + 1], Transform.WorldMatrix);
                var c = Vector3.Transform(XTris[i * 3 + 2], Transform.WorldMatrix);
                float dist;
                if (ray.Intersects(ref a, ref b, ref c, out dist))
                {
                    return MsbEditor.Gizmos.Axis.PosX;
                }
            }
            for (int i = 0; i < YTris.Count() / 3; i++)
            {
                var a = Vector3.Transform(YTris[i * 3], Transform.WorldMatrix);
                var b = Vector3.Transform(YTris[i * 3 + 1], Transform.WorldMatrix);
                var c = Vector3.Transform(YTris[i * 3 + 2], Transform.WorldMatrix);
                float dist;
                if (ray.Intersects(ref a, ref b, ref c, out dist))
                {
                    return MsbEditor.Gizmos.Axis.PosY;
                }
            }
            for (int i = 0; i < ZTris.Count() / 3; i++)
            {
                var a = Vector3.Transform(ZTris[i * 3], Transform.WorldMatrix);
                var b = Vector3.Transform(ZTris[i * 3 + 1], Transform.WorldMatrix);
                var c = Vector3.Transform(ZTris[i * 3 + 2], Transform.WorldMatrix);
                float dist;
                if (ray.Intersects(ref a, ref b, ref c, out dist))
                {
                    return MsbEditor.Gizmos.Axis.PosZ;
                }
            }
            return MsbEditor.Gizmos.Axis.None;
        }
    }
}
