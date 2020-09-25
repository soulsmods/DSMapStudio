using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPhysicsData : hkReferencedObject
    {
        public hkpWorldCinfo m_worldCinfo;
        public List<hkpPhysicsSystem> m_systems;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_worldCinfo = des.ReadClassPointer<hkpWorldCinfo>(br);
            m_systems = des.ReadClassPointerArray<hkpPhysicsSystem>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
        }
    }
}
