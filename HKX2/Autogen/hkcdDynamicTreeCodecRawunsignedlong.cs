using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicTreeCodecRawunsignedlong : IHavokObject
    {
        public hkAabb m_aabb;
        public ulong m_parent;
        public ulong m_children;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            m_parent = br.ReadUInt64();
            m_children = br.ReadUInt64();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_aabb.Write(bw);
            bw.WriteUInt64(m_parent);
            bw.WriteUInt64(m_children);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
