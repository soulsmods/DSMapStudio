using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiNavVolumeEdge : IHavokObject
    {
        public byte m_flags;
        public uint m_oppositeCell;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_flags = br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_oppositeCell = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteUInt32(m_oppositeCell);
        }
    }
}
