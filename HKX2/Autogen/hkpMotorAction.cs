using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpMotorAction : hkpUnaryAction
    {
        public Vector4 m_axis;
        public float m_spinRate;
        public float m_gain;
        public bool m_active;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_axis = des.ReadVector4(br);
            m_spinRate = br.ReadSingle();
            m_gain = br.ReadSingle();
            m_active = br.ReadBoolean();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_spinRate);
            bw.WriteSingle(m_gain);
            bw.WriteBoolean(m_active);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
