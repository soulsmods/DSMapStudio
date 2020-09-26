using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaAngularReferenceFrame : hkaParameterizedReferenceFrame
    {
        public float m_topAngle;
        public float m_radius;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_topAngle = br.ReadSingle();
            m_radius = br.ReadSingle();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_topAngle);
            bw.WriteSingle(m_radius);
            bw.WriteUInt64(0);
        }
    }
}
