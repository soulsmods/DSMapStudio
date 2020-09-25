using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiConvexSilhouetteSet : hkReferencedObject
    {
        public List<Vector4> m_vertexPool;
        public List<int> m_silhouetteOffsets;
        public hkQTransform m_cachedTransform;
        public Vector4 m_cachedUp;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_vertexPool = des.ReadVector4Array(br);
            m_silhouetteOffsets = des.ReadInt32Array(br);
            m_cachedTransform = new hkQTransform();
            m_cachedTransform.Read(des, br);
            m_cachedUp = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_cachedTransform.Write(bw);
        }
    }
}
