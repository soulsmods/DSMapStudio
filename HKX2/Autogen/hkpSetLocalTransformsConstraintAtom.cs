using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpSetLocalTransformsConstraintAtom : hkpConstraintAtom
    {
        public Matrix4x4 m_transformA;
        public Matrix4x4 m_transformB;
    }
}
