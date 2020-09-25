using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbSetBehaviorCommand : hkReferencedObject
    {
        public ulong m_characterId;
        public hkbBehaviorGraph m_behavior;
        public hkbGenerator m_rootGenerator;
        public List<hkbBehaviorGraph> m_referencedBehaviors;
        public int m_startStateIndex;
        public bool m_randomizeSimulation;
        public int m_padding;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_behavior = des.ReadClassPointer<hkbBehaviorGraph>(br);
            m_rootGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_referencedBehaviors = des.ReadClassPointerArray<hkbBehaviorGraph>(br);
            m_startStateIndex = br.ReadInt32();
            m_randomizeSimulation = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_padding = br.ReadInt32();
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            // Implement Write
            // Implement Write
            bw.WriteInt32(m_startStateIndex);
            bw.WriteBoolean(m_randomizeSimulation);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteInt32(m_padding);
            bw.WriteUInt32(0);
        }
    }
}
