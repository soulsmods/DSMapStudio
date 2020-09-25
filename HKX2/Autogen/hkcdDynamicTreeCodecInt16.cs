using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicTreeCodecInt16 : IHavokObject
    {
        public hkcdDynamicTreeCodecInt16IntAabb m_aabb;
        public uint m_parent;
        public uint m_children;
        public uint m_pad;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_aabb = new hkcdDynamicTreeCodecInt16IntAabb();
            m_aabb.Read(des, br);
            m_parent = br.ReadUInt32();
            m_children = br.ReadUInt32();
            br.AssertUInt32(0);
            m_pad = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_aabb.Write(bw);
            bw.WriteUInt32(m_parent);
            bw.WriteUInt32(m_children);
            bw.WriteUInt32(0);
            bw.WriteUInt32(m_pad);
        }
    }
}
