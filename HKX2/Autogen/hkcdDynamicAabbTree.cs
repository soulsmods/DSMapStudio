using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdDynamicAabbTree : hkReferencedObject
    {
        public hkcdDynamicTreeDefaultTree48Storage m_treePtr;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_treePtr = des.ReadClassPointer<hkcdDynamicTreeDefaultTree48Storage>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
