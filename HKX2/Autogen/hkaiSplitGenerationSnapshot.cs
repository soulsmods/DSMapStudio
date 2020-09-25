using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkaiSplitGenerationSnapshot : IHavokObject
    {
        public hkaiNavMeshGenerationSnapshot m_generationSnapshot;
        public hkaiSplitGenerationUtilsSettings m_splitSettings;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_generationSnapshot = new hkaiNavMeshGenerationSnapshot();
            m_generationSnapshot.Read(des, br);
            m_splitSettings = new hkaiSplitGenerationUtilsSettings();
            m_splitSettings.Read(des, br);
        }
        
        public virtual void Write(BinaryWriterEx bw)
        {
            m_generationSnapshot.Write(bw);
            m_splitSettings.Write(bw);
        }
    }
}
