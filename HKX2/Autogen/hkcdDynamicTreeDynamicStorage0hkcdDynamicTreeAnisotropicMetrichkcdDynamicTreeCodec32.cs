using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicTreeDynamicStorage0hkcdDynamicTreeAnisotropicMetrichkcdDynamicTreeCodec32 : hkcdDynamicTreeAnisotropicMetric
    {
        public List<hkcdDynamicTreeCodec32> m_nodes;
        public ushort m_firstFree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nodes = des.ReadClassArray<hkcdDynamicTreeCodec32>(br);
            m_firstFree = br.ReadUInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(m_firstFree);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
