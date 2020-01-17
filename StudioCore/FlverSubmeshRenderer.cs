using StudioCore.DebugPrimitives;
//using MeowDSIO.DataTypes.FLVER;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using StudioCore.Scene;

namespace StudioCore
{
    public class FlverSubmeshRenderer : Scene.RenderObject, IDisposable
    {
        public BoundingBox Bounds;

        private bool HasNoLODs = true;

        private Resource.ResourceHandle<Resource.FlverResource> FlverResource;
        private int FlverMeshIndex;

        bool WorldDirty = false;
        private Matrix4x4 _World = Matrix4x4.Identity;
        public Matrix4x4 WorldTransform
        {
            get
            {
                return _World;
            }
            set
            {
                _World = value;
                WorldDirty = true;
            }
        }

        protected Pipeline RenderPipeline;
        protected Shader[] Shaders;
        protected DeviceBuffer WorldBuffer;
        protected ResourceSet PerObjRS;

        public int VertexCount { get; private set; }

        public readonly NewMesh Parent;

        public bool IsVisible { get; set; } = true;

        private string _fullMaterialName;
        public string FullMaterialName
        {
            get => _fullMaterialName;
            set
            {
                if (_fullMaterialName != value)
                {
                    _fullMaterialName = value;
                    var shit = GetModelMaskIndexAndPrettyNameStartForCurrentMaterialName();
                    ModelMaskIndex = shit.MaskIndex;
                    PrettyMaterialName = value.Substring(shit.SubstringStartIndex).Trim();
                }
            }
        }

        public string PrettyMaterialName { get; private set; }

        private (int MaskIndex, int SubstringStartIndex) GetModelMaskIndexAndPrettyNameStartForCurrentMaterialName()
        {
            if (string.IsNullOrEmpty(FullMaterialName))
                return (-1, 0);

            int firstHashtag = FullMaterialName.IndexOf("#");
            if (firstHashtag == -1)
                return (-1, 0);
            int secondHashtagSearchStart = firstHashtag + 1;
            int secondHashtag = FullMaterialName.Substring(secondHashtagSearchStart).IndexOf("#");
            if (secondHashtag == -1)
                return (-1, 0);
            else
                secondHashtag += secondHashtagSearchStart;

            string maskText = FullMaterialName.Substring(secondHashtagSearchStart, secondHashtag - secondHashtagSearchStart);

            if (int.TryParse(maskText, out int mask))
                return (mask, secondHashtag + 1);
            else
                return (-1, 0);
        }

        //public string DefaultBoneName { get; set; } = null;
        public int DefaultBoneIndex { get; set; } = -1;

        public FlverShadingMode ShadingMode { get; set; } = FlverShadingMode.PBR_GLOSS_DS3;

        public int ModelMaskIndex { get; private set; }

        static System.Numerics.Vector3 SkinVector3(System.Numerics.Vector3 vOof, Matrix4x4[] bones, FLVER.VertexBoneWeights weights, bool isNormal = false)
        {
            var v = new Vector3(vOof.X, vOof.Y, vOof.Z);
            Vector3 a = isNormal ? Vector3.TransformNormal(v, bones[0]) * weights[0] : Vector3.Transform(v, bones[0]) * weights[0];
            Vector3 b = isNormal ? Vector3.TransformNormal(v, bones[1]) * weights[1] : Vector3.Transform(v, bones[1]) * weights[1];
            Vector3 c = isNormal ? Vector3.TransformNormal(v, bones[2]) * weights[2] : Vector3.Transform(v, bones[2]) * weights[2];
            Vector3 d = isNormal ? Vector3.TransformNormal(v, bones[3]) * weights[3] : Vector3.Transform(v, bones[3]) * weights[3];

            var r = (a + b + c + d) / (weights[0] + weights[1] + weights[2] + weights[3]);
            return new System.Numerics.Vector3(r.X, r.Y, r.Z);
        }

        static System.Numerics.Vector4 SkinVector4(System.Numerics.Vector4 vOof, Matrix4x4[] bones, FLVER.VertexBoneWeights weights, bool isNormal = false)
        {
            var v = new Vector4(vOof.X, vOof.Y, vOof.Z, vOof.W);

            Vector3 a = isNormal ? Vector3.TransformNormal(v.XYZ(), bones[0]) * weights[0] : Vector3.Transform(v.XYZ(), bones[0]) * weights[0];
            Vector3 b = isNormal ? Vector3.TransformNormal(v.XYZ(), bones[1]) * weights[1] : Vector3.Transform(v.XYZ(), bones[1]) * weights[1];
            Vector3 c = isNormal ? Vector3.TransformNormal(v.XYZ(), bones[2]) * weights[2] : Vector3.Transform(v.XYZ(), bones[2]) * weights[2];
            Vector3 d = isNormal ? Vector3.TransformNormal(v.XYZ(), bones[3]) * weights[3] : Vector3.Transform(v.XYZ(), bones[3]) * weights[3];

            var r = (a + b + c + d) / (weights[0] + weights[1] + weights[2] + weights[3]);
            return new System.Numerics.Vector4(r.X, r.Y, r.Z, v.W);
        }

        static void ApplySkin(FLVER.Vertex vert, 
            List<Matrix4x4> boneMatrices, List<int> meshBoneIndices, bool usesMeshBoneIndices)
        {
            int i1 = vert.BoneIndices[0];
            int i2 = vert.BoneIndices[1];
            int i3 = vert.BoneIndices[2];
            int i4 = vert.BoneIndices[3];

            if (usesMeshBoneIndices)
            {
                i1 = meshBoneIndices[i1];
                i2 = meshBoneIndices[i2];
                i3 = meshBoneIndices[i3];
                i4 = meshBoneIndices[i4];
            }

            vert.Position = SkinVector3(vert.Position, new Matrix4x4[]
                {
                    i1 >= 0 ? boneMatrices[i1] : Matrix4x4.Identity,
                    i2 >= 0 ? boneMatrices[i2] : Matrix4x4.Identity,
                    i3 >= 0 ? boneMatrices[i3] : Matrix4x4.Identity,
                    i4 >= 0 ? boneMatrices[i4] : Matrix4x4.Identity,
                }, vert.BoneWeights);

            vert.Normal = SkinVector3(vert.Normal, new Matrix4x4[]
                {
                    i1 >= 0 ? boneMatrices[i1] : Matrix4x4.Identity,
                    i2 >= 0 ? boneMatrices[i2] : Matrix4x4.Identity,
                    i3 >= 0 ? boneMatrices[i3] : Matrix4x4.Identity,
                    i4 >= 0 ? boneMatrices[i4] : Matrix4x4.Identity,
                }, vert.BoneWeights, isNormal: true);

            vert.Bitangent = SkinVector4(vert.Bitangent, new Matrix4x4[]
                {
                    i1 >= 0 ? boneMatrices[i1] : Matrix4x4.Identity,
                    i2 >= 0 ? boneMatrices[i2] : Matrix4x4.Identity,
                    i3 >= 0 ? boneMatrices[i3] : Matrix4x4.Identity,
                    i4 >= 0 ? boneMatrices[i4] : Matrix4x4.Identity,
                }, vert.BoneWeights, isNormal: true);

            vert.Tangents[0] = SkinVector4(vert.Tangents[0], new Matrix4x4[]
                {
                    i1 >= 0 ? boneMatrices[i1] : Matrix4x4.Identity,
                    i2 >= 0 ? boneMatrices[i2] : Matrix4x4.Identity,
                    i3 >= 0 ? boneMatrices[i3] : Matrix4x4.Identity,
                    i4 >= 0 ? boneMatrices[i4] : Matrix4x4.Identity,
                }, vert.BoneWeights, isNormal: true);
        }

        public FlverSubmeshRenderer(NewMesh parent, Resource.ResourceHandle<Resource.FlverResource> resourceHandle, int meshIndex,
            bool useSecondUV, Dictionary<string, int> boneIndexRemap = null,
            bool ignoreStaticTransforms = false)
        {
            FlverResource = resourceHandle;
            FlverMeshIndex = meshIndex;
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            var factory = gd.ResourceFactory;
            WorldBuffer = factory.CreateBuffer(new BufferDescription(64, Veldrid.BufferUsage.UniformBuffer | Veldrid.BufferUsage.Dynamic));
            gd.UpdateBuffer(WorldBuffer, 0, ref _World, 64);

            ResourceLayout projViewCombinedLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));

            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("position", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Float3),
                    new VertexElementDescription("uv1", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.UShort2),
                    new VertexElementDescription("normal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.SByte4),
                    new VertexElementDescription("binormal", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4),
                    new VertexElementDescription("bitangent", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4),
                    new VertexElementDescription("color", VertexElementSemantic.TextureCoordinate, Veldrid.VertexElementFormat.Byte4))
            };

            var res = StaticResourceCache.GetShaders(gd, gd.ResourceFactory, "SimpleFlver").ToTuple();
            Shaders = new Shader[] { res.Item1, res.Item2 };

            ResourceLayout projViewLayout = StaticResourceCache.GetResourceLayout(
                gd.ResourceFactory,
                StaticResourceCache.ProjViewLayoutDescription);

            ResourceLayout mainPerObjectLayout = StaticResourceCache.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding)));


            PerObjRS = factory.CreateResourceSet(new ResourceSetDescription(mainPerObjectLayout,
                WorldBuffer));

            bool isTriStrip = false;
            var fres = FlverResource.Get();
            if (fres != null)
            {
                var mesh = fres.GPUMeshes[FlverMeshIndex];
                isTriStrip = mesh.MeshFacesets[0].IsTriangleStrip;
            }

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = gd.IsDepthRangeZeroToOne ? DepthStencilStateDescription.DepthOnlyLessEqual : DepthStencilStateDescription.DepthOnlyGreaterEqual;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.CounterClockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = isTriStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: mainVertexLayouts,
                shaders: Shaders);
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { projViewLayout, mainPerObjectLayout };
            pipelineDescription.Outputs = gd.SwapchainFramebuffer.OutputDescription;
            RenderPipeline = factory.CreateGraphicsPipeline(pipelineDescription);
        }

        public override void DestroyDeviceObjects()
        {
            throw new NotImplementedException();
        }

        public override void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (WorldDirty)
            {
                cl.UpdateBuffer(WorldBuffer, 0, ref _World, 64);
                WorldDirty = false;
            }
        }

        public override void Render(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp)
        {
            if (!IsVisible)
                return;

            if (FlverResource == null || !FlverResource.IsLoaded || FlverResource.Get() == null)
                return;

            //if (mask != null && ModelMaskIndex >= 0 && !mask[ModelMaskIndex])
            //    return;

            if (FlverResource.TryLock())
            {
                var resource = FlverResource.Get();
                var mesh = resource.GPUMeshes[FlverMeshIndex];
                var vertbuffer = mesh.VertBuffer;

                cl.SetPipeline(RenderPipeline);
                cl.SetGraphicsResourceSet(0, sp.ProjViewRS);
                uint offset = 0;
                cl.SetGraphicsResourceSet(1, PerObjRS, 1, ref offset);
                cl.SetVertexBuffer(0, vertbuffer);

                foreach (var faceSet in mesh.MeshFacesets)
                {
                    if (faceSet.IndexCount == 0)
                        continue;
                    if (faceSet.LOD != 0)
                        continue;

                    //GFX.Device.DrawIndexedPrimitives(faceSet.IsTriangleStrip ? PrimitiveType.TriangleStrip : PrimitiveType.TriangleList, 0, 0,
                    //    faceSet.IsTriangleStrip ? (faceSet.IndexCount - 2) : (faceSet.IndexCount / 3));

                    cl.SetIndexBuffer(faceSet.IndexBuffer, faceSet.Is32Bit ? IndexFormat.UInt32 : IndexFormat.UInt16);
                    cl.DrawIndexed(faceSet.IndexBuffer.SizeInBytes / (faceSet.Is32Bit ? 4u : 2u), 1, 0, 0, 0);
                }
                FlverResource.Unlock();
            }
        }

        public void Dispose()
        {

            //VertBuffer.Dispose();

            // Just leave the texture data as-is, since 
            // TexturePool handles memory cleanup
        }
    }
}
