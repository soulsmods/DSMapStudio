using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace StudioCore.DebugPrimitives
{
    public class DbgPrimWireGrid : DbgPrimWire
    {
        public DbgPrimWireGrid(Color originColor, Color color, int unitRange, float unitSize)
        {
            NameColor = color;
            for (int h = -unitRange; h <= unitRange; h++)
            {
                AddLine(new Vector3(h, 0, unitRange) * unitSize,
                    new Vector3(h, 0, -unitRange) * unitSize, h == 0 ? originColor : color);
            }

            for (int v = -unitRange; v <= unitRange; v++)
            {
                AddLine(new Vector3(-unitRange, 0, v) * unitSize, 
                    new Vector3(unitRange, 0, v) * unitSize, v == 0 ? originColor : color);
            }
        }
    }
}
