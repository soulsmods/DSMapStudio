using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSetunsignedinthkContainerHeapAllocatorhkMapOperationsunsignedint : IHavokObject
    {
        public List<uint> m_elem;
        public int m_numElems;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elem = des.ReadUInt32Array(br);
            m_numElems = br.ReadInt32();
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_numElems);
            bw.WriteUInt32(0);
        }
    }
}
