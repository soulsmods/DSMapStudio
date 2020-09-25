using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSimClothDataCollidableTransformMap : IHavokObject
    {
        public int m_transformSetIndex;
        public List<uint> m_transformIndices;
        public List<Matrix4x4> m_offsets;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transformSetIndex = br.ReadInt32();
            br.AssertUInt32(0);
            m_transformIndices = des.ReadUInt32Array(br);
            m_offsets = des.ReadMatrix4Array(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_transformSetIndex);
            bw.WriteUInt32(0);
        }
    }
}
