using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpVehicleFrictionStatus : IHavokObject
    {
        public hkpVehicleFrictionStatusAxisStatus m_axis;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_axis = new hkpVehicleFrictionStatusAxisStatus();
            m_axis.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_axis.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
