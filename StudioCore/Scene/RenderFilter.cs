using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Scene
{
    [Flags]
    public enum RenderFilter
    {
        Debug = 1,
        Editor = 2,
        MapPiece = 4,
        Collision = 8,
        Character = 16,
        Object = 32,
        Navmesh = 64,
        Region = 128,
        All = 0xFFFFFFF,
    }
}
