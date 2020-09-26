using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSilhouetteRecorderGraphUnloadedEvent : hkaiSilhouetteRecorderReplayEvent
    {
        public uint m_sectionUid;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_sectionUid = br.ReadUInt32();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(m_sectionUid);
            bw.WriteUInt32(0);
        }
    }
}
