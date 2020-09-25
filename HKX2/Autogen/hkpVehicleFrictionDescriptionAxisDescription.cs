using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpVehicleFrictionDescriptionAxisDescription : IHavokObject
    {
        public float m_frictionCircleYtab;
        public float m_xStep;
        public float m_xStart;
        public float m_wheelSurfaceInertia;
        public float m_wheelSurfaceInertiaInv;
        public float m_wheelChassisMassRatio;
        public float m_wheelRadius;
        public float m_wheelRadiusInv;
        public float m_wheelDownForceFactor;
        public float m_wheelDownForceSumFactor;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_frictionCircleYtab = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_xStep = br.ReadSingle();
            m_xStart = br.ReadSingle();
            m_wheelSurfaceInertia = br.ReadSingle();
            m_wheelSurfaceInertiaInv = br.ReadSingle();
            m_wheelChassisMassRatio = br.ReadSingle();
            m_wheelRadius = br.ReadSingle();
            m_wheelRadiusInv = br.ReadSingle();
            m_wheelDownForceFactor = br.ReadSingle();
            m_wheelDownForceSumFactor = br.ReadSingle();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_frictionCircleYtab);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteSingle(m_xStep);
            bw.WriteSingle(m_xStart);
            bw.WriteSingle(m_wheelSurfaceInertia);
            bw.WriteSingle(m_wheelSurfaceInertiaInv);
            bw.WriteSingle(m_wheelChassisMassRatio);
            bw.WriteSingle(m_wheelRadius);
            bw.WriteSingle(m_wheelRadiusInv);
            bw.WriteSingle(m_wheelDownForceFactor);
            bw.WriteSingle(m_wheelDownForceSumFactor);
        }
    }
}
