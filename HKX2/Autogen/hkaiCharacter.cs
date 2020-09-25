using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum State
    {
        STATE_NEEDS_NEW_PATH = 0,
        STATE_FOLLOWING_PATH = 1,
        STATE_SLOWING_TO_GOAL = 2,
        STATE_GOAL_REACHED = 3,
        STATE_PATH_FAILED = 4,
        STATE_WANDERED_OFF_PATH = 5,
        STATE_REPATHING_INCOMPLETE_PATH = 6,
        STATE_MANUAL_CONTROL = 7,
    }
    
    public enum AvoidanceEnabledMaskBits
    {
        AVOID_BOUNDARIES = 1,
        AVOID_CHARACTERS = 2,
        AVOID_OBSTACLES = 4,
        AVOID_NONE = 0,
        AVOID_ALL = 7,
    }
    
    public enum AvoidanceState
    {
        AVOIDANCE_SUCCESS = 0,
        AVOIDANCE_FAILURE = 1,
    }
    
    public class hkaiCharacter : hkReferencedObject
    {
        public ulong m_userData;
        public Vector4 m_position;
        public Vector4 m_forward;
        public Vector4 m_velocity;
        public Vector4 m_up;
        public uint m_currentNavMeshFace;
        public float m_radius;
        public float m_desiredSpeed;
        public hkaiAdaptiveRanger m_adaptiveRanger;
        public hkaiAstarCostModifier m_costModifier;
        public hkaiAstarEdgeFilter m_edgeFilter;
        public uint m_agentFilterInfo;
        public hkaiAvoidanceProperties m_avoidanceProperties;
        public float m_avoidanceState;
        public uint m_agentPriority;
        public ushort m_avoidanceType;
        public byte m_avoidanceEnabledMask;
        public State m_state;
        public uint m_layer;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_userData = br.ReadUInt64();
            br.AssertUInt64(0);
            m_position = des.ReadVector4(br);
            m_forward = des.ReadVector4(br);
            m_velocity = des.ReadVector4(br);
            m_up = des.ReadVector4(br);
            m_currentNavMeshFace = br.ReadUInt32();
            m_radius = br.ReadSingle();
            m_desiredSpeed = br.ReadSingle();
            m_adaptiveRanger = new hkaiAdaptiveRanger();
            m_adaptiveRanger.Read(des, br);
            m_costModifier = des.ReadClassPointer<hkaiAstarCostModifier>(br);
            m_edgeFilter = des.ReadClassPointer<hkaiAstarEdgeFilter>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_agentFilterInfo = br.ReadUInt32();
            br.AssertUInt32(0);
            m_avoidanceProperties = des.ReadClassPointer<hkaiAvoidanceProperties>(br);
            m_avoidanceState = br.ReadSingle();
            m_agentPriority = br.ReadUInt32();
            m_avoidanceType = br.ReadUInt16();
            m_avoidanceEnabledMask = br.ReadByte();
            br.AssertByte(0);
            m_state = (State)br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_layer = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_userData);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_currentNavMeshFace);
            bw.WriteSingle(m_radius);
            bw.WriteSingle(m_desiredSpeed);
            m_adaptiveRanger.Write(bw);
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_agentFilterInfo);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteSingle(m_avoidanceState);
            bw.WriteUInt32(m_agentPriority);
            bw.WriteUInt16(m_avoidanceType);
            bw.WriteByte(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_layer);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
