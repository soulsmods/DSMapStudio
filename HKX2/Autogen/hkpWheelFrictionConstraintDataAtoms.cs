using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpWheelFrictionConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpWheelFrictionConstraintAtom m_friction;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms = new hkpSetLocalTransformsConstraintAtom();
            m_transforms.Read(des, br);
            m_friction = new hkpWheelFrictionConstraintAtom();
            m_friction.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_transforms.Write(bw);
            m_friction.Write(bw);
        }
    }
}
