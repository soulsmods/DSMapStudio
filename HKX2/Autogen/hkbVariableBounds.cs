using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbVariableBounds : IHavokObject
    {
        public hkbVariableValue m_min;
        public hkbVariableValue m_max;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = new hkbVariableValue();
            m_min.Read(des, br);
            m_max = new hkbVariableValue();
            m_max.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_min.Write(bw);
            m_max.Write(bw);
        }
    }
}
