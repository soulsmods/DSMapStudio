using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTransitionConstraintSet : hclConstraintSet
    {
        public List<hclTransitionConstraintSetPerParticle> m_perParticleData;
        public float m_toAnimPeriod;
        public float m_toAnimPlusDelayPeriod;
        public float m_toSimPeriod;
        public float m_toSimPlusDelayPeriod;
        public uint m_referenceMeshBufferIdx;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_perParticleData = des.ReadClassArray<hclTransitionConstraintSetPerParticle>(br);
            m_toAnimPeriod = br.ReadSingle();
            m_toAnimPlusDelayPeriod = br.ReadSingle();
            m_toSimPeriod = br.ReadSingle();
            m_toSimPlusDelayPeriod = br.ReadSingle();
            m_referenceMeshBufferIdx = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_toAnimPeriod);
            bw.WriteSingle(m_toAnimPlusDelayPeriod);
            bw.WriteSingle(m_toSimPeriod);
            bw.WriteSingle(m_toSimPlusDelayPeriod);
            bw.WriteUInt32(m_referenceMeshBufferIdx);
            bw.WriteUInt32(0);
        }
    }
}
