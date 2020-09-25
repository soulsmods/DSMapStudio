using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkDescriptionAttribute : IHavokObject
    {
        public string m_string;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_string = des.ReadStringPointer(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
