using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRigidBodyRagdollControlData : IHavokObject
    {
        public hkbKeyFrameControlData m_keyFrameControlData;
        public float m_durationToBlend;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_keyFrameControlData = new hkbKeyFrameControlData();
            m_keyFrameControlData.Read(des, br);
            m_durationToBlend = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_keyFrameControlData.Write(bw);
            bw.WriteSingle(m_durationToBlend);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
