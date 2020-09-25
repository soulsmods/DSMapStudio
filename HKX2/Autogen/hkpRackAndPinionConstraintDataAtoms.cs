using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpRackAndPinionConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpRackAndPinionConstraintAtom m_rackAndPinion;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms = new hkpSetLocalTransformsConstraintAtom();
            m_transforms.Read(des, br);
            m_rackAndPinion = new hkpRackAndPinionConstraintAtom();
            m_rackAndPinion.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_transforms.Write(bw);
            m_rackAndPinion.Write(bw);
        }
    }
}
