using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiPersistentEdgeKey : IHavokObject
    {
        public hkaiPersistentFaceKey m_faceKey;
        public short m_edgeOffset;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_faceKey = new hkaiPersistentFaceKey();
            m_faceKey.Read(des, br);
            m_edgeOffset = br.ReadInt16();
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_faceKey.Write(bw);
            bw.WriteInt16(m_edgeOffset);
            bw.WriteUInt16(0);
        }
    }
}
