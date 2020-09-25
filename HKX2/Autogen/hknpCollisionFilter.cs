using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpCollisionFilter : hkReferencedObject
    {
        public enum Type
        {
            TYPE_UNKNOWN = 0,
            TYPE_CONSTRAINT = 1,
            TYPE_GROUP = 2,
            TYPE_PAIR = 3,
            TYPE_USER = 4,
        }
        
        public Type m_type;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_type = (Type)br.ReadByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
