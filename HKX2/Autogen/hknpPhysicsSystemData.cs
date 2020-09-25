using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hknpPhysicsSystemData : hkReferencedObject
    {
        public List<hknpMaterial> m_materials;
        public List<hknpMotionProperties> m_motionProperties;
        public List<hknpMotionCinfo> m_motionCinfos;
        public List<hknpBodyCinfo> m_bodyCinfos;
        public List<hknpConstraintCinfo> m_constraintCinfos;
        public List<hkReferencedObject> m_referencedObjects;
        public string m_name;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_materials = des.ReadClassArray<hknpMaterial>(br);
            m_motionProperties = des.ReadClassArray<hknpMotionProperties>(br);
            m_motionCinfos = des.ReadClassArray<hknpMotionCinfo>(br);
            m_bodyCinfos = des.ReadClassArray<hknpBodyCinfo>(br);
            m_constraintCinfos = des.ReadClassArray<hknpConstraintCinfo>(br);
            m_referencedObjects = des.ReadClassPointerArray<hkReferencedObject>(br);
            m_name = des.ReadStringPointer(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
        }
    }
}
