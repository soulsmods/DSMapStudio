using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkSetunsignedlonglonghkContainerHeapAllocatorhkMapOperationsunsignedlonglong : IHavokObject
    {
        public List<ulong> m_elem;
        public int m_numElems;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_elem = des.ReadUInt64Array(br);
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
