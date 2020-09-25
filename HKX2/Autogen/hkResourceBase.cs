using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkResourceBase : hkReferencedObject
    {
        public enum Type
        {
            TYPE_RESOURCE = 0,
            TYPE_CONTAINER = 1,
        }
        
        
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
