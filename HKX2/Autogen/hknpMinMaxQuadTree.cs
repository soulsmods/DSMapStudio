using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMinMaxQuadTree : IHavokObject
    {
        public List<hknpMinMaxQuadTreeMinMaxLevel> m_coarseTreeData;
        public Vector4 m_offset;
        public float m_multiplier;
        public float m_invMultiplier;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_coarseTreeData = des.ReadClassArray<hknpMinMaxQuadTreeMinMaxLevel>(br);
            m_offset = des.ReadVector4(br);
            m_multiplier = br.ReadSingle();
            m_invMultiplier = br.ReadSingle();
            br.ReadUInt64();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_multiplier);
            bw.WriteSingle(m_invMultiplier);
            bw.WriteUInt64(0);
        }
    }
}
