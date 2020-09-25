using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMemoryResourceHandle : hkResourceHandle
    {
        public hkReferencedObject m_variant;
        public string m_name;
        public List<hkMemoryResourceHandleExternalLink> m_references;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_variant = des.ReadClassPointer<hkReferencedObject>(br);
            m_name = des.ReadStringPointer(br);
            m_references = des.ReadClassArray<hkMemoryResourceHandleExternalLink>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
