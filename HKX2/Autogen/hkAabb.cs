using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkAabb : IHavokObject
    {
        public Vector4 m_min;
        public Vector4 m_max;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_min = des.ReadVector4(br);
            m_max = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
