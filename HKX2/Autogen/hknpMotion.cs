using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpMotion : IHavokObject
    {
        public Vector4 m_centerOfMassAndMassFactor;
        public Quaternion m_orientation;
        public short m_inverseInertia;
        public uint m_firstAttachedBodyId;
        public short m_linearVelocityCage;
        public short m_integrationFactor;
        public ushort m_motionPropertiesId;
        public short m_maxLinearAccelerationDistancePerStep;
        public short m_maxRotationToPreventTunneling;
        public byte m_cellIndex;
        public byte m_spaceSplitterWeight;
        public Vector4 m_linearVelocity;
        public Vector4 m_angularVelocity;
        public Vector4 m_previousStepLinearVelocity;
        public Vector4 m_previousStepAngularVelocity;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_centerOfMassAndMassFactor = des.ReadVector4(br);
            m_orientation = des.ReadQuaternion(br);
            m_inverseInertia = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_firstAttachedBodyId = br.ReadUInt32();
            br.AssertUInt32(0);
            m_linearVelocityCage = br.ReadInt16();
            br.AssertUInt32(0);
            m_integrationFactor = br.ReadInt16();
            m_motionPropertiesId = br.ReadUInt16();
            m_maxLinearAccelerationDistancePerStep = br.ReadInt16();
            m_maxRotationToPreventTunneling = br.ReadInt16();
            m_cellIndex = br.ReadByte();
            m_spaceSplitterWeight = br.ReadByte();
            m_linearVelocity = des.ReadVector4(br);
            m_angularVelocity = des.ReadVector4(br);
            m_previousStepLinearVelocity = des.ReadVector4(br);
            m_previousStepAngularVelocity = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteInt16(m_inverseInertia);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteUInt32(m_firstAttachedBodyId);
            bw.WriteUInt32(0);
            bw.WriteInt16(m_linearVelocityCage);
            bw.WriteUInt32(0);
            bw.WriteInt16(m_integrationFactor);
            bw.WriteUInt16(m_motionPropertiesId);
            bw.WriteInt16(m_maxLinearAccelerationDistancePerStep);
            bw.WriteInt16(m_maxRotationToPreventTunneling);
            bw.WriteByte(m_cellIndex);
            bw.WriteByte(m_spaceSplitterWeight);
        }
    }
}
