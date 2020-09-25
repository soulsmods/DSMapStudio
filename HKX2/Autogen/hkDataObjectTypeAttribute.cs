using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkDataObjectTypeAttribute : IHavokObject
    {
        public string m_typeName;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_typeName = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
