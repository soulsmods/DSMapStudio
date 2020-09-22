using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterAddedInfo : hkReferencedObject
    {
        public ulong m_characterId;
        public string m_instanceName;
        public string m_templateName;
        public string m_fullPathToProject;
        public string m_localScriptsPath;
        public string m_remoteScriptsPath;
        public hkaSkeleton m_skeleton;
        public hkQTransform m_worldFromModel;
        public List<hkQTransform> m_poseModelSpace;
    }
}
