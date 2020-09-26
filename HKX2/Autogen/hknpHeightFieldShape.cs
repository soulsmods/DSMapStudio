using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpHeightFieldShape : hknpCompositeShape
    {
        public hkAabb m_aabb;
        public Vector4 m_floatToIntScale;
        public Vector4 m_intToFloatScale;
        public int m_intSizeX;
        public int m_intSizeZ;
        public int m_numBitsX;
        public int m_numBitsZ;
        public hknpMinMaxQuadTree m_minMaxTree;
        public int m_minMaxTreeCoarseness;
        public bool m_includeShapeKeyInSdfContacts;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_aabb = new hkAabb();
            m_aabb.Read(des, br);
            m_floatToIntScale = des.ReadVector4(br);
            m_intToFloatScale = des.ReadVector4(br);
            m_intSizeX = br.ReadInt32();
            m_intSizeZ = br.ReadInt32();
            m_numBitsX = br.ReadInt32();
            m_numBitsZ = br.ReadInt32();
            m_minMaxTree = new hknpMinMaxQuadTree();
            m_minMaxTree.Read(des, br);
            m_minMaxTreeCoarseness = br.ReadInt32();
            m_includeShapeKeyInSdfContacts = br.ReadBoolean();
            br.ReadUInt64();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_aabb.Write(bw);
            bw.WriteInt32(m_intSizeX);
            bw.WriteInt32(m_intSizeZ);
            bw.WriteInt32(m_numBitsX);
            bw.WriteInt32(m_numBitsZ);
            m_minMaxTree.Write(bw);
            bw.WriteInt32(m_minMaxTreeCoarseness);
            bw.WriteBoolean(m_includeShapeKeyInSdfContacts);
            bw.WriteUInt64(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
