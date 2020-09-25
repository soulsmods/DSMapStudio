using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclTaperedCapsuleShape : hclShape
    {
        public Vector4 m_small;
        public Vector4 m_big;
        public Vector4 m_coneApex;
        public Vector4 m_coneAxis;
        public Vector4 m_lVec;
        public Vector4 m_dVec;
        public Vector4 m_tanThetaVecNeg;
        public float m_smallRadius;
        public float m_bigRadius;
        public float m_l;
        public float m_d;
        public float m_cosTheta;
        public float m_sinTheta;
        public float m_tanTheta;
        public float m_tanThetaSqr;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_small = des.ReadVector4(br);
            m_big = des.ReadVector4(br);
            m_coneApex = des.ReadVector4(br);
            m_coneAxis = des.ReadVector4(br);
            m_lVec = des.ReadVector4(br);
            m_dVec = des.ReadVector4(br);
            m_tanThetaVecNeg = des.ReadVector4(br);
            m_smallRadius = br.ReadSingle();
            m_bigRadius = br.ReadSingle();
            m_l = br.ReadSingle();
            m_d = br.ReadSingle();
            m_cosTheta = br.ReadSingle();
            m_sinTheta = br.ReadSingle();
            m_tanTheta = br.ReadSingle();
            m_tanThetaSqr = br.ReadSingle();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_smallRadius);
            bw.WriteSingle(m_bigRadius);
            bw.WriteSingle(m_l);
            bw.WriteSingle(m_d);
            bw.WriteSingle(m_cosTheta);
            bw.WriteSingle(m_sinTheta);
            bw.WriteSingle(m_tanTheta);
            bw.WriteSingle(m_tanThetaSqr);
        }
    }
}
