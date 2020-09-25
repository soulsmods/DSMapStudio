using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbBehaviorInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public hkbBehaviorGraphData m_data;
        public List<hkbBehaviorInfoIdToNamePair> m_idToNamePairs;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_data = des.ReadClassPointer<hkbBehaviorGraphData>(br);
            m_idToNamePairs = des.ReadClassArray<hkbBehaviorInfoIdToNamePair>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            // Implement Write
        }
    }
}
