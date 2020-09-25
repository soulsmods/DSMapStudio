using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbLookAtModifierInternalState : hkReferencedObject
    {
        public Vector4 m_lookAtLastTargetWS;
        public float m_lookAtWeight;
        public bool m_isTargetInsideLimitCone;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_lookAtLastTargetWS = des.ReadVector4(br);
            m_lookAtWeight = br.ReadSingle();
            m_isTargetInsideLimitCone = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_lookAtWeight);
            bw.WriteBoolean(m_isTargetInsideLimitCone);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
