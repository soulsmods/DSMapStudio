using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpBridgeAtoms : IHavokObject
    {
        public hkpBridgeConstraintAtom m_bridgeAtom;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_bridgeAtom = new hkpBridgeConstraintAtom();
            m_bridgeAtom.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_bridgeAtom.Write(bw);
        }
    }
}
