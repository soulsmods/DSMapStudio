using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclAntiPinchConstraintSet : hclConstraintSet
    {
        public List<hclAntiPinchConstraintSetPerParticle> m_perParticleData;
        public float m_toAnimPeriod;
        public float m_toSimPeriod;
        public float m_toSimMaxDistance;
        public uint m_referenceMeshBufferIdx;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_perParticleData = des.ReadClassArray<hclAntiPinchConstraintSetPerParticle>(br);
            m_toAnimPeriod = br.ReadSingle();
            m_toSimPeriod = br.ReadSingle();
            m_toSimMaxDistance = br.ReadSingle();
            m_referenceMeshBufferIdx = br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_toAnimPeriod);
            bw.WriteSingle(m_toSimPeriod);
            bw.WriteSingle(m_toSimMaxDistance);
            bw.WriteUInt32(m_referenceMeshBufferIdx);
        }
    }
}
