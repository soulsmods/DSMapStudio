using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpSparseCompactMapunsignedshort : IHavokObject
    {
        public uint m_secondaryKeyMask;
        public uint m_sencondaryKeyBits;
        public List<ushort> m_primaryKeyToIndex;
        public List<ushort> m_valueAndSecondaryKeys;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_secondaryKeyMask = br.ReadUInt32();
            m_sencondaryKeyBits = br.ReadUInt32();
            m_primaryKeyToIndex = des.ReadUInt16Array(br);
            m_valueAndSecondaryKeys = des.ReadUInt16Array(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_secondaryKeyMask);
            bw.WriteUInt32(m_sencondaryKeyBits);
        }
    }
}
