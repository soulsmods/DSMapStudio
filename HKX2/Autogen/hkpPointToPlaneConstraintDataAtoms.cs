using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpPointToPlaneConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpLinConstraintAtom m_lin;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms = new hkpSetLocalTransformsConstraintAtom();
            m_transforms.Read(des, br);
            m_lin = new hkpLinConstraintAtom();
            m_lin.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_transforms.Write(bw);
            m_lin.Write(bw);
        }
    }
}
