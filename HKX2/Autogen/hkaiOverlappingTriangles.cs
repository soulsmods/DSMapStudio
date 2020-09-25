using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum WalkableTriangleSettings
    {
        ONLY_FIX_WALKABLE = 0,
        PREFER_WALKABLE = 1,
        PREFER_UNWALKABLE = 2,
    }
    
    public class hkaiOverlappingTriangles : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(0);
        }
    }
}
