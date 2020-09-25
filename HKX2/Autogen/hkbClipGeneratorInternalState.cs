using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbClipGeneratorInternalState : hkReferencedObject
    {
        public Matrix4x4 m_extractedMotion;
        public List<hkbClipGeneratorEcho> m_echos;
        public float m_localTime;
        public float m_time;
        public float m_previousUserControlledTimeFraction;
        public int m_bufferSize;
        public bool m_atEnd;
        public bool m_ignoreStartTime;
        public bool m_pingPongBackward;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_extractedMotion = des.ReadQSTransform(br);
            m_echos = des.ReadClassArray<hkbClipGeneratorEcho>(br);
            m_localTime = br.ReadSingle();
            m_time = br.ReadSingle();
            m_previousUserControlledTimeFraction = br.ReadSingle();
            m_bufferSize = br.ReadInt32();
            m_atEnd = br.ReadBoolean();
            m_ignoreStartTime = br.ReadBoolean();
            m_pingPongBackward = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_localTime);
            bw.WriteSingle(m_time);
            bw.WriteSingle(m_previousUserControlledTimeFraction);
            bw.WriteInt32(m_bufferSize);
            bw.WriteBoolean(m_atEnd);
            bw.WriteBoolean(m_ignoreStartTime);
            bw.WriteBoolean(m_pingPongBackward);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
