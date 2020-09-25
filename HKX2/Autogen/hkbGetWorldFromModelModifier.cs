using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbGetWorldFromModelModifier : hkbModifier
    {
        public Vector4 m_translationOut;
        public Quaternion m_rotationOut;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_translationOut = des.ReadVector4(br);
            m_rotationOut = des.ReadQuaternion(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
        }
    }
}
