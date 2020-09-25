using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpAngularDashpotAction : hkpBinaryAction
    {
        public Quaternion m_rotation;
        public float m_strength;
        public float m_damping;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rotation = des.ReadQuaternion(br);
            m_strength = br.ReadSingle();
            m_damping = br.ReadSingle();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_strength);
            bw.WriteSingle(m_damping);
            bw.WriteUInt64(0);
        }
    }
}
