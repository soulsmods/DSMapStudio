using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpPairCollisionFilter : hknpCollisionFilter
    {
        public hknpCollisionFilter m_childFilter;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt64();
            m_childFilter = des.ReadClassPointer<hknpCollisionFilter>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
