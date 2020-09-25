using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiDirectedGraphInstanceFreeBlockList : IHavokObject
    {
        public List<int> m_blocks;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_blocks = des.ReadInt32Array(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
