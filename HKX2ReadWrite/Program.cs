using SoulsFormats;
using System;
using System.IO;
using HKX2;

namespace HKX2ReadWrite
{
    class Program
    {
        static void Main(string[] args)
        {
            var hkxpath = args[0];
            using (FileStream stream = File.OpenRead(hkxpath))
            {
                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                var des = new HKX2.PackFileDeserializer();
                var root = (hkRootLevelContainer)des.Deserialize(br);

                // Strip some stuff
                var v = (hknpPhysicsSceneData)root.m_namedVariants[0].m_variant;
                foreach (fsnpCustomParamCompressedMeshShape s in v.m_systemDatas[0].m_referencedObjects)
                {
                    s.m_triangleIndexToShapeKey = null;
                    s.m_pParam = null;
                    s.m_edgeWeldingMap.m_primaryKeyToIndex = null;
                    s.m_edgeWeldingMap.m_secondaryKeyMask = 0;
                    s.m_edgeWeldingMap.m_sencondaryKeyBits = 0;
                    s.m_edgeWeldingMap.m_valueAndSecondaryKeys = null;
                    s.m_quadIsFlat.m_storage.m_numBits = 0;
                    s.m_quadIsFlat.m_storage.m_words = null;
                    s.m_triangleIsInterior.m_storage.m_numBits = 0;
                    s.m_triangleIsInterior.m_storage.m_words = null;
                }

                using (FileStream s2 = File.Create(hkxpath + ".out"))
                {
                    BinaryWriterEx bw = new BinaryWriterEx(false, s2);
                    var s = new HKX2.PackFileSerializer();
                    s.Serialize(root, bw);
                }
            }
        }
    }
}
