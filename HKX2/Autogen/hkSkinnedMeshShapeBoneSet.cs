using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSkinnedMeshShapeBoneSet : IHavokObject
    {
        public ushort m_boneBufferOffset;
        public ushort m_numBones;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_boneBufferOffset = br.ReadUInt16();
            m_numBones = br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_boneBufferOffset);
            bw.WriteUInt16(m_numBones);
        }
    }
}
