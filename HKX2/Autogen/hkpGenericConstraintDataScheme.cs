using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpGenericConstraintDataScheme : IHavokObject
    {
        public List<Vector4> m_data;
        public List<int> m_commands;
        public List<hkpConstraintMotor> m_motors;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_data = des.ReadVector4Array(br);
            m_commands = des.ReadInt32Array(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_motors = des.ReadClassPointerArray<hkpConstraintMotor>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
