using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MtChecks
    {
        MULTI_THREADING_CHECKS_ENABLE = 0,
        MULTI_THREADING_CHECKS_IGNORE = 1,
    }
    
    public class hkpWorldObject : hkReferencedObject
    {
        public enum BroadPhaseType
        {
            BROAD_PHASE_INVALID = 0,
            BROAD_PHASE_ENTITY = 1,
            BROAD_PHASE_PHANTOM = 2,
            BROAD_PHASE_BORDER = 3,
            BROAD_PHASE_MAX_ID = 4,
        }
        
        public ulong m_userData;
        public hkpLinkedCollidable m_collidable;
        public hkMultiThreadCheck m_multiThreadCheck;
        public string m_name;
        public List<hkSimpleProperty> m_properties;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.AssertUInt64(0);
            m_userData = br.ReadUInt64();
            m_collidable = new hkpLinkedCollidable();
            m_collidable.Read(des, br);
            m_multiThreadCheck = new hkMultiThreadCheck();
            m_multiThreadCheck.Read(des, br);
            br.AssertUInt32(0);
            m_name = des.ReadStringPointer(br);
            m_properties = des.ReadClassArray<hkSimpleProperty>(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(m_userData);
            m_collidable.Write(bw);
            m_multiThreadCheck.Write(bw);
            bw.WriteUInt32(0);
        }
    }
}
