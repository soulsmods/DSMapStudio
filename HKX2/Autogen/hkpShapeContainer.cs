using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpShapeContainer : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.ReadUInt64();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
        }
    }
}
