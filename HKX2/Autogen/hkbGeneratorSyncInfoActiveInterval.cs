using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGeneratorSyncInfoActiveInterval : IHavokObject
    {
        public hkbGeneratorSyncInfoSyncPoint m_syncPoints;
        public float m_fraction;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_syncPoints = new hkbGeneratorSyncInfoSyncPoint();
            m_syncPoints.Read(des, br);
            br.AssertUInt64(0);
            m_fraction = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_syncPoints.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_fraction);
        }
    }
}
