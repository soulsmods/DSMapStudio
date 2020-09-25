using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticTreeDynamicStoragehkcdStaticTreeCodecRaw : IHavokObject
    {
        public List<hkcdStaticTreeCodecRaw> m_nodes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_nodes = des.ReadClassArray<hkcdStaticTreeCodecRaw>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
