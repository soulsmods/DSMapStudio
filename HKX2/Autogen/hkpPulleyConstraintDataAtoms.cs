using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPulleyConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTranslationsConstraintAtom m_translations;
        public hkpPulleyConstraintAtom m_pulley;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_translations = new hkpSetLocalTranslationsConstraintAtom();
            m_translations.Read(des, br);
            m_pulley = new hkpPulleyConstraintAtom();
            m_pulley.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_translations.Write(bw);
            m_pulley.Write(bw);
        }
    }
}
