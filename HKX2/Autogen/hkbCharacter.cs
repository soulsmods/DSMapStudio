using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum CapabilityTypes
    {
        FULL_POSE = 1,
        MIRRORING = 2,
        DOCKING = 4,
        HAND_IK = 8,
        CHARACTER_CONTROL = 16,
        RAGDOLL = 32,
        FOOT_IK = 64,
        AI_CONTROL = 128,
        BASIC_CAPABILITIES = 7,
        PHYSICS_CAPABILITIES = 120,
        STANDARD_CAPABILITIES = 127,
    }
    
    public class hkbCharacter : hkReferencedObject
    {
        public List<hkbCharacter> m_nearbyCharacters;
        public ulong m_userData;
        public short m_currentLod;
        public string m_name;
        public hkbCharacterSetup m_setup;
        public hkbBehaviorGraph m_behaviorGraph;
        public hkbProjectData m_projectData;
        public int m_capabilities;
        public int m_effectiveCapabilities;
        public float m_deltaTime;
        public bool m_useCharactorDeltaTime;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_nearbyCharacters = des.ReadClassPointerArray<hkbCharacter>(br);
            m_userData = br.ReadUInt64();
            m_currentLod = br.ReadInt16();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_name = des.ReadStringPointer(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_setup = des.ReadClassPointer<hkbCharacterSetup>(br);
            m_behaviorGraph = des.ReadClassPointer<hkbBehaviorGraph>(br);
            m_projectData = des.ReadClassPointer<hkbProjectData>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_capabilities = br.ReadInt32();
            m_effectiveCapabilities = br.ReadInt32();
            m_deltaTime = br.ReadSingle();
            m_useCharactorDeltaTime = br.ReadBoolean();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(m_userData);
            bw.WriteInt16(m_currentLod);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            // Implement Write
            // Implement Write
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteInt32(m_capabilities);
            bw.WriteInt32(m_effectiveCapabilities);
            bw.WriteSingle(m_deltaTime);
            bw.WriteBoolean(m_useCharactorDeltaTime);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
