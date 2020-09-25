using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpMoppCodeCodeInfo : IHavokObject
    {
        public Vector4 m_offset;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_offset = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
