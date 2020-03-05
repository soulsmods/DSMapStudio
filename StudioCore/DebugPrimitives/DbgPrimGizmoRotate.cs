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
    public class DbgPrimGizmoRotate : DbgPrimGizmo
    {
        public const int Segments = 12;
        public const float TotalLength = 5.0f;
        public const float TipLength = 1.25f;
        public const float TipRadius = 0.25f;
        public const float StemRadius = 0.075f;

        public const float Radius = 3.0f;
        public const float RingRadius = 0.03f;
        public const int RingCount = 30;
        public const int SideCount = 4;


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

        public DbgPrimGizmoRotate()
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

                // Algorithm from
                // http://apparat-engine.blogspot.com/2013/04/procedural-meshes-torus.html
                void Ring(MsbEditor.Gizmos.Axis axis, Color color)
                {
                    var collist = AxisToList(axis);

                    List<Vector3> vertices = new List<Vector3>((RingCount + 1) * (SideCount + 1));
                    List<Vector3> colvertices = new List<Vector3>((RingCount + 1) * (SideCount + 1));
                    float theta = 0.0f;
                    float phi = 0.0f;
                    float verticalStride = (float)(Math.PI * 2.0) / (float)RingCount;
                    float horizontalStride = (float)(Math.PI * 2.0f) / (float)SideCount;
                    for (int i = 0; i < RingCount + 1; i++)
                    {
                        theta = verticalStride * i;
                        for (int j = 0; j < SideCount + 1; j++)
                        {
                            phi = horizontalStride * j;
                            float cosphi = (float)Math.Cos(phi);
                            float sinphi = (float)Math.Sin(phi);
                            float costheta = (float)Math.Cos(theta);
                            float sintheta = (float)Math.Sin(theta);
                            float a = costheta * (Radius + RingRadius * cosphi);
                            float b = sintheta * (Radius + RingRadius * cosphi);
                            float c = RingRadius * sinphi;
                            float ca = costheta * (Radius + RingRadius * 7.0f * cosphi);
                            float cb = sintheta * (Radius + RingRadius * 7.0f * cosphi);
                            float cc = RingRadius * 7.0f * sinphi;

                            if (axis == MsbEditor.Gizmos.Axis.PosX)
                            {
                                vertices.Add(new Vector3(c, a, b));
                                colvertices.Add(new Vector3(cc, ca, cb));
                            }
                            if (axis == MsbEditor.Gizmos.Axis.PosY)
                            {
                                vertices.Add(new Vector3(a, c, b));
                                colvertices.Add(new Vector3(ca, cc, cb));
                            }
                            if (axis == MsbEditor.Gizmos.Axis.PosZ)
                            {
                                vertices.Add(new Vector3(a, b, c));
                                colvertices.Add(new Vector3(ca, cb, cc));
                            }
                        }
                    }

                    for (int i = 0; i < RingCount; i++)
                    {
                        for (int j = 0; j < SideCount; j++)
                        {
                            int a = (j + i * (SideCount + 1));
                            int b = ((j + 1) + i * (SideCount + 1));
                            int c = (j  + (i + 1) * (SideCount + 1));
                            int d = ((j + 1) + (i + 1) * (SideCount + 1));
                            //AddQuad(vertices[a], vertices[b], vertices[c], vertices[d], color);
                            AddTri(vertices[a], vertices[b], vertices[c], color);
                            AddTri(vertices[b], vertices[d], vertices[c], color);
                            AddColTri(collist, colvertices[a], colvertices[b], colvertices[c]);
                            AddColTri(collist, colvertices[b], colvertices[d], colvertices[c]);
                        }
                    }
                }

                Ring(MsbEditor.Gizmos.Axis.PosX, Color.Red);
                Ring(MsbEditor.Gizmos.Axis.PosZ, Color.Blue);
                Ring(MsbEditor.Gizmos.Axis.PosY, Color.Green);

                //FinalizeBuffers(true);

                GeometryData = new DbgPrimGeometryData()
                {
                    GeomBuffer = GeomBuffer,
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
