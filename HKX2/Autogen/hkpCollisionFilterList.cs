using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCollisionFilterList : hkpCollisionFilter
    {
        public List<hkpCollisionFilter> m_collisionFilters;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collisionFilters = des.ReadClassPointerArray<hkpCollisionFilter>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
