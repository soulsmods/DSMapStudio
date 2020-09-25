using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBroadPhaseHandle : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(0);
        }
    }
}
