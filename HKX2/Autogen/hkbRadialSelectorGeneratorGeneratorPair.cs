using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRadialSelectorGeneratorGeneratorPair : IHavokObject
    {
        public hkbRadialSelectorGeneratorGeneratorInfo m_generators;
        public float m_minAngle;
        public float m_maxAngle;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_generators = new hkbRadialSelectorGeneratorGeneratorInfo();
            m_generators.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_minAngle = br.ReadSingle();
            m_maxAngle = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_generators.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_minAngle);
            bw.WriteSingle(m_maxAngle);
        }
    }
}
