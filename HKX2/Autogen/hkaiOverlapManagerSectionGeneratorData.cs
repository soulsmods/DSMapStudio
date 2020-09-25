using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiOverlapManagerSectionGeneratorData : hkReferencedObject
    {
        public hkaiSilhouetteGeneratorSectionContext m_context;
        public List<int> m_overlappedFaces;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_context = new hkaiSilhouetteGeneratorSectionContext();
            m_context.Read(des, br);
            m_overlappedFaces = des.ReadInt32Array(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            m_context.Write(bw);
        }
    }
}
