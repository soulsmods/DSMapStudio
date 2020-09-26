using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hclBoneSpaceDeformerLocalBlockUnpackedPNTB : IHavokObject
    {
        public Vector4 m_localPosition_0;
        public Vector4 m_localPosition_1;
        public Vector4 m_localPosition_2;
        public Vector4 m_localPosition_3;
        public Vector4 m_localPosition_4;
        public Vector4 m_localPosition_5;
        public Vector4 m_localPosition_6;
        public Vector4 m_localPosition_7;
        public Vector4 m_localPosition_8;
        public Vector4 m_localPosition_9;
        public Vector4 m_localPosition_10;
        public Vector4 m_localPosition_11;
        public Vector4 m_localPosition_12;
        public Vector4 m_localPosition_13;
        public Vector4 m_localPosition_14;
        public Vector4 m_localPosition_15;
        public Vector4 m_localNormal_0;
        public Vector4 m_localNormal_1;
        public Vector4 m_localNormal_2;
        public Vector4 m_localNormal_3;
        public Vector4 m_localNormal_4;
        public Vector4 m_localNormal_5;
        public Vector4 m_localNormal_6;
        public Vector4 m_localNormal_7;
        public Vector4 m_localNormal_8;
        public Vector4 m_localNormal_9;
        public Vector4 m_localNormal_10;
        public Vector4 m_localNormal_11;
        public Vector4 m_localNormal_12;
        public Vector4 m_localNormal_13;
        public Vector4 m_localNormal_14;
        public Vector4 m_localNormal_15;
        public Vector4 m_localTangent_0;
        public Vector4 m_localTangent_1;
        public Vector4 m_localTangent_2;
        public Vector4 m_localTangent_3;
        public Vector4 m_localTangent_4;
        public Vector4 m_localTangent_5;
        public Vector4 m_localTangent_6;
        public Vector4 m_localTangent_7;
        public Vector4 m_localTangent_8;
        public Vector4 m_localTangent_9;
        public Vector4 m_localTangent_10;
        public Vector4 m_localTangent_11;
        public Vector4 m_localTangent_12;
        public Vector4 m_localTangent_13;
        public Vector4 m_localTangent_14;
        public Vector4 m_localTangent_15;
        public Vector4 m_localBiTangent_0;
        public Vector4 m_localBiTangent_1;
        public Vector4 m_localBiTangent_2;
        public Vector4 m_localBiTangent_3;
        public Vector4 m_localBiTangent_4;
        public Vector4 m_localBiTangent_5;
        public Vector4 m_localBiTangent_6;
        public Vector4 m_localBiTangent_7;
        public Vector4 m_localBiTangent_8;
        public Vector4 m_localBiTangent_9;
        public Vector4 m_localBiTangent_10;
        public Vector4 m_localBiTangent_11;
        public Vector4 m_localBiTangent_12;
        public Vector4 m_localBiTangent_13;
        public Vector4 m_localBiTangent_14;
        public Vector4 m_localBiTangent_15;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_localPosition_0 = des.ReadVector4(br);
            m_localPosition_1 = des.ReadVector4(br);
            m_localPosition_2 = des.ReadVector4(br);
            m_localPosition_3 = des.ReadVector4(br);
            m_localPosition_4 = des.ReadVector4(br);
            m_localPosition_5 = des.ReadVector4(br);
            m_localPosition_6 = des.ReadVector4(br);
            m_localPosition_7 = des.ReadVector4(br);
            m_localPosition_8 = des.ReadVector4(br);
            m_localPosition_9 = des.ReadVector4(br);
            m_localPosition_10 = des.ReadVector4(br);
            m_localPosition_11 = des.ReadVector4(br);
            m_localPosition_12 = des.ReadVector4(br);
            m_localPosition_13 = des.ReadVector4(br);
            m_localPosition_14 = des.ReadVector4(br);
            m_localPosition_15 = des.ReadVector4(br);
            m_localNormal_0 = des.ReadVector4(br);
            m_localNormal_1 = des.ReadVector4(br);
            m_localNormal_2 = des.ReadVector4(br);
            m_localNormal_3 = des.ReadVector4(br);
            m_localNormal_4 = des.ReadVector4(br);
            m_localNormal_5 = des.ReadVector4(br);
            m_localNormal_6 = des.ReadVector4(br);
            m_localNormal_7 = des.ReadVector4(br);
            m_localNormal_8 = des.ReadVector4(br);
            m_localNormal_9 = des.ReadVector4(br);
            m_localNormal_10 = des.ReadVector4(br);
            m_localNormal_11 = des.ReadVector4(br);
            m_localNormal_12 = des.ReadVector4(br);
            m_localNormal_13 = des.ReadVector4(br);
            m_localNormal_14 = des.ReadVector4(br);
            m_localNormal_15 = des.ReadVector4(br);
            m_localTangent_0 = des.ReadVector4(br);
            m_localTangent_1 = des.ReadVector4(br);
            m_localTangent_2 = des.ReadVector4(br);
            m_localTangent_3 = des.ReadVector4(br);
            m_localTangent_4 = des.ReadVector4(br);
            m_localTangent_5 = des.ReadVector4(br);
            m_localTangent_6 = des.ReadVector4(br);
            m_localTangent_7 = des.ReadVector4(br);
            m_localTangent_8 = des.ReadVector4(br);
            m_localTangent_9 = des.ReadVector4(br);
            m_localTangent_10 = des.ReadVector4(br);
            m_localTangent_11 = des.ReadVector4(br);
            m_localTangent_12 = des.ReadVector4(br);
            m_localTangent_13 = des.ReadVector4(br);
            m_localTangent_14 = des.ReadVector4(br);
            m_localTangent_15 = des.ReadVector4(br);
            m_localBiTangent_0 = des.ReadVector4(br);
            m_localBiTangent_1 = des.ReadVector4(br);
            m_localBiTangent_2 = des.ReadVector4(br);
            m_localBiTangent_3 = des.ReadVector4(br);
            m_localBiTangent_4 = des.ReadVector4(br);
            m_localBiTangent_5 = des.ReadVector4(br);
            m_localBiTangent_6 = des.ReadVector4(br);
            m_localBiTangent_7 = des.ReadVector4(br);
            m_localBiTangent_8 = des.ReadVector4(br);
            m_localBiTangent_9 = des.ReadVector4(br);
            m_localBiTangent_10 = des.ReadVector4(br);
            m_localBiTangent_11 = des.ReadVector4(br);
            m_localBiTangent_12 = des.ReadVector4(br);
            m_localBiTangent_13 = des.ReadVector4(br);
            m_localBiTangent_14 = des.ReadVector4(br);
            m_localBiTangent_15 = des.ReadVector4(br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
        }
    }
}
