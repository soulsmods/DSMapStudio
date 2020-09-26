using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkpMultiSphereShape : hkpSphereRepShape
    {
        public int m_numSpheres;
        public Vector4 m_spheres_0;
        public Vector4 m_spheres_1;
        public Vector4 m_spheres_2;
        public Vector4 m_spheres_3;
        public Vector4 m_spheres_4;
        public Vector4 m_spheres_5;
        public Vector4 m_spheres_6;
        public Vector4 m_spheres_7;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numSpheres = br.ReadInt32();
            br.ReadUInt64();
            br.ReadUInt32();
            m_spheres_0 = des.ReadVector4(br);
            m_spheres_1 = des.ReadVector4(br);
            m_spheres_2 = des.ReadVector4(br);
            m_spheres_3 = des.ReadVector4(br);
            m_spheres_4 = des.ReadVector4(br);
            m_spheres_5 = des.ReadVector4(br);
            m_spheres_6 = des.ReadVector4(br);
            m_spheres_7 = des.ReadVector4(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt32(m_numSpheres);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
        }
    }
}
