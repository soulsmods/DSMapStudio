using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBallAndSocketConstraintDataAtoms : IHavokObject
    {
        public hkpSetLocalTranslationsConstraintAtom m_pivots;
        public hkpSetupStabilizationAtom m_setupStabilization;
        public hkpBallSocketConstraintAtom m_ballSocket;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_pivots = new hkpSetLocalTranslationsConstraintAtom();
            m_pivots.Read(des, br);
            m_setupStabilization = new hkpSetupStabilizationAtom();
            m_setupStabilization.Read(des, br);
            m_ballSocket = new hkpBallSocketConstraintAtom();
            m_ballSocket.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_pivots.Write(bw);
            m_setupStabilization.Write(bw);
            m_ballSocket.Write(bw);
        }
    }
}
