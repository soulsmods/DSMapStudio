using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSilhouetteReferenceFrame : IHavokObject
    {
        public Vector4 m_up;
        public Vector4 m_referenceAxis;
        public Vector4 m_orthogonalAxis;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_up = des.ReadVector4(br);
            m_referenceAxis = des.ReadVector4(br);
            m_orthogonalAxis = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
