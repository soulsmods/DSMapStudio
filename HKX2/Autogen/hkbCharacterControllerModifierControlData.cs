using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterControllerModifierControlData : IHavokObject
    {
        public float m_verticalGain;
        public float m_horizontalCatchUpGain;
        public float m_maxVerticalSeparation;
        public float m_maxHorizontalSeparation;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_verticalGain = br.ReadSingle();
            m_horizontalCatchUpGain = br.ReadSingle();
            m_maxVerticalSeparation = br.ReadSingle();
            m_maxHorizontalSeparation = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_verticalGain);
            bw.WriteSingle(m_horizontalCatchUpGain);
            bw.WriteSingle(m_maxVerticalSeparation);
            bw.WriteSingle(m_maxHorizontalSeparation);
        }
    }
}
