using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaDefaultAnimatedReferenceFrame : hkaAnimatedReferenceFrame
    {
        public Vector4 m_up;
        public Vector4 m_forward;
        public float m_duration;
        public List<Vector4> m_referenceFrameSamples;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_up = des.ReadVector4(br);
            m_forward = des.ReadVector4(br);
            m_duration = br.ReadSingle();
            br.AssertUInt32(0);
            m_referenceFrameSamples = des.ReadVector4Array(br);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_duration);
            bw.WriteUInt32(0);
            bw.WriteUInt64(0);
        }
    }
}
