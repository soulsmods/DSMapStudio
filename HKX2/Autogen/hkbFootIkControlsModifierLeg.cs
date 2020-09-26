using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbFootIkControlsModifierLeg : IHavokObject
    {
        public Vector4 m_groundPosition;
        public hkbEventProperty m_ungroundedEvent;
        public float m_verticalError;
        public bool m_hitSomething;
        public bool m_isPlantedMS;
        public bool m_enabled;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_groundPosition = des.ReadVector4(br);
            m_ungroundedEvent = new hkbEventProperty();
            m_ungroundedEvent.Read(des, br);
            m_verticalError = br.ReadSingle();
            m_hitSomething = br.ReadBoolean();
            m_isPlantedMS = br.ReadBoolean();
            m_enabled = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_ungroundedEvent.Write(bw);
            bw.WriteSingle(m_verticalError);
            bw.WriteBoolean(m_hitSomething);
            bw.WriteBoolean(m_isPlantedMS);
            bw.WriteBoolean(m_enabled);
            bw.WriteUInt64(0);
            bw.WriteByte(0);
        }
    }
}
