using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpDeformableFixedConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpDeformableLinConstraintAtom m_lin;
        public hkpDeformableAngConstraintAtom m_ang;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms = new hkpSetLocalTransformsConstraintAtom();
            m_transforms.Read(des, br);
            m_lin = new hkpDeformableLinConstraintAtom();
            m_lin.Read(des, br);
            m_ang = new hkpDeformableAngConstraintAtom();
            m_ang.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_transforms.Write(bw);
            m_lin.Write(bw);
            m_ang.Write(bw);
        }
    }
}
