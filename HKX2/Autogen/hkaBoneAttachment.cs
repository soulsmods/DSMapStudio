using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaBoneAttachment : hkReferencedObject
    {
        public string m_originalSkeletonName;
        public Matrix4x4 m_boneFromAttachment;
        public hkReferencedObject m_attachment;
        public string m_name;
        public short m_boneIndex;
    }
}
