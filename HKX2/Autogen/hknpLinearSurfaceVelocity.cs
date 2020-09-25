using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum ProjectMethod
    {
        VELOCITY_PROJECT = 0,
        VELOCITY_RESCALE = 1,
    }
    
    public class hknpLinearSurfaceVelocity : hknpSurfaceVelocity
    {
        public Space m_space;
        public ProjectMethod m_projectMethod;
        public float m_maxVelocityScale;
        public Vector4 m_velocityMeasurePlane;
        public Vector4 m_velocity;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_space = (Space)br.ReadByte();
            m_projectMethod = (ProjectMethod)br.ReadByte();
            br.AssertUInt16(0);
            m_maxVelocityScale = br.ReadSingle();
            br.AssertUInt64(0);
            m_velocityMeasurePlane = des.ReadVector4(br);
            m_velocity = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(0);
            bw.WriteSingle(m_maxVelocityScale);
            bw.WriteUInt64(0);
        }
    }
}
