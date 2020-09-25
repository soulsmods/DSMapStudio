using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdPlanarGeometryPrimitivesPlane : IHavokObject
    {
        public long m_iEqn;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_iEqn = br.ReadInt64();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt64(m_iEqn);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
