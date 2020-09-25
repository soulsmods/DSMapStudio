using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxVertexBuffer : hkReferencedObject
    {
        public hkxVertexBufferVertexData m_data;
        public hkxVertexDescription m_desc;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = new hkxVertexBufferVertexData();
            m_data.Read(des, br);
            m_desc = new hkxVertexDescription();
            m_desc.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_data.Write(bw);
            m_desc.Write(bw);
        }
    }
}
