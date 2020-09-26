using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpStiffSpringConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTranslationsConstraintAtom m_pivots;
        public hkpSetupStabilizationAtom m_setupStabilization;
        public hkpStiffSpringConstraintAtom m_spring;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivots = new hkpSetLocalTranslationsConstraintAtom();
            m_pivots.Read(des, br);
            m_setupStabilization = new hkpSetupStabilizationAtom();
            m_setupStabilization.Read(des, br);
            m_spring = new hkpStiffSpringConstraintAtom();
            m_spring.Read(des, br);
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_pivots.Write(bw);
            m_setupStabilization.Write(bw);
            m_spring.Write(bw);
            bw.WriteUInt32(0);
        }
    }
}
