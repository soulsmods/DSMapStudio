using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGeneratorPartitionInfo : IHavokObject
    {
        public uint m_boneMask;
        public uint m_partitionMask;
        public short m_numBones;
        public short m_numMaxPartitions;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_boneMask = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_partitionMask = br.ReadUInt32();
            m_numBones = br.ReadInt16();
            m_numMaxPartitions = br.ReadInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_boneMask);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt32(m_partitionMask);
            bw.WriteInt16(m_numBones);
            bw.WriteInt16(m_numMaxPartitions);
        }
    }
}
