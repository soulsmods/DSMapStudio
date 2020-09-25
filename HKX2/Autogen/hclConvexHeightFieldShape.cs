using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclConvexHeightFieldShape : hclShape
    {
        public ushort m_res;
        public ushort m_resIncBorder;
        public Vector4 m_floatCorrectionOffset;
        public List<byte> m_heights;
        public int m_faces;
        public Matrix4x4 m_localToMapTransform;
        public Vector4 m_localToMapScale;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_res = br.ReadUInt16();
            m_resIncBorder = br.ReadUInt16();
            br.AssertUInt32(0);
            m_floatCorrectionOffset = des.ReadVector4(br);
            m_heights = des.ReadByteArray(br);
            m_faces = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_localToMapTransform = des.ReadTransform(br);
            m_localToMapScale = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteUInt16(m_res);
            bw.WriteUInt16(m_resIncBorder);
            bw.WriteUInt32(0);
            bw.WriteInt32(m_faces);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
