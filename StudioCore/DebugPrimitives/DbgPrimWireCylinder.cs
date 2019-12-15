using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireCylinder : DbgPrimWire
    {
        public DbgPrimWireCylinder(Transform location, float range, float height, int numSegments, Color color)
        {
            NameColor = color;
            Transform = location;

            float top = height / 2;
            float bottom = -top;

            for (int i = 0; i < numSegments; i++)
            {
                float angle = (1.0f * i / numSegments) * Utils.Pi * 2.0f;
                float x = (float)Math.Cos(angle);
                float z = (float)Math.Sin(angle);
                //Very last one wraps around to the first one
                if (i == numSegments - 1)
                {
                    float x_next = (float)Math.Cos(0);
                    float z_next = (float)Math.Sin(0);
                    // Create line to wrap around to first point
                    //---- Top
                    AddLine(new Vector3(x, top, z), new Vector3(x_next, top, z_next), color);
                    //---- Bottom
                    AddLine(new Vector3(x, bottom, z), new Vector3(x_next, bottom, z_next), color);
                }
                else
                {
                    float angle_next = (1.0f * (i + 1) / numSegments) * Utils.Pi * 2.0f;
                    float x_next = (float)Math.Cos(angle_next);
                    float z_next = (float)Math.Sin(angle_next);

                    // Create line to next point in ring
                    //---- Top
                    AddLine(new Vector3(x, top, z), new Vector3(x_next, top, z_next), color);
                    //---- Bottom
                    AddLine(new Vector3(x, bottom, z), new Vector3(x_next, bottom, z_next), color);
                }

                // Make pillar from top to bottom
                AddLine(new Vector3(x, bottom, z), new Vector3(x, top, z), color);
            }
        }
    }
}
