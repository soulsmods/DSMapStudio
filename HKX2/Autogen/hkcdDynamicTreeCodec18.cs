using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicTreeCodec18 : IHavokObject
    {
        public hkAabbHalf m_aabb;
        public ushort m_parent;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_aabb = new hkAabbHalf();
            m_aabb.Read(des, br);
            m_parent = br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_aabb.Write(bw);
            bw.WriteUInt16(m_parent);
        }
    }
}
