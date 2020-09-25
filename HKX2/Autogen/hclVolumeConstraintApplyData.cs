using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclVolumeConstraintApplyData : IHavokObject
    {
        public Vector4 m_frameVector;
        public ushort m_particleIndex;
        public float m_stiffness;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_frameVector = des.ReadVector4(br);
            m_particleIndex = br.ReadUInt16();
            br.AssertUInt16(0);
            m_stiffness = br.ReadSingle();
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_particleIndex);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_stiffness);
            bw.WriteUInt64(0);
        }
    }
}
