using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum VariableType
    {
        VARIABLE_TYPE_INVALID = -1,
        VARIABLE_TYPE_BOOL = 0,
        VARIABLE_TYPE_INT8 = 1,
        VARIABLE_TYPE_INT16 = 2,
        VARIABLE_TYPE_INT32 = 3,
        VARIABLE_TYPE_REAL = 4,
        VARIABLE_TYPE_POINTER = 5,
        VARIABLE_TYPE_VECTOR3 = 6,
        VARIABLE_TYPE_VECTOR4 = 7,
        VARIABLE_TYPE_QUATERNION = 8,
    }
    
    public class hkbVariableInfo : IHavokObject
    {
        public hkbRoleAttribute m_role;
        public VariableType m_type;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_role = new hkbRoleAttribute();
            m_role.Read(des, br);
            m_type = (VariableType)br.ReadSByte();
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_role.Write(bw);
            bw.WriteByte(0);
        }
    }
}
