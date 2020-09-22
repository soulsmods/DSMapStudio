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
    }
}
