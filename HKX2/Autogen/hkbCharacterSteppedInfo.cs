using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterSteppedInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public float m_deltaTime;
        public hkQTransform m_worldFromModel;
        public List<hkQTransform> m_poseModelSpace;
        public List<hkQTransform> m_rigidAttachmentTransforms;
    }
}
