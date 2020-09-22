using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbProjectStringData : hkReferencedObject
    {
        public List<string> m_animationFilenames;
        public List<string> m_behaviorFilenames;
        public List<string> m_characterFilenames;
        public List<string> m_eventNames;
        public string m_animationPath;
        public string m_behaviorPath;
        public string m_characterPath;
        public string m_scriptsPath;
        public string m_fullPathToSource;
    }
}
