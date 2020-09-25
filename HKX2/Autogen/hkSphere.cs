using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSphere : IHavokObject
    {
        public Vector4 m_pos;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pos = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
