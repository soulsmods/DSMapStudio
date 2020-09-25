using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRotationalConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalRotationsConstraintAtom m_rotations;
        public hkpAngConstraintAtom m_ang;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_rotations = new hkpSetLocalRotationsConstraintAtom();
            m_rotations.Read(des, br);
            m_ang = new hkpAngConstraintAtom();
            m_ang.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_rotations.Write(bw);
            m_ang.Write(bw);
        }
    }
}
