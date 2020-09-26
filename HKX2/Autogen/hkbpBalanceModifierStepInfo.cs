using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbpBalanceModifierStepInfo : IHavokObject
    {
        public short m_boneIndex;
        public float m_fractionOfSolution;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_boneIndex = br.ReadInt16();
            br.ReadUInt16();
            m_fractionOfSolution = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_boneIndex);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_fractionOfSolution);
        }
    }
}
