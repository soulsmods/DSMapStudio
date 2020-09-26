using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbDelayedModifierInternalState : hkReferencedObject
    {
        public float m_secondsElapsed;
        public bool m_isActive;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_secondsElapsed = br.ReadSingle();
            m_isActive = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_secondsElapsed);
            bw.WriteBoolean(m_isActive);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
