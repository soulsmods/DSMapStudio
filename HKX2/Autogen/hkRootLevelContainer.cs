using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkRootLevelContainer : IHavokObject
    {
        public List<hkRootLevelContainerNamedVariant> m_namedVariants;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_namedVariants = des.ReadClassArray<hkRootLevelContainerNamedVariant>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
