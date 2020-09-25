using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSilhouetteRecorderVolumeLoadedEvent : hkaiSilhouetteRecorderReplayEvent
    {
        public hkaiNavVolumeInstance m_volume;
        public hkaiNavVolumeMediator m_mediator;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_volume = des.ReadClassPointer<hkaiNavVolumeInstance>(br);
            m_mediator = des.ReadClassPointer<hkaiNavVolumeMediator>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
