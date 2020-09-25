using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum AccessType
    {
        HK_ACCESS_IGNORE = 0,
        HK_ACCESS_RO = 1,
        HK_ACCESS_RW = 2,
    }
    
    public enum ReadMode
    {
        THIS_OBJECT_ONLY = 0,
        RECURSIVE = 1,
    }
    
    public class hkMultiThreadCheck : IHavokObject
    {
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
