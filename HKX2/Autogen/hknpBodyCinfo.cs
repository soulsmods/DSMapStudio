using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpBodyCinfo : IHavokObject
    {
        public hknpShape m_shape;
        public uint m_reservedBodyId;
        public uint m_motionId;
        public byte m_qualityId;
        public ushort m_materialId;
        public uint m_collisionFilterInfo;
        public int m_flags;
        public float m_collisionLookAheadDistance;
        public string m_name;
        public ulong m_userData;
        public Vector4 m_position;
        public Quaternion m_orientation;
        public byte m_spuFlags;
        public hkLocalFrame m_localFrame;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_shape = des.ReadClassPointer<hknpShape>(br);
            m_reservedBodyId = br.ReadUInt32();
            m_motionId = br.ReadUInt32();
            m_qualityId = br.ReadByte();
            br.AssertByte(0);
            m_materialId = br.ReadUInt16();
            m_collisionFilterInfo = br.ReadUInt32();
            m_flags = br.ReadInt32();
            m_collisionLookAheadDistance = br.ReadSingle();
            m_name = des.ReadStringPointer(br);
            m_userData = br.ReadUInt64();
            m_position = des.ReadVector4(br);
            m_orientation = des.ReadQuaternion(br);
            m_spuFlags = br.ReadByte();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_localFrame = des.ReadClassPointer<hkLocalFrame>(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            // Implement Write
            bw.WriteUInt32(m_reservedBodyId);
            bw.WriteUInt32(m_motionId);
            bw.WriteByte(m_qualityId);
            bw.WriteByte(0);
            bw.WriteUInt16(m_materialId);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteInt32(m_flags);
            bw.WriteSingle(m_collisionLookAheadDistance);
            bw.WriteUInt64(m_userData);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            // Implement Write
        }
    }
}
