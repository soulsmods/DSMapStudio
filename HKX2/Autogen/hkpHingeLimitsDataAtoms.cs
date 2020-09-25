using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpHingeLimitsDataAtoms : IHavokObject
    {
        public enum Axis
        {
            AXIS_AXLE = 0,
            AXIS_PERP_TO_AXLE_1 = 1,
            AXIS_PERP_TO_AXLE_2 = 2,
        }
        
        public hkpSetLocalRotationsConstraintAtom m_rotations;
        public hkpAngLimitConstraintAtom m_angLimit;
        public hkp2dAngConstraintAtom m_2dAng;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotations = new hkpSetLocalRotationsConstraintAtom();
            m_rotations.Read(des, br);
            m_angLimit = new hkpAngLimitConstraintAtom();
            m_angLimit.Read(des, br);
            m_2dAng = new hkp2dAngConstraintAtom();
            m_2dAng.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_rotations.Write(bw);
            m_angLimit.Write(bw);
            m_2dAng.Write(bw);
        }
    }
}
