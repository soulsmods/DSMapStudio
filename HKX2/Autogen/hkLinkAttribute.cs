using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum Link
    {
        NONE = 0,
        DIRECT_LINK = 1,
        CHILD = 2,
        MESH = 3,
        PARENT_NAME = 4,
        IMGSELECT = 5,
    }
    
    public class hkLinkAttribute : IHavokObject
    {
        public Link m_type;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_type = (Link)br.ReadSByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
