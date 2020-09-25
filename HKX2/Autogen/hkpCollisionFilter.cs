using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum hkpFilterType
    {
        HK_FILTER_UNKNOWN = 0,
        HK_FILTER_NULL = 1,
        HK_FILTER_GROUP = 2,
        HK_FILTER_LIST = 3,
        HK_FILTER_CUSTOM = 4,
        HK_FILTER_PAIR = 5,
        HK_FILTER_CONSTRAINT = 6,
    }
    
    public class hkpCollisionFilter : hkReferencedObject
    {
        public uint m_prepad;
        public hkpFilterType m_type;
        public uint m_postpad;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_prepad = br.ReadUInt32();
            br.AssertUInt32(0);
            m_type = (hkpFilterType)br.ReadUInt32();
            m_postpad = br.ReadUInt32();
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_prepad);
            bw.WriteUInt32(0);
            bw.WriteUInt32(m_postpad);
            bw.WriteUInt64(0);
        }
    }
}
