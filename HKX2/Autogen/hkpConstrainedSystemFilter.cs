using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpConstrainedSystemFilter : hkpCollisionFilter
    {
        public hkpCollisionFilter m_otherFilter;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_otherFilter = des.ReadClassPointer<hkpCollisionFilter>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
