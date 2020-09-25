using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum UpVectorTransformMethod
    {
        USE_GLOBAL_UP = 0,
        USE_INSTANCE_TRANSFORM = 1,
        USE_FACE_NORMAL = 2,
    }
    
    public class hkaiReferenceFrameAndExtrusion : IHavokObject
    {
        public Vector4 m_up;
        public float m_cellExtrusion;
        public float m_silhouetteRadiusExpasion;
        public UpVectorTransformMethod m_upTransformMethod;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_up = des.ReadVector4(br);
            m_cellExtrusion = br.ReadSingle();
            m_silhouetteRadiusExpasion = br.ReadSingle();
            m_upTransformMethod = (UpVectorTransformMethod)br.ReadByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_cellExtrusion);
            bw.WriteSingle(m_silhouetteRadiusExpasion);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
