using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetWordVariableCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public string m_variableName;
        public hkbVariableValue m_value;
        public Vector4 m_quadValue;
        public VariableType m_type;
        public bool m_global;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_variableName = des.ReadStringPointer(br);
            m_value = new hkbVariableValue();
            m_value.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_quadValue = des.ReadVector4(br);
            m_type = (VariableType)br.ReadByte();
            m_global = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            m_value.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteBoolean(m_global);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
