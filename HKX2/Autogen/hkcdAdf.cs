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
        public float m_range;
        public List<uint> m_nodes;
        public List<ushort> m_voxels;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_accuracy = br.ReadSingle();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            m_domain = new hkAabb();
            m_domain.Read(des, br);
            m_origin = des.ReadVector4(br);
            m_scale = des.ReadVector4(br);
            m_range = br.ReadSingle();
            br.AssertUInt32(0);
            m_nodes = des.ReadUInt32Array(br);
            m_voxels = des.ReadUInt16Array(br);
            br.AssertUInt64(0);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            bw.WriteSingle(m_accuracy);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            m_domain.Write(bw);
            bw.WriteSingle(m_range);
            bw.WriteUInt32(0);
            bw.WriteUInt64(0);
        }
    }
}
