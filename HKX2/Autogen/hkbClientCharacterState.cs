using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkbClientCharacterState : hkReferencedObject
    {
        public List<ulong> m_deformableSkinIds;
        public List<ulong> m_rigidSkinIds;
        public List<short> m_externalEventIds;
        public List<hkbAuxiliaryNodeInfo> m_auxiliaryInfo;
        public List<short> m_activeEventIds;
        public List<short> m_activeVariableIds;
        public ulong m_characterId;
        public string m_instanceName;
        public string m_templateName;
        public string m_fullPathToProject;
        public string m_localScriptsPath;
        public string m_remoteScriptsPath;
        public hkbBehaviorGraphData m_behaviorData;
        public hkbBehaviorGraphInternalState m_behaviorInternalState;
        public bool m_visible;
        public float m_elapsedSimulationTime;
        public hkaSkeleton m_skeleton;
        public hkQTransform m_worldFromModel;
        public List<hkQTransform> m_poseModelSpace;
        public List<hkQTransform> m_rigidAttachmentTransforms;
    }
}
