using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpWorldSnapshot : hkReferencedObject
    {
        public hknpWorldCinfo m_worldCinfo;
        public List<hknpBody> m_bodies;
        public List<string> m_bodyNames;
        public List<hknpMotion> m_motions;
        public List<hknpConstraintCinfo> m_constraints;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_worldCinfo = new hknpWorldCinfo();
            m_worldCinfo.Read(des, br);
            m_bodies = des.ReadClassArray<hknpBody>(br);
            m_bodyNames = des.ReadStringPointerArray(br);
            m_motions = des.ReadClassArray<hknpMotion>(br);
            m_constraints = des.ReadClassArray<hknpConstraintCinfo>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_worldCinfo.Write(bw);
        }
    }
}
