using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinnedMeshShapeBoneSection : IHavokObject
    {
        public hkMeshShape m_meshBuffer;
        public ushort m_startBoneSetId;
        public short m_numBoneSets;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_meshBuffer = des.ReadClassPointer<hkMeshShape>(br);
            m_startBoneSetId = br.ReadUInt16();
            m_numBoneSets = br.ReadInt16();
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt16(m_startBoneSetId);
            bw.WriteInt16(m_numBoneSets);
            bw.WriteUInt32(0);
        }
    }
}
