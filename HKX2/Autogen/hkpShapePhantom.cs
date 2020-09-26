using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpShapePhantom : hkpPhantom
    {
        public hkMotionState m_motionState;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_motionState = new hkMotionState();
            m_motionState.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            m_motionState.Write(bw);
        }
    }
}
