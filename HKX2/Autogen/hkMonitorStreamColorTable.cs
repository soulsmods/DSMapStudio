using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkMonitorStreamColorTable : hkReferencedObject
    {
        public List<hkMonitorStreamColorTableColorPair> m_colorPairs;
        public uint m_defaultColor;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_colorPairs = des.ReadClassArray<hkMonitorStreamColorTableColorPair>(br);
            m_defaultColor = br.ReadUInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_defaultColor);
            bw.WriteUInt32(0);
        }
    }
}
