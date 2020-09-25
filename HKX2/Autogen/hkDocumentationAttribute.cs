using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkDocumentationAttribute : IHavokObject
    {
        public string m_docsSectionTag;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_docsSectionTag = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
