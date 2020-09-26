using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ShapeDispatchTypeEnum
    {
        CONVEX_IMPLICIT = 0,
        CONVEX = 1,
        HEIGHT_FIELD = 2,
        COMPOSITE = 3,
        USER = 4,
        NUM_DISPATCH_TYPES = 5,
    }
    
    public class hkcdShapeDispatchType : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(0);
        }
    }
}
