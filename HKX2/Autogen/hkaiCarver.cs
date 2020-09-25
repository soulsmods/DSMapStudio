using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiCarver : hkReferencedObject
    {
        public enum FlagBits
        {
            CARVER_ERODE_EDGES = 1,
        }
        
        public hkaiVolume m_volume;
        public uint m_flags;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_volume = des.ReadClassPointer<hkaiVolume>(br);
            m_flags = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt32(0);
        }
    }
}
