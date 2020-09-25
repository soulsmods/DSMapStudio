using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDashpotAction : hkpBinaryAction
    {
        public Vector4 m_point;
        public float m_strength;
        public float m_damping;
        public Vector4 m_impulse;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_point = des.ReadVector4(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_strength = br.ReadSingle();
            m_damping = br.ReadSingle();
            br.AssertUInt64(0);
            m_impulse = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_strength);
            bw.WriteSingle(m_damping);
            bw.WriteUInt64(0);
        }
    }
}
