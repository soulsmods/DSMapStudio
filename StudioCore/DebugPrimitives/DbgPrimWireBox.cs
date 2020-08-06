using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using Veldrid.Utilities;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireBox : DbgPrimWire
    {
        public Vector3 LocalMax = Vector3.One / 2;
        public Vector3 LocalMin = -Vector3.One / 2;

        private List<(Vector3, Vector3)> Lines = new List<(Vector3, Vector3)>();

        public void UpdateTransform(Transform newTransform)
        {
            Vector3 center = ((LocalMax + LocalMin) / 2);
            Vector3 size = (LocalMax - LocalMin) / 2;

            //Transform = new Transform(Matrix4x4.CreateScale(size) 
            //    * Matrix4x4.CreateTranslation(center) 
            //    * newTransform.WorldMatrix);
        }

        private DbgPrimGeometryData GeometryData = null;

        public DbgPrimWireBox(Transform location, Vector3 localMin, Vector3 localMax, Color color)
        {
            NameColor = color;

            LocalMin = localMin;
            LocalMax = localMax;

            if (GeometryData != null)
            {
                SetBuffers(GeometryData.GeomBuffer);
            }
            else
            {
                //var min = -Vector3.One;
                //var max = Vector3.One;
                var min = LocalMin;
                var max = LocalMax;

                // 3 Letters of below names: 
                // [T]op/[B]ottom, [F]ront/[B]ack, [L]eft/[R]ight
                var tfl = new Vector3(min.X, max.Y, max.Z);
                var tfr = new Vector3(max.X, max.Y, max.Z);
                var bfr = new Vector3(max.X, min.Y, max.Z);
                var bfl = new Vector3(min.X, min.Y, max.Z);
                var tbl = new Vector3(min.X, max.Y, min.Z);
                var tbr = new Vector3(max.X, max.Y, min.Z);
                var bbr = new Vector3(max.X, min.Y, min.Z);
                var bbl = new Vector3(min.X, min.Y, min.Z);

                // Top Face
                AddLine(tfl, tfr, color);
                Lines.Add((tfl, tfr));
                AddLine(tfr, tbr, color);
                Lines.Add((tfr, tbr));
                AddLine(tbr, tbl, color);
                Lines.Add((tbr, tbl));
                AddLine(tbl, tfl, color);
                Lines.Add((tbl, tfl));

                // Bottom Face
                AddLine(bfl, bfr, color);
                Lines.Add((bfl, bfr));
                AddLine(bfr, bbr, color);
                Lines.Add((bfr, bbr));
                AddLine(bbr, bbl, color);
                Lines.Add((bbr, bbl));
                AddLine(bbl, bfl, color);
                Lines.Add((bbl, bfl));

                // Four Vertical Pillars
                AddLine(bfl, tfl, color);
                Lines.Add((bfl, tfl));
                AddLine(bfr, tfr, color);
                Lines.Add((bfr, tfr));
                AddLine(bbl, tbl, color);
                Lines.Add((bbl, tbl));
                AddLine(bbr, tbr, color);
                Lines.Add((bbr, tbr));

                //FinalizeBuffers(true);

                GeometryData = new DbgPrimGeometryData()
                {
                    GeomBuffer = GeometryBuffer
                };

                Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                {
                    UpdatePerFrameResources(d, cl, null);
                });
            }
        }

        public bool RayCast(Ray ray, Matrix4x4 transform, out float dist)
        {
            Vector3 scale;
            Matrix4x4 transformNoScale;
            Utils.ExtractScale(transform, out scale, out transformNoScale);
            var invw = transformNoScale.Inverse();
            var newo = Vector3.Transform(ray.Origin, invw);
            var newd = Vector3.TransformNormal(ray.Direction, invw);
            var tray = new Ray(newo, newd);
            if (tray.Intersects(new BoundingBox(LocalMin * scale, LocalMax * scale)))
            {
                foreach (var line in Lines)
                {
                    var a = line.Item1 * scale;
                    var b = line.Item2 * scale;
                    var c = a + b / 2.0f;
                    var dir = Vector3.Normalize(b - a);
                    var mag = new Vector3(Vector3.Dot(dir, Vector3.UnitY) + Vector3.Dot(dir, Vector3.UnitZ),
                                          Vector3.Dot(dir, Vector3.UnitX) + Vector3.Dot(dir, Vector3.UnitZ),
                                          Vector3.Dot(dir, Vector3.UnitX) + Vector3.Dot(dir, Vector3.UnitY));
                    var tol = 0.008f * Vector3.Distance(newo, c);
                    var bb = new BoundingBox(a - mag * tol, b + mag * tol);
                    if (Utils.RayBoxIntersection(ref tray, ref bb, out dist))
                    {
                        if (dist > 0.0f)
                        {
                            return true;
                        }
                    }
                }
            }
            dist = float.MaxValue;
            return false;
        }
    }
}
