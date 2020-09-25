using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiUserEdgeUtilsUserEdgePair : IHavokObject
    {
        public Vector4 m_x;
        public Vector4 m_y;
        public Vector4 m_z;
        public uint m_instanceUidA;
        public uint m_instanceUidB;
        public int m_faceA;
        public int m_faceB;
        public int m_userDataA;
        public int m_userDataB;
        public short m_costAtoB;
        public short m_costBtoA;
        public UserEdgeDirection m_direction;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_x = des.ReadVector4(br);
            m_y = des.ReadVector4(br);
            m_z = des.ReadVector4(br);
            m_instanceUidA = br.ReadUInt32();
            m_instanceUidB = br.ReadUInt32();
            m_faceA = br.ReadInt32();
            m_faceB = br.ReadInt32();
            m_userDataA = br.ReadInt32();
            m_userDataB = br.ReadInt32();
            m_costAtoB = br.ReadInt16();
            m_costBtoA = br.ReadInt16();
            m_direction = (UserEdgeDirection)br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_instanceUidA);
            bw.WriteUInt32(m_instanceUidB);
            bw.WriteInt32(m_faceA);
            bw.WriteInt32(m_faceB);
            bw.WriteInt32(m_userDataA);
            bw.WriteInt32(m_userDataB);
            bw.WriteInt16(m_costAtoB);
            bw.WriteInt16(m_costBtoA);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
