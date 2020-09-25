using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclSetupMeshSectionTriangle : IHavokObject
    {
        public uint m_indices;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_indices = br.ReadUInt32();
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_indices);
            bw.WriteUInt64(0);
        }
    }
}
