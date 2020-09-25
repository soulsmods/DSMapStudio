using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbTransformVectorModifier : hkbModifier
    {
        public Quaternion m_rotation;
        public Vector4 m_translation;
        public Vector4 m_vectorIn;
        public Vector4 m_vectorOut;
        public bool m_rotateOnly;
        public bool m_inverse;
        public bool m_computeOnActivate;
        public bool m_computeOnModify;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_rotation = des.ReadQuaternion(br);
            m_translation = des.ReadVector4(br);
            m_vectorIn = des.ReadVector4(br);
            m_vectorOut = des.ReadVector4(br);
            m_rotateOnly = br.ReadBoolean();
            m_inverse = br.ReadBoolean();
            m_computeOnActivate = br.ReadBoolean();
            m_computeOnModify = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteBoolean(m_rotateOnly);
            bw.WriteBoolean(m_inverse);
            bw.WriteBoolean(m_computeOnActivate);
            bw.WriteBoolean(m_computeOnModify);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
