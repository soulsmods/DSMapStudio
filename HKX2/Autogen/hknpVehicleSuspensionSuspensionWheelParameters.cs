using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleSuspensionSuspensionWheelParameters : IHavokObject
    {
        public Vector4 m_hardpointChassisSpace;
        public Vector4 m_directionChassisSpace;
        public float m_length;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_hardpointChassisSpace = des.ReadVector4(br);
            m_directionChassisSpace = des.ReadVector4(br);
            m_length = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_length);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
