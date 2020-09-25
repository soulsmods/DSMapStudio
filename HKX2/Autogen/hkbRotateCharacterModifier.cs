using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRotateCharacterModifier : hkbModifier
    {
        public float m_degreesPerSecond;
        public float m_speedMultiplier;
        public Vector4 m_axisOfRotation;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_degreesPerSecond = br.ReadSingle();
            m_speedMultiplier = br.ReadSingle();
            m_axisOfRotation = des.ReadVector4(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_degreesPerSecond);
            bw.WriteSingle(m_speedMultiplier);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
