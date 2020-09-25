using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRadialSelectorGenerator : hkbGenerator
    {
        public List<hkbRadialSelectorGeneratorGeneratorPair> m_generatorPairs;
        public float m_angle;
        public float m_radius;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generatorPairs = des.ReadClassArray<hkbRadialSelectorGeneratorGeneratorPair>(br);
            m_angle = br.ReadSingle();
            m_radius = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_angle);
            bw.WriteSingle(m_radius);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
