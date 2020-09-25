using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkVariableTweakingHelperIntVariableInfo : IHavokObject
    {
        public string m_name;
        public int m_value;
        public bool m_tweakOn;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_value = br.ReadInt32();
            m_tweakOn = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_value);
            bw.WriteBoolean(m_tweakOn);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
