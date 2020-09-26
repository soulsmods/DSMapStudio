using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpVehicleFrictionStatus : IHavokObject
    {
        public hkpVehicleFrictionStatusAxisStatus m_axis_0;
        public hkpVehicleFrictionStatusAxisStatus m_axis_1;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_axis_0 = new hkpVehicleFrictionStatusAxisStatus();
            m_axis_0.Read(des, br);
            m_axis_1 = new hkpVehicleFrictionStatusAxisStatus();
            m_axis_1.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_axis_0.Write(bw);
            m_axis_1.Write(bw);
        }
    }
}
