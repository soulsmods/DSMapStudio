using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbConstraintSetup : IHavokObject
    {
        public enum Type
        {
            BALL_AND_SOCKET = 0,
            RAGDOLL = 1,
        }
        
        public Type m_type;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_type = (Type)br.ReadSByte();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
