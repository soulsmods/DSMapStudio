using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BuildType
    {
        BUILT_WITH_CHUNK_SUBDIVISION = 0,
        BUILT_WITHOUT_CHUNK_SUBDIVISION = 1,
        BUILD_NOT_SET = 2,
    }
    
    public class hkpMoppCode : hkReferencedObject
    {
        public hkpMoppCodeCodeInfo m_info;
        public List<byte> m_data;
        public BuildType m_buildType;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_info = new hkpMoppCodeCodeInfo();
            m_info.Read(des, br);
            m_data = des.ReadByteArray(br);
            m_buildType = (BuildType)br.ReadSByte();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_info.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
