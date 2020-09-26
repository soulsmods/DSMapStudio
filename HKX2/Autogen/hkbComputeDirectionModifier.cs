using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeDirectionModifier : hkbModifier
    {
        public Vector4 m_pointIn;
        public Vector4 m_pointOut;
        public float m_groundAngleOut;
        public float m_upAngleOut;
        public float m_verticalOffset;
        public bool m_reverseGroundAngle;
        public bool m_reverseUpAngle;
        public bool m_projectPoint;
        public bool m_normalizePoint;
        public bool m_computeOnlyOnce;
        public bool m_computedOutput;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_pointIn = des.ReadVector4(br);
            m_pointOut = des.ReadVector4(br);
            m_groundAngleOut = br.ReadSingle();
            m_upAngleOut = br.ReadSingle();
            m_verticalOffset = br.ReadSingle();
            m_reverseGroundAngle = br.ReadBoolean();
            m_reverseUpAngle = br.ReadBoolean();
            m_projectPoint = br.ReadBoolean();
            m_normalizePoint = br.ReadBoolean();
            m_computeOnlyOnce = br.ReadBoolean();
            m_computedOutput = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_groundAngleOut);
            bw.WriteSingle(m_upAngleOut);
            bw.WriteSingle(m_verticalOffset);
            bw.WriteBoolean(m_reverseGroundAngle);
            bw.WriteBoolean(m_reverseUpAngle);
            bw.WriteBoolean(m_projectPoint);
            bw.WriteBoolean(m_normalizePoint);
            bw.WriteBoolean(m_computeOnlyOnce);
            bw.WriteBoolean(m_computedOutput);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
