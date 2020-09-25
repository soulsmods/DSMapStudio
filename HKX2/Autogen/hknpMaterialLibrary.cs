using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMaterialLibrary : hkReferencedObject
    {
        public hkFreeListArrayhknpMaterialhknpMaterialId8hknpMaterialFreeListArrayOperations m_entries;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_entries = new hkFreeListArrayhknpMaterialhknpMaterialId8hknpMaterialFreeListArrayOperations();
            m_entries.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            m_entries.Write(bw);
        }
    }
}
