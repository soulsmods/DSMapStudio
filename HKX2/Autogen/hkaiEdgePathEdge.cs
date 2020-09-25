using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiEdgePathEdge : IHavokObject
    {
        public Vector4 m_left;
        public Vector4 m_right;
        public Vector4 m_up;
        public Matrix4x4 m_followingTransform;
        public hkaiPersistentEdgeKey m_edge;
        public hkaiEdgePathFollowingCornerInfo m_leftFollowingCorner;
        public hkaiEdgePathFollowingCornerInfo m_rightFollowingCorner;
        public byte m_flags;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_left = des.ReadVector4(br);
            m_right = des.ReadVector4(br);
            m_up = des.ReadVector4(br);
            m_followingTransform = des.ReadMatrix4(br);
            m_edge = new hkaiPersistentEdgeKey();
            m_edge.Read(des, br);
            m_leftFollowingCorner = new hkaiEdgePathFollowingCornerInfo();
            m_leftFollowingCorner.Read(des, br);
            m_rightFollowingCorner = new hkaiEdgePathFollowingCornerInfo();
            m_rightFollowingCorner.Read(des, br);
            m_flags = br.ReadByte();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            br.AssertByte(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_edge.Write(bw);
            m_leftFollowingCorner.Write(bw);
            m_rightFollowingCorner.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
