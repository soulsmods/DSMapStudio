using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGeneratorSyncInfoActiveInterval : IHavokObject
    {
        public hkbGeneratorSyncInfoSyncPoint m_syncPoints_0;
        public hkbGeneratorSyncInfoSyncPoint m_syncPoints_1;
        public float m_fraction;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_syncPoints_0 = new hkbGeneratorSyncInfoSyncPoint();
            m_syncPoints_0.Read(des, br);
            m_syncPoints_1 = new hkbGeneratorSyncInfoSyncPoint();
            m_syncPoints_1.Read(des, br);
            m_fraction = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_syncPoints_0.Write(bw);
            m_syncPoints_1.Write(bw);
            bw.WriteSingle(m_fraction);
        }
    }
}
