using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Space
    {
        USE_LOCAL_SPACE = 0,
        USE_WORLD_SPACE = 1,
    }
    
    public class hknpSurfaceVelocity : hkReferencedObject
    {
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
