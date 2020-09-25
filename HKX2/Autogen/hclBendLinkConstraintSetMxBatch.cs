using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBendLinkConstraintSetMxBatch : IHavokObject
    {
        public float m_bendMinLengths;
        public float m_stretchMaxLengths;
        public float m_stretchStiffnesses;
        public float m_bendStiffnesses;
        public float m_invMassesA;
        public float m_invMassesB;
        public ushort m_particlesA;
        public ushort m_particlesB;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_bendMinLengths = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_stretchMaxLengths = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_stretchStiffnesses = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_bendStiffnesses = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_invMassesA = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_invMassesB = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_particlesA = br.ReadUInt16();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_particlesB = br.ReadUInt16();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_bendMinLengths);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_stretchMaxLengths);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_stretchStiffnesses);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_bendStiffnesses);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_invMassesA);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_invMassesB);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(m_particlesA);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteUInt16(m_particlesB);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
