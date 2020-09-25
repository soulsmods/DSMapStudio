using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbJigglerModifierInternalState : hkReferencedObject
    {
        public List<Vector4> m_currentVelocitiesWS;
        public List<Vector4> m_currentPositions;
        public float m_timeStep;
        public bool m_initNextModify;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_currentVelocitiesWS = des.ReadVector4Array(br);
            m_currentPositions = des.ReadVector4Array(br);
            m_timeStep = br.ReadSingle();
            m_initNextModify = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_timeStep);
            bw.WriteBoolean(m_initNextModify);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
