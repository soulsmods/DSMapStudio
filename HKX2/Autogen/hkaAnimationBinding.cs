using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public enum BlendHint
    {
        NORMAL = 0,
        ADDITIVE_DEPRECATED = 1,
        ADDITIVE = 2,
    }
    
    public class hkaAnimationBinding : hkReferencedObject
    {
        public string m_originalSkeletonName;
        public hkaAnimation m_animation;
        public List<short> m_transformTrackToBoneIndices;
        public List<short> m_floatTrackToFloatSlotIndices;
        public List<short> m_partitionIndices;
        public BlendHint m_blendHint;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_originalSkeletonName = des.ReadStringPointer(br);
            m_animation = des.ReadClassPointer<hkaAnimation>(br);
            m_transformTrackToBoneIndices = des.ReadInt16Array(br);
            m_floatTrackToFloatSlotIndices = des.ReadInt16Array(br);
            m_partitionIndices = des.ReadInt16Array(br);
            m_blendHint = (BlendHint)br.ReadSByte();
            br.ReadUInt32();
            br.ReadUInt16();
            br.ReadByte();
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            // Implement Write
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
        }
    }
}
