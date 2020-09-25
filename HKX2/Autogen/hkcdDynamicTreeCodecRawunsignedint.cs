using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicTreeCodecRawunsignedint : IHavokObject
    {
        public hkAabb m_aabb;
        public uint m_parent;
        public uint m_children;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            m_parent = br.ReadUInt32();
            m_children = br.ReadUInt32();
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_aabb.Write(bw);
            bw.WriteUInt32(m_parent);
            bw.WriteUInt32(m_children);
            bw.WriteUInt64(0);
        }
    }
}
