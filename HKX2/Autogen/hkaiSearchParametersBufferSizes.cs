using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ThreadingType
    {
        SINGLE_THREADED = 0,
        MULTI_THREADED = 1,
    }
    
    public enum SearchType
    {
        SEARCH_REGULAR = 0,
        SEARCH_HIERARCHICAL = 1,
    }
    
    public class hkaiSearchParametersBufferSizes : IHavokObject
    {
        public int m_maxOpenSetSizeBytes;
        public int m_maxSearchStateSizeBytes;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_maxOpenSetSizeBytes = br.ReadInt32();
            m_maxSearchStateSizeBytes = br.ReadInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt32(m_maxOpenSetSizeBytes);
            bw.WriteInt32(m_maxSearchStateSizeBytes);
        }
    }
}
