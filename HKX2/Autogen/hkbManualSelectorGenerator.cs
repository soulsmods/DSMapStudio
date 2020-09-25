using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbManualSelectorGenerator : hkbGenerator
    {
        public List<hkbGenerator> m_generators;
        public short m_selectedGeneratorIndex;
        public hkbCustomIdSelector m_indexSelector;
        public bool m_selectedIndexCanChangeAfterActivate;
        public hkbTransitionEffect m_generatorChangedTransitionEffect;
        public int m_endOfClipEventId;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generators = des.ReadClassPointerArray<hkbGenerator>(br);
            m_selectedGeneratorIndex = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_indexSelector = des.ReadClassPointer<hkbCustomIdSelector>(br);
            m_selectedIndexCanChangeAfterActivate = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_generatorChangedTransitionEffect = des.ReadClassPointer<hkbTransitionEffect>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_endOfClipEventId = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt16(m_selectedGeneratorIndex);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            // Implement Write
            bw.WriteBoolean(m_selectedIndexCanChangeAfterActivate);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteInt32(m_endOfClipEventId);
            bw.WriteUInt32(0);
        }
    }
}
