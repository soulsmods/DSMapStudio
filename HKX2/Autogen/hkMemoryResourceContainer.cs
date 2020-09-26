using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryResourceContainer : hkResourceContainer
    {
        public string m_name;
        public List<hkMemoryResourceHandle> m_resourceHandles;
        public List<hkMemoryResourceContainer> m_children;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_name = des.ReadStringPointer(br);
            br.ReadUInt64();
            m_resourceHandles = des.ReadClassPointerArray<hkMemoryResourceHandle>(br);
            m_children = des.ReadClassPointerArray<hkMemoryResourceContainer>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
