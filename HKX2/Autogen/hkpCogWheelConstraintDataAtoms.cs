using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpCogWheelConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpCogWheelConstraintAtom m_cogWheels;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms = new hkpSetLocalTransformsConstraintAtom();
            m_transforms.Read(des, br);
            m_cogWheels = new hkpCogWheelConstraintAtom();
            m_cogWheels.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_transforms.Write(bw);
            m_cogWheels.Write(bw);
        }
    }
}
