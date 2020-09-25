using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSkinOperatorBoneInfluence : IHavokObject
    {
        public byte m_boneIndex;
        public byte m_weight;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_boneIndex = br.ReadByte();
            m_weight = br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_boneIndex);
            bw.WriteByte(m_weight);
        }
    }
}
