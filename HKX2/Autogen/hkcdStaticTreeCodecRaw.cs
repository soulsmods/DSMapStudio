using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticTreeCodecRaw : IHavokObject
    {
        public hkAabb m_aabb;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_aabb.Write(bw);
        }
    }
}
