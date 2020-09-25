using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdSimdTreeNode : hkcdFourAabb
    {
        public enum Flags
        {
            HAS_INTERNALS = 1,
            HAS_LEAVES = 2,
            HAS_NULLS = 4,
        }
        
        public uint m_data;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_data);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
