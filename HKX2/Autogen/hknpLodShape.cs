using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpLodShape : hknpCompositeShape
    {
        public int m_numLevelsOfDetail;
        public hknpLodShapeLevelOfDetailInfo m_infos;
        public hknpShape m_shapes;
        public uint m_shapesMemorySizes;
        public int m_indexCurrentShapeOnSpu;
        public hknpShape m_currentShapePpuAddress;
        public hkAabb m_maximumAabb;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numLevelsOfDetail = br.ReadInt32();
            m_infos = new hknpLodShapeLevelOfDetailInfo();
            m_infos.Read(des, br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_shapes = des.ReadClassPointer<hknpShape>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_shapesMemorySizes = br.ReadUInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_indexCurrentShapeOnSpu = br.ReadInt32();
            br.AssertUInt32(0);
            m_currentShapePpuAddress = des.ReadClassPointer<hknpShape>(br);
            br.AssertUInt64(0);
            m_maximumAabb = new hkAabb();
            m_maximumAabb.Read(des, br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_numLevelsOfDetail);
            m_infos.Write(bw);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(m_shapesMemorySizes);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_indexCurrentShapeOnSpu);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteUInt64(0);
            m_maximumAabb.Write(bw);
        }
    }
}
