using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiWorldCharacterStepSerializableContext : hkReferencedObject
    {
        public CharacterCallbackType m_callbackType;
        public float m_timestep;
        public List<hkaiCharacter> m_characters;
        public List<hkaiLocalSteeringInput> m_localSteeringInputs;
        public List<hkaiObstacleGenerator> m_obstacleGenerators;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_callbackType = (CharacterCallbackType)br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_timestep = br.ReadSingle();
            m_characters = des.ReadClassPointerArray<hkaiCharacter>(br);
            m_localSteeringInputs = des.ReadClassArray<hkaiLocalSteeringInput>(br);
            m_obstacleGenerators = des.ReadClassPointerArray<hkaiObstacleGenerator>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_timestep);
        }
    }
}
