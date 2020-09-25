using SoulsFormats;
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
        public Matrix4x4 m_worldFromModel;
        public List<Matrix4x4> m_poseModelSpace;
        public List<Matrix4x4> m_rigidAttachmentTransforms;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_deformableSkinIds = des.ReadUInt64Array(br);
            m_rigidSkinIds = des.ReadUInt64Array(br);
            m_externalEventIds = des.ReadInt16Array(br);
            m_auxiliaryInfo = des.ReadClassPointerArray<hkbAuxiliaryNodeInfo>(br);
            m_activeEventIds = des.ReadInt16Array(br);
            m_activeVariableIds = des.ReadInt16Array(br);
            m_characterId = br.ReadUInt64();
            m_instanceName = des.ReadStringPointer(br);
            m_templateName = des.ReadStringPointer(br);
            m_fullPathToProject = des.ReadStringPointer(br);
            m_localScriptsPath = des.ReadStringPointer(br);
            m_remoteScriptsPath = des.ReadStringPointer(br);
            m_behaviorData = des.ReadClassPointer<hkbBehaviorGraphData>(br);
            m_behaviorInternalState = des.ReadClassPointer<hkbBehaviorGraphInternalState>(br);
            br.AssertUInt64(0);
            m_visible = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_elapsedSimulationTime = br.ReadSingle();
            m_skeleton = des.ReadClassPointer<hkaSkeleton>(br);
            br.AssertUInt64(0);
            m_worldFromModel = des.ReadQSTransform(br);
            m_poseModelSpace = des.ReadQSTransformArray(br);
            m_rigidAttachmentTransforms = des.ReadQSTransformArray(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_characterId);
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteBoolean(m_visible);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_elapsedSimulationTime);
            // Implement Write
            bw.WriteUInt64(0);
        }
    }
}
