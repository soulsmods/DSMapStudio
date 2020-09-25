using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEventInfo : IHavokObject
    {
        public enum Flags
        {
            FLAG_SILENT = 1,
            FLAG_SYNC_POINT = 2,
        }
        
        public uint m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_flags = br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
