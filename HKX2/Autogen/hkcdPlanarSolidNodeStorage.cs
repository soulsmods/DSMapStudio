using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdPlanarSolidNodeStorage : hkReferencedObject
    {
        public List<hkcdPlanarSolidNode> m_storage;
        public uint m_firstFreeNodeId;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_storage = des.ReadClassArray<hkcdPlanarSolidNode>(br);
            m_firstFreeNodeId = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_firstFreeNodeId);
            bw.WriteUInt32(0);
        }
    }
}
