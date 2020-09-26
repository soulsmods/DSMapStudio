using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpDummyShape : hknpShape
    {
        public hkAabb m_aabb;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            br.ReadUInt64();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_aabb.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
