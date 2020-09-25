using SoulsFormats;
using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public class hkxScene : hkReferencedObject
    {
        public string m_modeller;
        public string m_asset;
        public float m_sceneLength;
        public uint m_numFrames;
        public hkxNode m_rootNode;
        public List<hkxNodeSelectionSet> m_selectionSets;
        public List<hkxCamera> m_cameras;
        public List<hkxLight> m_lights;
        public List<hkxMaterial> m_materials;
        public List<hkxTextureInplace> m_inplaceTextures;
        public List<hkxTextureFile> m_externalTextures;
        public List<hkxSkinBinding> m_skinBindings;
        public List<hkxSpline> m_splines;
        public Matrix4x4 m_appliedTransform;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_modeller = des.ReadStringPointer(br);
            m_asset = des.ReadStringPointer(br);
            m_sceneLength = br.ReadSingle();
            m_numFrames = br.ReadUInt32();
            m_rootNode = des.ReadClassPointer<hkxNode>(br);
            m_selectionSets = des.ReadClassPointerArray<hkxNodeSelectionSet>(br);
            m_cameras = des.ReadClassPointerArray<hkxCamera>(br);
            m_lights = des.ReadClassPointerArray<hkxLight>(br);
            br.AssertUInt64(0);
            br.AssertUInt64(0);
            m_materials = des.ReadClassPointerArray<hkxMaterial>(br);
            m_inplaceTextures = des.ReadClassPointerArray<hkxTextureInplace>(br);
            m_externalTextures = des.ReadClassPointerArray<hkxTextureFile>(br);
            m_skinBindings = des.ReadClassPointerArray<hkxSkinBinding>(br);
            m_splines = des.ReadClassPointerArray<hkxSpline>(br);
            m_appliedTransform = des.ReadMatrix3(br);
        }
        
        public override void Write(BinaryWriterEx bw)
        {
            base.Write(bw);
            bw.WriteSingle(m_sceneLength);
            bw.WriteUInt32(m_numFrames);
            // Implement Write
            bw.WriteUInt64(0);
            bw.WriteUInt64(0);
        }
    }
}
