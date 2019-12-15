using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireBox : DbgPrimWire
    {
        public Vector3 LocalMax = Vector3.One / 2;
        public Vector3 LocalMin = -Vector3.One / 2;

        public void UpdateTransform(Transform newTransform)
        {
            Vector3 center = ((LocalMax + LocalMin) / 2);
            Vector3 size = (LocalMax - LocalMin) / 2;

            Transform = new Transform(Matrix4x4.CreateScale(size) 
                * Matrix4x4.CreateTranslation(center) 
                * newTransform.WorldMatrix);
        }

        private DbgPrimGeometryData GeometryData = null;

        public DbgPrimWireBox(Transform location, Vector3 localMin, Vector3 localMax, Color color)
        {
            Transform = location;
            NameColor = color;
            OverrideColor = color;

            LocalMin = localMin;
            LocalMax = localMax;

            if (GeometryData != null)
            {
                SetBuffers(GeometryData.VertBuffer, GeometryData.IndexBuffer);
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
                AddLine(tfr, tbr, color);
                AddLine(tbr, tbl, color);
                AddLine(tbl, tfl, color);

                // Bottom Face
                AddLine(bfl, bfr, color);
                AddLine(bfr, bbr, color);
                AddLine(bbr, bbl, color);
                AddLine(bbl, bfl, color);

                // Four Vertical Pillars
                AddLine(bfl, tfl, color);
                AddLine(bfr, tfr, color);
                AddLine(bbl, tbl, color);
                AddLine(bbr, tbr, color);

                //FinalizeBuffers(true);

                GeometryData = new DbgPrimGeometryData()
                {
                    VertBuffer = VertBuffer,
                    IndexBuffer = IndexBuffer,
                };
            }
        }
    }
}
