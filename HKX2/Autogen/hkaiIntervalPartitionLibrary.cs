using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiIntervalPartitionLibrary : IHavokObject
    {
        public List<float> m_data;
        public List<hkaiIntervalPartitionLibraryPartitionRecord> m_partitionRecords;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_data = des.ReadSingleArray(br);
            m_partitionRecords = des.ReadClassArray<hkaiIntervalPartitionLibraryPartitionRecord>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
