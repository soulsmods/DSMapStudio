using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiAvoidanceSolverBoundaryObstacle : IHavokObject
    {
        public Vector4 m_start;
        public Vector4 m_end;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_start = des.ReadVector4(br);
            m_end = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
