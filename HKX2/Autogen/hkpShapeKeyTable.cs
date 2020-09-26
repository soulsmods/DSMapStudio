using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpShapeKeyTable : IHavokObject
    {
        public hkpShapeKeyTableBlock m_lists;
        public uint m_occupancyBitField;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_lists = des.ReadClassPointer<hkpShapeKeyTableBlock>(br);
            m_occupancyBitField = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt32(m_occupancyBitField);
            bw.WriteUInt32(0);
        }
    }
}
