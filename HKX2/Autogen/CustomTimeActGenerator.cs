using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class CustomTimeActGenerator : hkbGenerator
    {
        public enum OffsetType
        {
            WeaponCategory = 10,
            IdleCategory = 11,
            Variable = 200,
            Float2Step = 201,
            Float3Step = 202,
            OffsetNone = 0,
        }
        
        public hkbGenerator m_generator;
        public OffsetType m_offsetType;
        public int m_taeId;
        public int m_valIndex;
        public float m_valRate;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_generator = des.ReadClassPointer<hkbGenerator>(br);
            m_offsetType = (OffsetType)br.ReadInt32();
            m_taeId = br.ReadInt32();
            m_valIndex = br.ReadInt32();
            m_valRate = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt64();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteInt32(m_taeId);
            bw.WriteInt32(m_valIndex);
            bw.WriteSingle(m_valRate);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
