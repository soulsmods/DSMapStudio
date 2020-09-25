using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum MeshShapeIndexStridingType
    {
        INDICES_INVALID = 0,
        INDICES_INT16 = 1,
        INDICES_INT32 = 2,
        INDICES_MAX_ID = 3,
    }
    
    public enum MeshShapeMaterialIndexStridingType
    {
        MATERIAL_INDICES_INVALID = 0,
        MATERIAL_INDICES_INT8 = 1,
        MATERIAL_INDICES_INT16 = 2,
        MATERIAL_INDICES_MAX_ID = 3,
    }
    
    public class hkpMeshShape : hkpShapeCollection
    {
        public Vector4 m_scaling;
        public int m_numBitsForSubpartIndex;
        public List<hkpMeshShapeSubpart> m_subparts;
        public List<ushort> m_weldingInfo;
        public WeldingType m_weldingType;
        public float m_radius;
        public int m_pad;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_scaling = des.ReadVector4(br);
            m_numBitsForSubpartIndex = br.ReadInt32();
            br.AssertUInt32(0);
            m_subparts = des.ReadClassArray<hkpMeshShapeSubpart>(br);
            m_weldingInfo = des.ReadUInt16Array(br);
            m_weldingType = (WeldingType)br.ReadByte();
            br.AssertUInt16(0);
            br.AssertByte(0);
            m_radius = br.ReadSingle();
            m_pad = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_numBitsForSubpartIndex);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_radius);
            bw.WriteInt32(m_pad);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
