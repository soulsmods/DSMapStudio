using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbManualSelectorTransitionEffect : hkbTransitionEffect
    {
        public List<hkbTransitionEffect> m_transitionEffects;
        public byte m_selectedIndex;
        public hkbCustomIdSelector m_indexSelector;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_transitionEffects = des.ReadClassPointerArray<hkbTransitionEffect>(br);
            m_selectedIndex = br.ReadByte();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
            m_indexSelector = des.ReadClassPointer<hkbCustomIdSelector>(br);
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteByte(m_selectedIndex);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
