using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicTreeDynamicStorage0hkcdDynamicTreeAnisotropicMetrichkcdDynamicTreeCodecInt16 : hkcdDynamicTreeAnisotropicMetric
    {
        public List<hkcdDynamicTreeCodecInt16> m_nodes;
        public uint m_firstFree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nodes = des.ReadClassArray<hkcdDynamicTreeCodecInt16>(br);
            m_firstFree = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_firstFree);
            bw.WriteUInt32(0);
        }
    }
}
