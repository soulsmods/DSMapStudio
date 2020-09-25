using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpTyremarkPoint : IHavokObject
    {
        public Vector4 m_pointLeft;
        public Vector4 m_pointRight;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pointLeft = des.ReadVector4(br);
            m_pointRight = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
