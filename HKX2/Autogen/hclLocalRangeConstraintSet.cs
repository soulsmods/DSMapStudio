using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ShapeType
    {
        SHAPE_SPHERE = 0,
        SHAPE_CYLINDER = 1,
    }
    
    public enum ForceUpgrade6
    {
        HCL_FORCE_UPGRADE6 = 0,
    }
    
    public class hclLocalRangeConstraintSet : hclConstraintSet
    {
        public List<hclLocalRangeConstraintSetLocalConstraint> m_localConstraints;
        public uint m_referenceMeshBufferIdx;
        public float m_stiffness;
        public ShapeType m_shapeType;
        public bool m_applyNormalComponent;
    }
}
