using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using Veldrid;
using Veldrid.Utilities;
using SoulsFormats;
using System.ComponentModel.DataAnnotations;
using StudioCore.MsbEditor;

namespace StudioCore.Resource
{
    public class FlverResource : IResource, IDisposable
    {
        private static Stack<FlverCache> FlverCaches = new Stack<FlverCache>();
        public static int CacheCount { get; private set; } = 0;
        public static long CacheFootprint
        {
            get
            {
                long total = 0;
                lock (CacheLock)
                {
                    foreach (var c in FlverCaches)
                    {
                        total += c.MemoryUsage;
                    }
                }
                return total;
            }
        }
        private static object CacheLock = new object();

        //private static ArrayPool<FlverLayout> VerticesPool = ArrayPool<FlverLayout>.Create();

        public const bool CaptureMaterialLayouts = false;

        /// <summary>
        /// Cache of material layouts that can be dumped
        /// </summary>
        public static Dictionary<string, FLVER2.BufferLayout> MaterialLayouts = new Dictionary<string, FLVER2.BufferLayout>();
        public static object _matLayoutLock = new object();

        private FlverCache GetCache()
        {
            lock (CacheLock)
            {
                if (FlverCaches.Count > 0)
                {
                    return FlverCaches.Pop();
                }
                CacheCount++;
            }
            return new FlverCache();
        }

        private void ReleaseCache(FlverCache cache)
        {
            if (cache != null)
            {
                cache.ResetUsage();
                lock (CacheLock)
                {
                    FlverCaches.Push(cache);
                }
            }
        }

        public static void PurgeCaches()
        {
            FlverCaches.Clear();
            //VerticesPool = ArrayPool<FlverLayout>.Create();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public class FlverMaterial : IResourceEventListener
        {
            public string MaterialName;
            public Scene.GPUBufferAllocator.GPUBufferHandle MaterialBuffer;
            public Scene.Material MaterialData;

            public string ShaderName = null;
            public MeshLayoutType LayoutType;
            public SpecializationConstant[] SpecializationConstants = null;
            public VertexLayoutDescription VertexLayout;
            public uint VertexSize;

            public TextureResourceHande AlbedoTextureResource = null;
            public TextureResourceHande AlbedoTextureResource2 = null;
            public TextureResourceHande NormalTextureResource = null;
            public TextureResourceHande NormalTextureResource2 = null;
            public TextureResourceHande SpecularTextureResource = null;
            public TextureResourceHande SpecularTextureResource2 = null;
            public TextureResourceHande ShininessTextureResource = null;
            public TextureResourceHande ShininessTextureResource2 = null;
            public TextureResourceHande BlendmaskTextureResource = null;

            private void SetMaterialTexture(TextureResourceHande handle, ref ushort matTex, ushort defaultTex)
            {
                if (handle != null && handle.IsLoaded && handle.TryLock())
                {
                    var res = handle.Get();
                    if (res != null && res.GPUTexture != null)
                    {
                        matTex = (ushort)handle.Get().GPUTexture.TexHandle;
                    }
                    else
                    {
                        matTex = defaultTex;
                    }
                    handle.Unlock();
                }
                else
                {
                    matTex = defaultTex;
                }
            }

            private void ReleaseTexture(TextureResourceHande handle)
            {
                if (handle != null)
                {
                    handle.Release();
                }
            }

            public void ReleaseTextures()
            {
                ReleaseTexture(AlbedoTextureResource);
                ReleaseTexture(AlbedoTextureResource2);
                ReleaseTexture(NormalTextureResource);
                ReleaseTexture(NormalTextureResource2);
                ReleaseTexture(SpecularTextureResource);
                ReleaseTexture(SpecularTextureResource2);
                ReleaseTexture(ShininessTextureResource);
                ReleaseTexture(ShininessTextureResource2);
                ReleaseTexture(BlendmaskTextureResource);
            }

            public void UpdateMaterial()
            {
                SetMaterialTexture(AlbedoTextureResource, ref MaterialData.colorTex, 0);
                SetMaterialTexture(AlbedoTextureResource2, ref MaterialData.colorTex2, 0);
                SetMaterialTexture(NormalTextureResource, ref MaterialData.normalTex, 1);
                SetMaterialTexture(NormalTextureResource2, ref MaterialData.normalTex2, 1);
                SetMaterialTexture(SpecularTextureResource, ref MaterialData.specTex, 2);
                SetMaterialTexture(SpecularTextureResource2, ref MaterialData.specTex2, 2);
                SetMaterialTexture(ShininessTextureResource, ref MaterialData.shininessTex, 2);
                SetMaterialTexture(ShininessTextureResource2, ref MaterialData.shininessTex2, 2);
                SetMaterialTexture(BlendmaskTextureResource, ref MaterialData.blendMaskTex, 0);

                Scene.Renderer.AddBackgroundUploadTask((d, cl) =>
                {
                    MaterialBuffer.FillBuffer(cl, ref MaterialData);
                });
            }

            public void OnResourceLoaded(IResourceHandle handle)
            {
                UpdateMaterial();
            }

            public void OnResourceUnloaded(IResourceHandle handle)
            {
                UpdateMaterial();
            }
        }

        public unsafe class FlverSubmesh
        {
            public struct FlverSubmeshFaceSet
            {
                public int IndexCount;
                public int IndexOffset;
                public int PickingIndicesCount;
                //public IntPtr PickingIndices;
                public bool BackfaceCulling;
                public bool IsTriangleStrip;
                public byte LOD;
                public bool IsMotionBlur;
                public bool Is32Bit;
            }

            public List<FlverSubmeshFaceSet> MeshFacesets { get; set; } = new List<FlverSubmeshFaceSet>();

            public Scene.VertexIndexBufferAllocator.VertexIndexBufferHandle GeomBuffer { get; set; }

            public int VertexCount { get; set; }
            // This is native because using managed arrays causes a weird memory leak
            public IntPtr PickingVertices = IntPtr.Zero;

            public BoundingBox Bounds { get; set; }

            public int DefaultBoneIndex { get; set; } = -1;

            public FlverMaterial Material { get; set; } = null;
        }

        /// <summary>
        /// Low level access to the flver struct. Use only in modification mode.
        /// </summary>
        public FLVER0 FlverDeS = null;
        public FLVER2 Flver = null;

        public FlverSubmesh[] GPUMeshes = null;
        public FlverMaterial[] GPUMaterials = null;

        public BoundingBox Bounds { get; set; }

        public List<FLVER.Bone> Bones { get; private set; } = null;

        private string TexturePathToVirtual(string texpath)
        {
            if (texpath.Contains(@"\map\"))
            {
                var splits = texpath.Split('\\');
                var mapid = splits[splits.Length - 3];
                return $@"map/tex/{mapid}/{Path.GetFileNameWithoutExtension(texpath)}";
            }
            // Chr texture reference
            else if (texpath.Contains(@"\chr\"))
            {
                var splits = texpath.Split('\\');
                var chrid = splits[splits.Length - 3];
                return $@"chr/{chrid}/tex/{Path.GetFileNameWithoutExtension(texpath)}";
            }
            // Obj texture reference
            else if (texpath.Contains(@"\obj\"))
            {
                var splits = texpath.Split('\\');
                var objid = splits[splits.Length - 3];
                return $@"obj/{objid}/tex/{Path.GetFileNameWithoutExtension(texpath)}";
            }
            // Parts texture reference
            /*else if (texpath.Contains(@"\parts\"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Texture2D>($@"Assets/{gamePath}/Parts/textures/{Path.GetFileNameWithoutExtension(path)}.dds");
                return asset;
            }*/
            return texpath;
        }

        private void LookupTexture(ref TextureResourceHande handle, FlverMaterial dest, IFlverTexture matparam, string mtd)
        {
            var path = matparam.Path;
            if (matparam.Path == "")
            {
                var mtdstring = Path.GetFileNameWithoutExtension(mtd);
                if (MtdBank.Mtds.ContainsKey(mtdstring))
                {
                    var tex = MtdBank.Mtds[mtdstring].Textures.Find(x => (x.Type == matparam.Type));
                    if (tex == null || !tex.Extended || tex.Path == "")
                    {
                        return;
                    }
                    path = tex.Path;
                }
            }
            handle = ResourceManager.GetTextureResource(TexturePathToVirtual(path.ToLower()));
            handle.Acquire();
            handle.AddResourceEventListener(dest);

        }

        unsafe private void ProcessMaterial(IFlverMaterial mat, FlverMaterial dest, GameType type)
        {
            dest.MaterialName = Path.GetFileNameWithoutExtension(mat.MTD);
            dest.MaterialBuffer = Scene.Renderer.MaterialBufferAllocator.Allocate((uint)sizeof(Scene.Material), sizeof(Scene.Material));
            dest.MaterialData = new Scene.Material();

            if (!CFG.Current.EnableTexturing)
            {
                dest.ShaderName = @"SimpleFlver";
                dest.LayoutType = MeshLayoutType.LayoutSky;
                dest.VertexLayout = MeshLayoutUtils.GetLayoutDescription(dest.LayoutType);
                dest.VertexSize = MeshLayoutUtils.GetLayoutVertexSize(dest.LayoutType);
                dest.SpecializationConstants = new SpecializationConstant[0];
                return;
            }

            bool blend = false;
            bool blendMask = false;
            bool hasNormal2 = false;
            bool hasSpec2 = false;
            bool hasShininess2 = false;

            foreach (var matparam in mat.Textures)
            {
                string paramNameCheck;
                if (matparam.Type == null)
                {
                    paramNameCheck = "G_DIFFUSE";
                }
                else
                {
                    paramNameCheck = matparam.Type.ToUpper();
                }
                if (paramNameCheck == "G_DIFFUSETEXTURE2" || paramNameCheck == "G_DIFFUSE2" || paramNameCheck.Contains("ALBEDO_2"))
                {
                    LookupTexture(ref dest.AlbedoTextureResource2, dest, matparam, mat.MTD);
                    blend = true;
                }
                else if (paramNameCheck == "G_DIFFUSETEXTURE" || paramNameCheck == "G_DIFFUSE" || paramNameCheck.Contains("ALBEDO"))
                {
                    LookupTexture(ref dest.AlbedoTextureResource, dest, matparam, mat.MTD);
                }
                else if (paramNameCheck == "G_BUMPMAPTEXTURE2" || paramNameCheck == "G_BUMPMAP2" || paramNameCheck.Contains("NORMAL_2"))
                {
                    LookupTexture(ref dest.NormalTextureResource2, dest, matparam, mat.MTD);
                    blend = true;
                    hasNormal2 = true;
                }
                else if (paramNameCheck == "G_BUMPMAPTEXTURE" || paramNameCheck == "G_BUMPMAP" || paramNameCheck.Contains("NORMAL"))
                {
                    LookupTexture(ref dest.NormalTextureResource, dest, matparam, mat.MTD);
                }
                else if (paramNameCheck == "G_SPECULARTEXTURE2" || paramNameCheck == "G_SPECULAR2" || paramNameCheck.Contains("SPECULAR_2"))
                {
                    LookupTexture(ref dest.SpecularTextureResource2, dest, matparam, mat.MTD);
                    blend = true;
                    hasSpec2 = true;
                }
                else if (paramNameCheck == "G_SPECULARTEXTURE" || paramNameCheck == "G_SPECULAR" || paramNameCheck.Contains("SPECULAR"))
                {
                    LookupTexture(ref dest.SpecularTextureResource, dest, matparam, mat.MTD);
                }
                else if (paramNameCheck == "G_SHININESSTEXTURE2" || paramNameCheck == "G_SHININESS2" || paramNameCheck.Contains("SHININESS2"))
                {
                    LookupTexture(ref dest.ShininessTextureResource2, dest, matparam, mat.MTD);
                    blend = true;
                    hasShininess2 = true;
                }
                else if (paramNameCheck == "G_SHININESSTEXTURE" || paramNameCheck == "G_SHININESS" || paramNameCheck.Contains("SHININESS"))
                {
                    LookupTexture(ref dest.ShininessTextureResource, dest, matparam, mat.MTD);
                }
                else if (paramNameCheck.Contains("BLENDMASK"))
                {
                    LookupTexture(ref dest.BlendmaskTextureResource, dest, matparam, mat.MTD);
                    blendMask = true;
                }
            }

            if (blendMask)
            {
                dest.ShaderName = @"FlverShader\FlverShader_blendmask";
                dest.LayoutType = MeshLayoutType.LayoutUV2;
            }
            else if (blend)
            {
                dest.ShaderName = @"FlverShader\FlverShader_blend";
                dest.LayoutType = MeshLayoutType.LayoutUV2;
            }
            else
            {
                dest.ShaderName = @"FlverShader\FlverShader";
                dest.LayoutType = MeshLayoutType.LayoutStandard;
            }

            List<SpecializationConstant> specConstants = new List<SpecializationConstant>();
            specConstants.Add(new SpecializationConstant(0, (uint)type));
            if (blend || blendMask)
            {
                specConstants.Add(new SpecializationConstant(1, hasNormal2));
                specConstants.Add(new SpecializationConstant(2, hasSpec2));
                specConstants.Add(new SpecializationConstant(3, hasShininess2));
            }

            dest.SpecializationConstants = specConstants.ToArray();
            dest.VertexLayout = MeshLayoutUtils.GetLayoutDescription(dest.LayoutType);
            dest.VertexSize = MeshLayoutUtils.GetLayoutVertexSize(dest.LayoutType);

            dest.UpdateMaterial();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FillVertex(ref Vector3 dest, ref FLVER.Vertex v)
        {
            dest = v.Position;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillNormalSNorm8(sbyte *dest, ref FLVER.Vertex v)
        {
            var n = Vector3.Normalize(new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z));
            dest[0] = (sbyte)(n.X * 127.0f);
            dest[1] = (sbyte)(n.Y * 127.0f);
            dest[2] = (sbyte)(n.Z * 127.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillUVShort(short* dest, ref FLVER.Vertex v, byte index)
        {
            var uv = v.GetUV(index);
            dest[0] = (short)(uv.X * 2048.0f);
            dest[1] = (short)(uv.Y * 2048.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillUVShortZero(short* dest)
        {
            dest[0] = 0;
            dest[1] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillUVFloat(ref Vector2 dest, ref FLVER.Vertex v, byte index)
        {
            var uv = v.GetUV(index);
            dest.X = uv.X;
            dest.Y = uv.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillBinormalBitangentSNorm8(sbyte* destBinorm, sbyte* destBitan, ref FLVER.Vertex v, byte index)
        {
            var tan = v.GetTangent(index);
            var t = Vector3.Normalize(new Vector3(tan.X, tan.Y, tan.Z));
            destBitan[0] = (sbyte)(t.X * 127.0f);
            destBitan[1] = (sbyte)(t.Y * 127.0f);
            destBitan[2] = (sbyte)(t.Z * 127.0f);
            destBitan[3] = (sbyte)(tan.W * 127.0f);

            var bn = Vector3.Cross(Vector3.Normalize(v.Normal), Vector3.Normalize(new Vector3(t.X, t.Y, t.Z))) * tan.W;
            destBinorm[0] = (sbyte)(bn.X * 127.0f);
            destBinorm[1] = (sbyte)(bn.Y * 127.0f);
            destBinorm[2] = (sbyte)(bn.Z * 127.0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillBinormalBitangentSNorm8Zero(sbyte* destBinorm, sbyte* destBitan)
        {
            destBitan[0] = 0;
            destBitan[1] = 0;
            destBitan[2] = 0;
            destBitan[3] = 127;

            destBinorm[0] = 0;
            destBinorm[1] = 0;
            destBinorm[2] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void FillColorUNorm(byte* dest, ref FLVER.Vertex v)
        {
            
        }

        unsafe private void FillVerticesNormalOnly(FLVER2.Mesh mesh, Span<Vector3> pickingVerts, IntPtr vertBuffer)
        {
            Span<FlverLayoutSky> verts = new Span<FlverLayoutSky>(vertBuffer.ToPointer(), mesh.VertexCount);
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vert = mesh.Vertices[i];

                verts[i] = new FlverLayoutSky();
                pickingVerts[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                fixed (FlverLayoutSky* v = &verts[i])
                {
                    FillVertex(ref (*v).Position, ref vert);
                    FillNormalSNorm8((*v).Normal, ref vert);
                }
            }
        }

        unsafe private void FillVerticesNormalOnly(FLVER0.Mesh mesh, Span<Vector3> pickingVerts, IntPtr vertBuffer)
        {
            Span<FlverLayoutSky> verts = new Span<FlverLayoutSky>(vertBuffer.ToPointer(), mesh.Vertices.Count);
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vert = mesh.Vertices[i];

                verts[i] = new FlverLayoutSky();
                pickingVerts[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                fixed (FlverLayoutSky* v = &verts[i])
                {
                    FillVertex(ref (*v).Position, ref vert);
                    FillNormalSNorm8((*v).Normal, ref vert);
                }
            }
        }

        unsafe private void FillVerticesStandard(FLVER2.Mesh mesh, Span<Vector3> pickingVerts, IntPtr vertBuffer)
        {
            Span<FlverLayout> verts = new Span<FlverLayout>(vertBuffer.ToPointer(), mesh.VertexCount);
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vert = mesh.Vertices[i];

                verts[i] = new FlverLayout();
                pickingVerts[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                fixed (FlverLayout* v = &verts[i])
                {
                    FillVertex(ref (*v).Position, ref vert);
                    FillNormalSNorm8((*v).Normal, ref vert);
                    if (vert.UVCount > 0)
                    {
                        FillUVShort((*v).Uv1, ref vert, 0);
                    }
                    else
                    {
                        FillUVShortZero((*v).Uv1);
                    }
                    if (vert.TangentCount > 0)
                    {
                        FillBinormalBitangentSNorm8((*v).Binormal, (*v).Bitangent, ref vert, 0);
                    }
                    else
                    {
                        FillBinormalBitangentSNorm8Zero((*v).Binormal, (*v).Bitangent);
                    }
                }
            }
        }

        unsafe private void FillVerticesStandard(FLVER0.Mesh mesh, Span<Vector3> pickingVerts, IntPtr vertBuffer)
        {
            Span<FlverLayout> verts = new Span<FlverLayout>(vertBuffer.ToPointer(), mesh.Vertices.Count);
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vert = mesh.Vertices[i];

                verts[i] = new FlverLayout();
                pickingVerts[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                fixed (FlverLayout* v = &verts[i])
                {
                    FillVertex(ref (*v).Position, ref vert);
                    FillNormalSNorm8((*v).Normal, ref vert);
                    if (vert.UVCount > 0)
                    {
                        FillUVShort((*v).Uv1, ref vert, 0);
                    }
                    else
                    {
                        FillUVShortZero((*v).Uv1);
                    }
                    if (vert.TangentCount > 0)
                    {
                        FillBinormalBitangentSNorm8((*v).Binormal, (*v).Bitangent, ref vert, 0);
                    }
                    else
                    {
                        FillBinormalBitangentSNorm8Zero((*v).Binormal, (*v).Bitangent);
                    }
                }
            }
        }

        unsafe private void FillVerticesUV2(FLVER2.Mesh mesh, Span<Vector3> pickingVerts, IntPtr vertBuffer)
        {
            Span<FlverLayoutUV2> verts = new Span<FlverLayoutUV2>(vertBuffer.ToPointer(), mesh.VertexCount);
            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vert = mesh.Vertices[i];

                verts[i] = new FlverLayoutUV2();
                pickingVerts[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                fixed (FlverLayoutUV2* v = &verts[i])
                {
                    FillVertex(ref (*v).Position, ref vert);
                    FillNormalSNorm8((*v).Normal, ref vert);
                    FillUVShort((*v).Uv1, ref vert, 0);
                    FillUVShort((*v).Uv2, ref vert, 1);
                    if (vert.TangentCount > 0)
                    {
                        FillBinormalBitangentSNorm8((*v).Binormal, (*v).Bitangent, ref vert, 0);
                    }
                    else
                    {
                        FillBinormalBitangentSNorm8Zero((*v).Binormal, (*v).Bitangent);
                    }
                }
            }
        }

        unsafe private void FillVerticesUV2(FLVER0.Mesh mesh, Span<Vector3> pickingVerts, IntPtr vertBuffer)
        {
            Span<FlverLayoutUV2> verts = new Span<FlverLayoutUV2>(vertBuffer.ToPointer(), mesh.Vertices.Count);
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                var vert = mesh.Vertices[i];

                verts[i] = new FlverLayoutUV2();
                pickingVerts[i] = new Vector3(vert.Position.X, vert.Position.Y, vert.Position.Z);
                fixed (FlverLayoutUV2* v = &verts[i])
                {
                    FillVertex(ref (*v).Position, ref vert);
                    FillNormalSNorm8((*v).Normal, ref vert);
                    FillUVShort((*v).Uv1, ref vert, 0);
                    FillUVShort((*v).Uv2, ref vert, 1);
                    if (vert.TangentCount > 0)
                    {
                        FillBinormalBitangentSNorm8((*v).Binormal, (*v).Bitangent, ref vert, 0);
                    }
                    else
                    {
                        FillBinormalBitangentSNorm8Zero((*v).Binormal, (*v).Bitangent);
                    }
                }
            }
        }

        unsafe private void ProcessMesh(FLVER0.Mesh mesh, FlverSubmesh dest)
        {
            var factory = Scene.Renderer.Factory;

            dest.Material = GPUMaterials[mesh.MaterialIndex];

            //var MeshVertices = VerticesPool.Rent(mesh.VertexCount);
            var vSize = dest.Material.VertexSize;
            var meshVertices = Marshal.AllocHGlobal(mesh.Vertices.Count * (int)vSize);
            dest.PickingVertices = Marshal.AllocHGlobal(mesh.Vertices.Count * sizeof(Vector3));
            var pvhandle = new Span<Vector3>(dest.PickingVertices.ToPointer(), mesh.Vertices.Count);

            if (dest.Material.LayoutType == MeshLayoutType.LayoutSky)
            {
                FillVerticesNormalOnly(mesh, pvhandle, meshVertices);
            }
            else if (dest.Material.LayoutType == MeshLayoutType.LayoutUV2)
            {
                FillVerticesUV2(mesh, pvhandle, meshVertices);
            }
            else
            {
                FillVerticesStandard(mesh, pvhandle, meshVertices);
            }

            dest.VertexCount = mesh.Vertices.Count;

            dest.MeshFacesets = new List<FlverSubmesh.FlverSubmeshFaceSet>();

            bool is32bit = false;//FlverDeS.Version > 0x20005 && mesh.Vertices.Count > 65535;
            int indicesTotal = 0;
            ushort[] fs16 = null;
            int[] fs32 = null;

            int idxoffset = 0;
            if (mesh.VertexIndices.Count != 0)
            {
                var indices = mesh.Triangulate(FlverDeS.Version).ToArray();
                uint buffersize = (uint)indices.Length * (is32bit ? 4u : 2u);

                indicesTotal = indices.Length;
                if (is32bit)
                {
                    fs32 = new int[indicesTotal];
                }
                else
                {
                    fs16 = new ushort[indicesTotal];
                }

                var newFaceSet = new FlverSubmesh.FlverSubmeshFaceSet()
                {
                    BackfaceCulling = true,
                    IsTriangleStrip = false,
                    //IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer)),
                    IndexOffset = idxoffset,

                    IndexCount = indices.Length,
                    Is32Bit = is32bit,
                    PickingIndicesCount = indices.Length,
                    //PickingIndices = Marshal.AllocHGlobal(indices.Length * 4),
                };
                fixed (void* iptr = indices)
                {
                    //Unsafe.CopyBlock(newFaceSet.PickingIndices.ToPointer(), iptr, (uint)indices.Length * 4);
                }

                if (is32bit)
                {
                    for (int i = 0; i < indices.Length; i++)
                    {
                        if (indices[i] == 0xFFFF && indices[i] > mesh.Vertices.Count)
                        {
                            fs32[newFaceSet.IndexOffset + i] = -1;
                        }
                        else
                        {
                            fs32[newFaceSet.IndexOffset + i] = indices[i];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < indices.Length; i++)
                    {
                        if (indices[i] == 0xFFFF && indices[i] > mesh.Vertices.Count)
                        {
                            fs16[newFaceSet.IndexOffset + i] = 0xFFFF;
                        }
                        else
                        {
                            fs16[newFaceSet.IndexOffset + i] = (ushort)indices[i];
                        }
                    }
                }

                dest.MeshFacesets.Add(newFaceSet);
            }

            dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)dest.PickingVertices.ToPointer(), dest.VertexCount, 12, Quaternion.Identity, Vector3.Zero, Vector3.One);

            uint vbuffersize = (uint)mesh.Vertices.Count * (uint)vSize;
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, (uint)indicesTotal * (is32bit ? 4u : 2u), (int)vSize, 4, (h) =>
            {
                h.FillVBuffer(meshVertices, vSize * (uint)mesh.Vertices.Count, () =>
                {
                    Marshal.FreeHGlobal(meshVertices);
                });
                if (is32bit)
                {
                    h.FillIBuffer(fs32);
                }
                else
                {
                    h.FillIBuffer(fs16);
                }
            });

            if (CaptureMaterialLayouts)
            {
                lock (_matLayoutLock)
                {
                    if (!MaterialLayouts.ContainsKey(dest.Material.MaterialName))
                    {
                        MaterialLayouts.Add(dest.Material.MaterialName, Flver.BufferLayouts[mesh.LayoutIndex]);
                    }
                }
            }
        }

        unsafe private void ProcessMesh(FLVER2.Mesh mesh, FlverSubmesh dest)
        {
            var factory = Scene.Renderer.Factory;

            dest.Material = GPUMaterials[mesh.MaterialIndex];

            //var MeshVertices = VerticesPool.Rent(mesh.VertexCount);
            var vSize = dest.Material.VertexSize;
            var meshVertices = Marshal.AllocHGlobal(mesh.VertexCount * (int)vSize);
            dest.PickingVertices = Marshal.AllocHGlobal(mesh.VertexCount * sizeof(Vector3));
            var pvhandle = new Span<Vector3>(dest.PickingVertices.ToPointer(), mesh.VertexCount);

            if (dest.Material.LayoutType == MeshLayoutType.LayoutSky)
            {
                FillVerticesNormalOnly(mesh, pvhandle, meshVertices);
            }
            else if (dest.Material.LayoutType == MeshLayoutType.LayoutUV2)
            {
                FillVerticesUV2(mesh, pvhandle, meshVertices);
            }
            else
            {
                FillVerticesStandard(mesh, pvhandle, meshVertices);
            }

            dest.VertexCount = mesh.VertexCount;

            dest.MeshFacesets = new List<FlverSubmesh.FlverSubmeshFaceSet>();
            var facesets = mesh.FaceSets;
            var fsUploadsPending = facesets.Count();

            bool is32bit = Flver.Header.Version > 0x20005 && mesh.VertexCount > 65535;
            int indicesTotal = 0;
            ushort[] fs16 = null;
            int[] fs32 = null;
            foreach (var faceset in facesets)
            {
                indicesTotal += faceset.Indices.Length;
            }
            if (is32bit)
            {
                fs32 = new int[indicesTotal];
            }
            else
            {
                fs16 = new ushort[indicesTotal];
            }

            int idxoffset = 0;
            foreach (var faceset in facesets)
            {
                if (faceset.Indices.Length == 0)
                    continue;

                //At this point they use 32-bit faceset vertex indices

                uint buffersize = (uint)faceset.IndicesCount * (is32bit ? 4u : 2u);
                var indices = faceset.TriangleStrip ? faceset.Triangulate(true).ToArray() : faceset.Indices.ToArray();
                var newFaceSet = new FlverSubmesh.FlverSubmeshFaceSet()
                {
                    BackfaceCulling = faceset.CullBackfaces,
                    IsTriangleStrip = faceset.TriangleStrip,
                    //IndexBuffer = factory.CreateBuffer(new BufferDescription(buffersize, BufferUsage.IndexBuffer)),
                    IndexOffset = idxoffset,

                    IndexCount = faceset.IndicesCount,
                    Is32Bit = is32bit,
                    PickingIndicesCount = indices.Length,
                    //PickingIndices = Marshal.AllocHGlobal(indices.Length * 4),
                };
                fixed (void* iptr = indices)
                {
                    //Unsafe.CopyBlock(newFaceSet.PickingIndices.ToPointer(), iptr, (uint)indices.Length * 4);
                }
                

                if ((faceset.Flags & FLVER2.FaceSet.FSFlags.LodLevel1) > 0)
                {
                    newFaceSet.LOD = 1;
                    //HasNoLODs = false;
                    newFaceSet.IsMotionBlur = false;
                }
                else if ((faceset.Flags & FLVER2.FaceSet.FSFlags.LodLevel2) > 0)
                {
                    newFaceSet.LOD = 2;
                    //HasNoLODs = false;
                    newFaceSet.IsMotionBlur = false;
                }

                if ((faceset.Flags & FLVER2.FaceSet.FSFlags.MotionBlur) > 0)
                {
                    newFaceSet.IsMotionBlur = true;
                }

                if (is32bit)
                {
                    for (int i = 0; i < faceset.Indices.Length; i++)
                    {
                        if (faceset.Indices[i] == 0xFFFF && faceset.Indices[i] > mesh.Vertices.Length)
                        {
                            fs32[newFaceSet.IndexOffset + i] = -1;
                        }
                        else
                        {
                            fs32[newFaceSet.IndexOffset + i] = faceset.Indices[i];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < faceset.Indices.Length; i++)
                    {
                        if (faceset.Indices[i] == 0xFFFF && faceset.Indices[i] > mesh.Vertices.Length)
                        {
                            fs16[newFaceSet.IndexOffset + i] = 0xFFFF;
                        }
                        else
                        {
                            fs16[newFaceSet.IndexOffset + i] = (ushort)faceset.Indices[i];
                        }
                    }
                }

                dest.MeshFacesets.Add(newFaceSet);
                idxoffset += faceset.Indices.Length;
            }

            dest.Bounds = BoundingBox.CreateFromPoints((Vector3*)dest.PickingVertices.ToPointer(), dest.VertexCount, 12, Quaternion.Identity, Vector3.Zero, Vector3.One);

            uint vbuffersize = (uint)mesh.VertexCount * (uint)vSize;
            dest.GeomBuffer = Scene.Renderer.GeometryBufferAllocator.Allocate(vbuffersize, (uint)indicesTotal * (is32bit ? 4u : 2u), (int)vSize, 4, (h) =>
            {
                h.FillVBuffer(meshVertices, vSize * (uint)mesh.VertexCount, () =>
                {
                    Marshal.FreeHGlobal(meshVertices);
                });
                if (is32bit)
                {
                    h.FillIBuffer(fs32);
                }
                else
                {
                    h.FillIBuffer(fs16);
                }
            });

            facesets = null;

            if (CaptureMaterialLayouts)
            {
                lock (_matLayoutLock)
                {
                    if (!MaterialLayouts.ContainsKey(dest.Material.MaterialName))
                    {
                        MaterialLayouts.Add(dest.Material.MaterialName, Flver.BufferLayouts[mesh.VertexBuffers[0].LayoutIndex]);
                    }
                }
            }

            Marshal.FreeHGlobal(dest.PickingVertices);
        }

        private bool LoadInternalDeS(AccessLevel al, GameType type)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                GPUMeshes = new FlverSubmesh[FlverDeS.Meshes.Count()];
                GPUMaterials = new FlverMaterial[FlverDeS.Materials.Count()];
                Bounds = new BoundingBox();

                for (int i = 0; i < FlverDeS.Materials.Count(); i++)
                {
                    GPUMaterials[i] = new FlverMaterial();
                    ProcessMaterial(FlverDeS.Materials[i], GPUMaterials[i], type);
                }

                for (int i = 0; i < FlverDeS.Meshes.Count(); i++)
                {
                    GPUMeshes[i] = new FlverSubmesh();
                    ProcessMesh(FlverDeS.Meshes[i], GPUMeshes[i]);
                    if (i == 0)
                    {
                        Bounds = GPUMeshes[i].Bounds;
                    }
                    else
                    {
                        Bounds = BoundingBox.Combine(Bounds, GPUMeshes[i].Bounds);
                    }
                }

                Bones = FlverDeS.Bones;
            }

            if (al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Flver = null;
            }
            //return false;
            return true;
        }

        private bool LoadInternal(AccessLevel al, GameType type)
        {
            if (al == AccessLevel.AccessFull || al == AccessLevel.AccessGPUOptimizedOnly)
            {
                GPUMeshes = new FlverSubmesh[Flver.Meshes.Count()];
                GPUMaterials = new FlverMaterial[Flver.Materials.Count()];
                Bounds = new BoundingBox();

                for (int i = 0; i < Flver.Materials.Count(); i++)
                {
                    GPUMaterials[i] = new FlverMaterial();
                    ProcessMaterial(Flver.Materials[i], GPUMaterials[i], type);
                }

                for (int i = 0; i < Flver.Meshes.Count(); i++)
                {
                    GPUMeshes[i] = new FlverSubmesh();
                    ProcessMesh(Flver.Meshes[i], GPUMeshes[i]);
                    if (i == 0)
                    {
                        Bounds = GPUMeshes[i].Bounds;
                    }
                    else
                    {
                        Bounds = BoundingBox.Combine(Bounds, GPUMeshes[i].Bounds);
                    }
                }

                Bones = Flver.Bones;
            }

            if (al == AccessLevel.AccessGPUOptimizedOnly)
            {
                Flver = null;
            }
            //return false;
            return true;
        }

        bool IResource._Load(byte[] bytes, AccessLevel al, GameType type)
        {
            bool ret;
            if (type == GameType.DemonsSouls)
            {
                FlverDeS = FLVER0.Read(bytes);
                ret = LoadInternalDeS(al, type);
            }
            else
            {
                var cache = (al == AccessLevel.AccessGPUOptimizedOnly) ? GetCache() : null;
                Flver = FLVER2.Read(bytes, cache);
                ret = LoadInternal(al, type);
                ReleaseCache(cache);
            }
            return ret;
        }

        bool IResource._Load(string file, AccessLevel al, GameType type)
        {
            bool ret;
            if (type == GameType.DemonsSouls)
            {
                FlverDeS = FLVER0.Read(file);
                ret = LoadInternalDeS(al, type);
            }
            else
            {
                var cache = (al == AccessLevel.AccessGPUOptimizedOnly) ? GetCache() : null;
                Flver = FLVER2.Read(file, cache);
                ret = LoadInternal(al, type);
                ReleaseCache(cache);
            }
            return ret;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                if (GPUMeshes != null)
                {
                    foreach (var m in GPUMeshes)
                    {
                        m.GeomBuffer.Dispose();
                        //Marshal.FreeHGlobal(m.PickingVertices);
                    }
                }

                if (GPUMaterials != null)
                {
                    foreach (var m in GPUMaterials)
                    {
                        m.ReleaseTextures();
                    }
                }

                disposedValue = true;
            }
        }

        ~FlverResource()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
