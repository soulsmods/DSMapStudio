using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRadialSelectorGeneratorGeneratorPair : IHavokObject
    {
        public hkbRadialSelectorGeneratorGeneratorInfo m_generators_0;
        public hkbRadialSelectorGeneratorGeneratorInfo m_generators_1;
        public float m_minAngle;
        public float m_maxAngle;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_generators_0 = new hkbRadialSelectorGeneratorGeneratorInfo();
            m_generators_0.Read(des, br);
            m_generators_1 = new hkbRadialSelectorGeneratorGeneratorInfo();
            m_generators_1.Read(des, br);
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_generators_0.Write(bw);
            m_generators_1.Write(bw);
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
        }
    }
}
