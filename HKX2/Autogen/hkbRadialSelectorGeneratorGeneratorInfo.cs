using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRadialSelectorGeneratorGeneratorInfo : IHavokObject
    {
        public hkbGenerator m_generator;
        public float m_angle;
        public float m_radialSpeed;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_angle = br.ReadSingle();
            m_radialSpeed = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteSingle(m_angle);
            bw.WriteSingle(m_radialSpeed);
        }
    }
}
