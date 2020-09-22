using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbPoseStoringGeneratorOutputListener : hkbGeneratorOutputListener
    {
        public List<hkbPoseStoringGeneratorOutputListenerStoredPose> m_storedPoses;
        public bool m_dirty;
    }
}
