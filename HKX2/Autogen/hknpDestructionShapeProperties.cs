using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpDestructionShapeProperties : hkReferencedObject
    {
        public Matrix4x4 m_worldFromShape;
        public bool m_isHierarchicalCompound;
        public bool m_hasDestructionShapes;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_worldFromShape = des.ReadTransform(br);
            m_isHierarchicalCompound = br.ReadBoolean();
            m_hasDestructionShapes = br.ReadBoolean();
            br.AssertUInt64(0);
            br.AssertUInt32(0);
            br.AssertUInt16(0);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteBoolean(m_isHierarchicalCompound);
            bw.WriteBoolean(m_hasDestructionShapes);
            bw.WriteUInt64(0);
            bw.WriteUInt32(0);
            bw.WriteUInt16(0);
        }
    }
}
