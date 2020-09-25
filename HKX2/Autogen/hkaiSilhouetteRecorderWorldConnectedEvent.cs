using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSilhouetteRecorderWorldConnectedEvent : hkaiSilhouetteRecorderReplayEvent
    {
        public hkaiWorld m_world;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_world = des.ReadClassPointer<hkaiWorld>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
