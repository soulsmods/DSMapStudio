using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCameraShakeEventPayload : hkbEventPayload
    {
        public float m_amplitude;
        public float m_halfLife;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_amplitude = br.ReadSingle();
            m_halfLife = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_amplitude);
            bw.WriteSingle(m_halfLife);
        }
    }
}
