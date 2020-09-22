using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoseStoringGeneratorOutputListenerStoredPose : hkReferencedObject
    {
        public hkbNode m_node;
        public List<hkQTransform> m_pose;
        public hkQTransform m_worldFromModel;
        public bool m_isPoseValid;
    }
}
