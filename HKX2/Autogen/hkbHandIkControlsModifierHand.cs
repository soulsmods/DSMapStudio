using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbHandIkControlsModifierHand : IHavokObject
    {
        public hkbHandIkControlData m_controlData;
        public int m_handIndex;
        public bool m_enable;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_controlData = new hkbHandIkControlData();
            m_controlData.Read(des, br);
            m_handIndex = br.ReadInt32();
            m_enable = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_controlData.Write(bw);
            bw.WriteInt32(m_handIndex);
            bw.WriteBoolean(m_enable);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
