using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTransformSetUsage : IHavokObject
    {
        public enum Component
        {
            COMPONENT_TRANSFORM = 0,
            COMPONENT_INVTRANSPOSE = 1,
            NUM_COMPONENTS = 2,
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
        public List<hclTransformSetUsageTransformTracker> m_perComponentTransformTrackers;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_perComponentFlags = br.ReadByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_perComponentTransformTrackers = des.ReadClassArray<hclTransformSetUsageTransformTracker>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteByte(m_perComponentFlags);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
