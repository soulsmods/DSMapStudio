using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpVehicleDriverInputAnalogStatus : hknpVehicleDriverInputStatus
    {
        public float m_positionX;
        public float m_positionY;
        public bool m_handbrakeButtonPressed;
        public bool m_reverseButtonPressed;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_positionX = br.ReadSingle();
            m_positionY = br.ReadSingle();
            m_handbrakeButtonPressed = br.ReadBoolean();
            m_reverseButtonPressed = br.ReadBoolean();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_positionX);
            bw.WriteSingle(m_positionY);
            bw.WriteBoolean(m_handbrakeButtonPressed);
            bw.WriteBoolean(m_reverseButtonPressed);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
