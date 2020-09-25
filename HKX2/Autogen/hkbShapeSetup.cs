using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbShapeSetup : IHavokObject
    {
        public enum Type
        {
            CAPSULE = 0,
            FILE = 1,
        }
        
        public float m_capsuleHeight;
        public float m_capsuleRadius;
        public string m_fileName;
        public Type m_type;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_capsuleHeight = br.ReadSingle();
            m_capsuleRadius = br.ReadSingle();
            m_fileName = des.ReadStringPointer(br);
            m_type = (Type)br.ReadSByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_capsuleHeight);
            bw.WriteSingle(m_capsuleRadius);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
