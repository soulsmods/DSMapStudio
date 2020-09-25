using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpHingeConstraintDataAtoms : IHavokObject
    {
        public enum Axis
        {
            AXIS_AXLE = 0,
        }
        
        public hkpSetLocalTransformsConstraintAtom m_transforms;
        public hkpSetupStabilizationAtom m_setupStabilization;
        public hkp2dAngConstraintAtom m_2dAng;
        public hkpBallSocketConstraintAtom m_ballSocket;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_transforms = new hkpSetLocalTransformsConstraintAtom();
            m_transforms.Read(des, br);
            m_setupStabilization = new hkpSetupStabilizationAtom();
            m_setupStabilization.Read(des, br);
            m_2dAng = new hkp2dAngConstraintAtom();
            m_2dAng.Read(des, br);
            m_ballSocket = new hkpBallSocketConstraintAtom();
            m_ballSocket.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_transforms.Write(bw);
            m_setupStabilization.Write(bw);
            m_2dAng.Write(bw);
            m_ballSocket.Write(bw);
        }
    }
}
