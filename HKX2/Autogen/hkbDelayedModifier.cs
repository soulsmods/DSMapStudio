using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbDelayedModifier : hkbModifierWrapper
    {
        public float m_delaySeconds;
        public float m_durationSeconds;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_delaySeconds = br.ReadSingle();
            m_durationSeconds = br.ReadSingle();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_delaySeconds);
            bw.WriteSingle(m_durationSeconds);
            bw.WriteUInt64(0);
        }
    }
}
