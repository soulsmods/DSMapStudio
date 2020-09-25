using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiAabbTreeNavVolumeMediator : hkaiNavVolumeMediator
    {
        public hkaiNavVolume m_navVolume;
        public hkcdStaticAabbTree m_tree;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_navVolume = des.ReadClassPointer<hkaiNavVolume>(br);
            m_tree = des.ReadClassPointer<hkcdStaticAabbTree>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            // Implement Write
        }
    }
}
