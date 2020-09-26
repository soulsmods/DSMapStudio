using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkcdAdf : IHavokObject
    {
        public float m_accuracy;
        public hkAabb m_domain;
        public Vector4 m_origin;
        public Vector4 m_scale;
        public float m_range_0;
        public float m_range_1;
        public List<uint> m_nodes;
        public List<ushort> m_voxels;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_accuracy = br.ReadSingle();
            br.ReadUInt64();
            br.ReadUInt32();
            m_domain = new hkAabb();
            m_domain.Read(des, br);
            m_origin = des.ReadVector4(br);
            m_scale = des.ReadVector4(br);
            m_range_0 = br.ReadSingle();
            m_range_1 = br.ReadSingle();
            m_nodes = des.ReadUInt32Array(br);
            m_voxels = des.ReadUInt16Array(br);
            br.ReadUInt64();
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_accuracy);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            m_domain.Write(bw);
            bw.WriteSingle(m_range_0);
            bw.WriteSingle(m_range_1);
            bw.WriteUInt64(0);
        }
    }
}
