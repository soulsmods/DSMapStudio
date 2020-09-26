using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxSplineControlPoint : IHavokObject
    {
        public Vector4 m_position;
        public Vector4 m_tangentIn;
        public Vector4 m_tangentOut;
        public ControlType m_inType;
        public ControlType m_outType;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_position = des.ReadVector4(br);
            m_tangentIn = des.ReadVector4(br);
            m_tangentOut = des.ReadVector4(br);
            m_inType = (ControlType)br.ReadByte();
            m_outType = (ControlType)br.ReadByte();
            br.ReadUInt64();
            br.ReadUInt32();
            br.ReadUInt16();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
