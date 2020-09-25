using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkRootLevelContainerNamedVariant : IHavokObject
    {
        public string m_name;
        public string m_className;
        public hkReferencedObject m_variant;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_name = des.ReadStringPointer(br);
            m_className = des.ReadStringPointer(br);
            m_variant = des.ReadClassPointer<hkReferencedObject>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
        }
    }
}
