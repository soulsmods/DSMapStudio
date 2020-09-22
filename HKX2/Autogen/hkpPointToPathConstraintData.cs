using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum OrientationConstraintType
    {
        CONSTRAIN_ORIENTATION_INVALID = 0,
        CONSTRAIN_ORIENTATION_NONE = 1,
        CONSTRAIN_ORIENTATION_ALLOW_SPIN = 2,
        CONSTRAIN_ORIENTATION_TO_PATH = 3,
        CONSTRAIN_ORIENTATION_MAX_ID = 4,
    }
    
    public class hkpPointToPathConstraintData : hkpConstraintData
    {
        public hkpBridgeAtoms m_atoms;
        public hkpParametricCurve m_path;
        public float m_maxFrictionForce;
        public OrientationConstraintType m_angularConstrainedDOF;
        public Matrix4x4 m_transform_OS_KS;
    }
}
