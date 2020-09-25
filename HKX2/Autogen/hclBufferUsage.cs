using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBufferUsage : IHavokObject
    {
        public enum Component
        {
            COMPONENT_POSITION = 0,
            COMPONENT_NORMAL = 1,
            COMPONENT_TANGENT = 2,
            COMPONENT_BITANGENT = 3,
        }
        
        public enum InternalFlags
        {
            USAGE_NONE = 0,
            USAGE_READ = 1,
            USAGE_WRITE = 2,
            USAGE_FULL_WRITE = 4,
            USAGE_READ_BEFORE_WRITE = 8,
        }
        
        public byte m_perComponentFlags;
        public bool m_trianglesRead;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_perComponentFlags = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_trianglesRead = br.ReadBoolean();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_perComponentFlags);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteBoolean(m_trianglesRead);
        }
    }
}
