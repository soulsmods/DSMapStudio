using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpWheelFrictionConstraintAtom : hkpConstraintAtom
    {
        public byte m_isEnabled;
        public byte m_forwardAxis;
        public byte m_sideAxis;
        public float m_maxFrictionForce;
        public float m_torque;
        public float m_radius;
        public float m_frictionImpulse;
        public float m_slipImpulse;
        public hkpWheelFrictionConstraintAtomAxle m_axle;
    }
}
