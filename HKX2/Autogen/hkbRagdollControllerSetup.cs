using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbRagdollControllerSetup : IHavokObject
    {
        public enum Type
        {
            POWERED = 1,
            RIGID_BODY = 2,
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
