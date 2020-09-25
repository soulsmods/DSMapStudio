using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkVariableTweakingHelperVector4VariableInfo : IHavokObject
    {
        public string m_name;
        public float m_x;
        public float m_y;
        public float m_z;
        public float m_w;
        public bool m_tweakOn;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_x = br.ReadSingle();
            m_y = br.ReadSingle();
            m_z = br.ReadSingle();
            m_w = br.ReadSingle();
            m_tweakOn = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_x);
            bw.WriteSingle(m_y);
            bw.WriteSingle(m_z);
            bw.WriteSingle(m_w);
            bw.WriteBoolean(m_tweakOn);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
