using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSpringAction : hkpBinaryAction
    {
        public Vector4 m_lastForce;
        public Vector4 m_positionAinA;
        public Vector4 m_positionBinB;
        public float m_restLength;
        public float m_strength;
        public float m_damping;
        public bool m_onCompression;
        public bool m_onExtension;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_lastForce = des.ReadVector4(br);
            m_positionAinA = des.ReadVector4(br);
            m_positionBinB = des.ReadVector4(br);
            m_restLength = br.ReadSingle();
            m_strength = br.ReadSingle();
            m_damping = br.ReadSingle();
            m_onCompression = br.ReadBoolean();
            m_onExtension = br.ReadBoolean();
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_restLength);
            bw.WriteSingle(m_strength);
            bw.WriteSingle(m_damping);
            bw.WriteBoolean(m_onCompression);
            bw.WriteBoolean(m_onExtension);
            bw.WriteUInt16(0);
        }
    }
}
