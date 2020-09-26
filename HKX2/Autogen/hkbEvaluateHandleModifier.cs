using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbEvaluateHandleModifier : hkbModifier
    {
        public enum HandleChangeMode
        {
            HANDLE_CHANGE_MODE_ABRUPT = 0,
            HANDLE_CHANGE_MODE_CONSTANT_VELOCITY = 1,
        }
        
        public hkbHandle m_handle;
        public Vector4 m_handlePositionOut;
        public Quaternion m_handleRotationOut;
        public bool m_isValidOut;
        public float m_extrapolationTimeStep;
        public float m_handleChangeSpeed;
        public HandleChangeMode m_handleChangeMode;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_handle = des.ReadClassPointer<hkbHandle>(br);
            m_handlePositionOut = des.ReadVector4(br);
            m_handleRotationOut = des.ReadQuaternion(br);
            m_isValidOut = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
            m_extrapolationTimeStep = br.ReadSingle();
            m_handleChangeSpeed = br.ReadSingle();
            m_handleChangeMode = (HandleChangeMode)br.ReadSByte();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt64();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteBoolean(m_isValidOut);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_extrapolationTimeStep);
            bw.WriteSingle(m_handleChangeSpeed);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
