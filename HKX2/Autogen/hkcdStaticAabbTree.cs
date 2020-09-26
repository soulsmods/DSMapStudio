using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdStaticAabbTree : hkReferencedObject
    {
        public hkcdStaticTreeDefaultTreeStorage6 m_treePtr;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_treePtr = des.ReadClassPointer<hkcdStaticTreeDefaultTreeStorage6>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            // Implement Write
        }
    }
}
