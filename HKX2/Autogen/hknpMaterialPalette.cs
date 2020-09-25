using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMaterialPalette : hkReferencedObject
    {
        public List<hknpMaterialDescriptor> m_entries;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_entries = des.ReadClassArray<hknpMaterialDescriptor>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
