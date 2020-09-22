using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpPhysicsSystemData : hkReferencedObject
    {
        public List<hknpMaterial> m_materials;
        public List<hknpMotionProperties> m_motionProperties;
        public List<hknpMotionCinfo> m_motionCinfos;
        public List<hknpBodyCinfo> m_bodyCinfos;
        public List<hknpConstraintCinfo> m_constraintCinfos;
        public List<hkReferencedObject> m_referencedObjects;
        public string m_name;
    }
}
