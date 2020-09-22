using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbCharacterStringData : hkReferencedObject
    {
        public List<hkbCharacterStringDataFileNameMeshNamePair> m_skinNames;
        public List<hkbCharacterStringDataFileNameMeshNamePair> m_boneAttachmentNames;
        public List<hkbAssetBundleStringData> m_animationBundleNameData;
        public List<hkbAssetBundleStringData> m_animationBundleFilenameData;
        public List<string> m_characterPropertyNames;
        public List<string> m_retargetingSkeletonMapperFilenames;
        public List<string> m_lodNames;
        public List<string> m_mirroredSyncPointSubstringsA;
        public List<string> m_mirroredSyncPointSubstringsB;
        public string m_name;
        public string m_rigName;
        public string m_ragdollName;
        public string m_behaviorFilename;
        public string m_luaScriptOnCharacterActivated;
        public string m_luaScriptOnCharacterDeactivated;
        public List<string> m_luaFiles;
    }
}
