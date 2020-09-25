using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbpReachTowardTargetModifierHand : IHavokObject
    {
        public short m_shoulderIndex;
        public bool m_isEnabled;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shoulderIndex = br.ReadInt16();
            m_isEnabled = br.ReadBoolean();
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_shoulderIndex);
            bw.WriteBoolean(m_isEnabled);
            bw.WriteByte(0);
        }
    }
}
