using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireRay : DbgPrimWire
    {

        public void UpdateTransform(Transform newTransform)
        {
        }

        private DbgPrimGeometryData GeometryData = null;

        public DbgPrimWireRay(Transform location, Vector3 start, Vector3 end, Color color)
        {
            Transform = location;
            NameColor = color;


            if (GeometryData != null)
            {
                SetBuffers(GeometryData.GeomBuffer);
            }
            else
            {
                // 3 Letters of below names: 
                // [T]op/[B]ottom, [F]ront/[B]ack, [L]eft/[R]ight

                // Top Face
                AddLine(start, end, color);

                //FinalizeBuffers(true);

                GeometryData = new DbgPrimGeometryData()
                {
                    GeomBuffer = GeomBuffer
                };
            }
        }
    }
}
