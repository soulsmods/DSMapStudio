using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BlendType
    {
        BLEND_TYPE_BLEND_IN = 0,
        BLEND_TYPE_FULL_ON = 1,
    }
    
    public enum DockingFlagBits
    {
        FLAG_NONE = 0,
        FLAG_DOCK_TO_FUTURE_POSITION = 1,
        FLAG_OVERRIDE_MOTION = 2,
    }
    
    public class hkbDockingGenerator : hkbGenerator
    {
        public short m_dockingBone;
        public Vector4 m_translationOffset;
        public Quaternion m_rotationOffset;
        public BlendType m_blendType;
        public ushort m_flags;
        public hkbGenerator m_child;
        public int m_intervalStart;
        public int m_intervalEnd;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_dockingBone = br.ReadInt16();
            br.AssertUInt32(0);
            br.AssertUInt16(0);
            m_translationOffset = des.ReadVector4(br);
            m_rotationOffset = des.ReadQuaternion(br);
            m_blendType = (BlendType)br.ReadSByte();
            br.AssertByte(0);
            m_flags = br.ReadUInt16();
            br.AssertUInt32(0);
            m_child = des.ReadClassPointer<hkbGenerator>(br);
            m_intervalStart = br.ReadInt32();
            m_intervalEnd = br.ReadInt32();
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteInt16(m_dockingBone);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteUInt32(0);
            // Implement Write
            bw.WriteInt32(m_intervalStart);
            bw.WriteInt32(m_intervalEnd);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
