using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaFootstepAnalysisInfo : hkReferencedObject
    {
        public List<char> m_name;
        public List<char> m_nameStrike;
        public List<char> m_nameLift;
        public List<char> m_nameLock;
        public List<char> m_nameUnlock;
        public List<float> m_minPos;
        public List<float> m_maxPos;
        public List<float> m_minVel;
        public List<float> m_maxVel;
        public List<float> m_allBonesDown;
        public List<float> m_anyBonesDown;
        public float m_posTol;
        public float m_velTol;
        public float m_duration;
    }
}
