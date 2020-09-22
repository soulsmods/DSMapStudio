using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpWheelFrictionConstraintAtomAxle
    {
        public float m_spinVelocity;
        public float m_sumVelocity;
        public int m_numWheels;
        public int m_wheelsSolved;
        public int m_stepsSolved;
        public float m_invInertia;
        public float m_inertia;
        public float m_impulseScaling;
        public float m_impulseMax;
        public bool m_isFixed;
        public int m_numWheelsOnGround;
    }
}
