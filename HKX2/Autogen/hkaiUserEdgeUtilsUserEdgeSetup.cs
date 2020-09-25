using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiUserEdgeUtilsUserEdgeSetup : IHavokObject
    {
        public hkaiUserEdgeUtilsObb m_obbA;
        public hkaiUserEdgeUtilsObb m_obbB;
        public uint m_userDataA;
        public uint m_userDataB;
        public float m_costAtoB;
        public float m_costBtoA;
        public Vector4 m_worldUpA;
        public Vector4 m_worldUpB;
        public UserEdgeDirection m_direction;
        public UserEdgeSetupSpace m_space;
        public bool m_forceAlign;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_obbA = new hkaiUserEdgeUtilsObb();
            m_obbA.Read(des, br);
            m_obbB = new hkaiUserEdgeUtilsObb();
            m_obbB.Read(des, br);
            m_userDataA = br.ReadUInt32();
            m_userDataB = br.ReadUInt32();
            m_costAtoB = br.ReadSingle();
            m_costBtoA = br.ReadSingle();
            m_worldUpA = des.ReadVector4(br);
            m_worldUpB = des.ReadVector4(br);
            m_direction = (UserEdgeDirection)br.ReadByte();
            m_space = (UserEdgeSetupSpace)br.ReadByte();
            m_forceAlign = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_obbA.Write(bw);
            m_obbB.Write(bw);
            bw.WriteUInt32(m_userDataA);
            bw.WriteUInt32(m_userDataB);
            bw.WriteSingle(m_costAtoB);
            bw.WriteSingle(m_costBtoA);
            bw.WriteBoolean(m_forceAlign);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteByte(0);
        }
    }
}
