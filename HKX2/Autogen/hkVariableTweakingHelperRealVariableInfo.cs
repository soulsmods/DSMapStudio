using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkVariableTweakingHelperRealVariableInfo : IHavokObject
    {
        public string m_name;
        public float m_value;
        public bool m_tweakOn;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_value = br.ReadSingle();
            m_tweakOn = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_value);
            bw.WriteBoolean(m_tweakOn);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
