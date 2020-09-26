using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbComputeRotationFromAxisAngleModifier : hkbModifier
    {
        public Quaternion m_rotationOut;
        public Vector4 m_axis;
        public float m_angleDegrees;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUInt64();
            m_rotationOut = des.ReadQuaternion(br);
            m_axis = des.ReadVector4(br);
            m_angleDegrees = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteSingle(m_angleDegrees);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
