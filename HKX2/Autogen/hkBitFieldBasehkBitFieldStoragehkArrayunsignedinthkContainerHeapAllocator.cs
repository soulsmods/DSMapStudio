using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkBitFieldBasehkBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator : IHavokObject
    {
        public hkBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator m_storage;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_storage = new hkBitFieldStoragehkArrayunsignedinthkContainerHeapAllocator();
            m_storage.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_storage.Write(bw);
        }
    }
}
