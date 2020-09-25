using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbpControlledReachModifier : hkbModifier
    {
        public float m_fadeInStart;
        public float m_fadeInEnd;
        public float m_fadeOutStart;
        public float m_fadeOutEnd;
        public float m_fadeOutDuration;
        public float m_sensorAngle;
        public short m_handIndex;
        public bool m_isHandEnabled;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_fadeInStart = br.ReadSingle();
            m_fadeInEnd = br.ReadSingle();
            m_fadeOutStart = br.ReadSingle();
            m_fadeOutEnd = br.ReadSingle();
            m_fadeOutDuration = br.ReadSingle();
            m_sensorAngle = br.ReadSingle();
            m_handIndex = br.ReadInt16();
            br.AssertUInt16(0);
            m_isHandEnabled = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_fadeInStart);
            bw.WriteSingle(m_fadeInEnd);
            bw.WriteSingle(m_fadeOutStart);
            bw.WriteSingle(m_fadeOutEnd);
            bw.WriteSingle(m_fadeOutDuration);
            bw.WriteSingle(m_sensorAngle);
            bw.WriteInt16(m_handIndex);
            bw.WriteUInt16(0);
            bw.WriteBoolean(m_isHandEnabled);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
