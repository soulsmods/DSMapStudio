﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A model format used since Dark Souls 1. Extension: .flv, .flver
    /// </summary>
    public partial class FLVER2 : SoulsFile<FLVER2>, IFlver
    {
        /// <summary>
        /// General values for this model.
        /// </summary>
        public FLVER2Header Header { get; set; }

        /// <summary>
        /// Dummy polygons in this model.
        /// </summary>
        public List<FLVER.Dummy> Dummies { get; set; }
        IReadOnlyList<FLVER.Dummy> IFlver.Dummies => Dummies;

        /// <summary>
        /// Materials in this model, usually one per mesh.
        /// </summary>
        public List<Material> Materials { get; set; }
        IReadOnlyList<IFlverMaterial> IFlver.Materials => Materials;

        /// <summary>
        /// Lists of GX elements referenced by materials in DS2 and beyond.
        /// </summary>
        public List<GXList> GXLists { get; set; }

        /// <summary>
        /// Bones used by this model, may or may not be the full skeleton.
        /// </summary>
        public List<FLVER.Bone> Bones { get; set; }
        IReadOnlyList<FLVER.Bone> IFlver.Bones => Bones;

        /// <summary>
        /// Individual chunks of the model.
        /// </summary>
        public List<Mesh> Meshes { get; set; }
        IReadOnlyList<IFlverMesh> IFlver.Meshes => Meshes;

        /// <summary>
        /// Layouts determining how to write vertex information.
        /// </summary>
        public List<BufferLayout> BufferLayouts { get; set; }

        /// <summary>
        /// Unknown; only present in Sekiro.
        /// </summary>
        public SekiroUnkStruct SekiroUnk { get; set; }

        private FlverCache Cache = null;

        /// <summary>
        /// Creates a FLVER with a default header and empty lists.
        /// </summary>
        public FLVER2()
        {
            Header = new FLVER2Header();
            Dummies = new List<FLVER.Dummy>();
            Materials = new List<Material>();
            GXLists = new List<GXList>();
            Bones = new List<FLVER.Bone>();
            Meshes = new List<Mesh>();
            BufferLayouts = new List<BufferLayout>();
        }

        /// <summary>
        /// Creates a FLVER with a preset cache
        /// </summary>
        public static FLVER2 Read(byte[] bytes, FlverCache cache)
        {
            BinaryReaderEx br = new BinaryReaderEx(false, bytes);
            FLVER2 file = new FLVER2();
            file.Cache = cache;
            DCX.Type ctype;
            br = SFUtil.GetDecompressedBR(br, out ctype);
            file.Compression = ctype;
            file.Read(br);
            return file;
        }

        /// <summary>
        /// Loads a file from the specified path, automatically decompressing it if necessary.
        /// </summary>
        public static FLVER2 Read(string path, FlverCache cache)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                BinaryReaderEx br = new BinaryReaderEx(false, stream);
                FLVER2 file = new FLVER2();
                file.Cache = cache;
                DCX.Type ctype;
                br = SFUtil.GetDecompressedBR(br, out ctype);
                file.Compression = ctype;
                file.Read(br);
                return file;
            }
        }

        /// <summary>
        /// Returns true if the data appears to be a FLVER.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 0xC)
                return false;

            string magic = br.GetASCII(0, 6);
            string endian = br.GetASCII(6, 2);
            br.BigEndian = endian == "B\0";
            int version = br.GetInt32(8);
            return magic == "FLVER\0" && version >= 0x20000;
        }

        /// <summary>
        /// Reads FLVER data from a BinaryReaderEx.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            if (Cache == null)
            {
                Cache = new FlverCache();
            }

            br.BigEndian = false;

            Header = new FLVER2Header();
            br.AssertASCII("FLVER\0");
            Header.BigEndian = br.AssertASCII("L\0", "B\0") == "B\0";
            br.BigEndian = Header.BigEndian;

            // Gundam Unicorn: 0x20005, 0x2000E
            // ACVD: 
            // DS1: 2000B (PS3 o0700/1), 2000C, 2000D
            // DS2 NT: 2000F, 20010
            // DS2: 20010, 20009 (armor 9320)
            // SFS: 20010
            // BB:  20013, 20014
            // DS3: 20013, 20014
            // SDT: 2001A, 20016 (test chr)
            Header.Version = br.AssertInt32(0x20005, 0x20007, 0x20009, 0x2000B, 0x2000C, 0x2000D, 0x2000E, 0x2000F, 0x20010, 0x20013, 0x20014, 0x20016, 0x2001A);

            int dataOffset = br.ReadInt32();
            br.ReadInt32(); // Data length
            int dummyCount = br.ReadInt32();
            int materialCount = br.ReadInt32();
            int boneCount = br.ReadInt32();
            int meshCount = br.ReadInt32();
            int vertexBufferCount = br.ReadInt32();

            Header.BoundingBoxMin = br.ReadVector3();
            Header.BoundingBoxMax = br.ReadVector3();

            br.ReadInt32(); // Face count not including motion blur meshes or degenerate faces
            br.ReadInt32(); // Total face count

            int vertexIndicesSize = br.AssertByte(0, 8, 16, 32);
            Header.Unicode = br.ReadBoolean();
            Header.Unk4A = br.ReadBoolean();
            br.AssertByte(0);

            Header.Unk4C = br.ReadInt32();

            int faceSetCount = br.ReadInt32();
            int bufferLayoutCount = br.ReadInt32();
            int textureCount = br.ReadInt32();

            Header.Unk5C = br.ReadByte();
            Header.Unk5D = br.ReadByte();
            br.AssertByte(0);
            br.AssertByte(0);

            br.AssertInt32(0);
            br.AssertInt32(0);
            Header.Unk68 = br.AssertInt32(0, 1, 2, 3, 4);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);

            Dummies = new List<FLVER.Dummy>(dummyCount);
            for (int i = 0; i < dummyCount; i++)
                Dummies.Add(new FLVER.Dummy(br, Header.Version));

            Materials = new List<Material>(materialCount);
            var gxListIndices = new Dictionary<int, int>();
            GXLists = new List<GXList>();
            for (int i = 0; i < materialCount; i++)
                Materials.Add(new Material(br, Header, GXLists, gxListIndices));

            Bones = new List<FLVER.Bone>(boneCount);
            for (int i = 0; i < boneCount; i++)
                Bones.Add(new FLVER.Bone(br, Header.Unicode));

            Meshes = new List<Mesh>(meshCount);
            for (int i = 0; i < meshCount; i++)
                Meshes.Add(new Mesh(br, Header));

            var faceSets = new List<FaceSet>(faceSetCount);
            for (int i = 0; i < faceSetCount; i++)
                faceSets.Add(new FaceSet(br, Header, Cache, vertexIndicesSize, dataOffset));

            var vertexBuffers = new List<VertexBuffer>(vertexBufferCount);
            for (int i = 0; i < vertexBufferCount; i++)
                vertexBuffers.Add(new VertexBuffer(br));

            BufferLayouts = new List<BufferLayout>(bufferLayoutCount);
            for (int i = 0; i < bufferLayoutCount; i++)
                BufferLayouts.Add(new BufferLayout(br));

            var textures = new List<Texture>(textureCount);
            for (int i = 0; i < textureCount; i++)
                textures.Add(new Texture(br, Header));

            if (Header.Version >= 0x2001A)
                SekiroUnk = new SekiroUnkStruct(br);

            Dictionary<int, Texture> textureDict = SFUtil.Dictionize(textures);
            foreach (Material material in Materials)
            {
                material.TakeTextures(textureDict);
            }
            if (textureDict.Count != 0)
                throw new NotSupportedException("Orphaned textures found.");

            Dictionary<int, FaceSet> faceSetDict = SFUtil.Dictionize(faceSets);
            Dictionary<int, VertexBuffer> vertexBufferDict = SFUtil.Dictionize(vertexBuffers);
            foreach (Mesh mesh in Meshes)
            {
                mesh.TakeFaceSets(faceSetDict);
                mesh.TakeVertexBuffers(vertexBufferDict, BufferLayouts);
                mesh.ReadVertices(br, dataOffset, BufferLayouts, Header, Cache);
            }
            if (faceSetDict.Count != 0)
                throw new NotSupportedException("Orphaned face sets found.");
            if (vertexBufferDict.Count != 0)
                throw new NotSupportedException("Orphaned vertex buffers found.");
        }

        /// <summary>
        /// Writes FLVER data to a BinaryWriterEx.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = Header.BigEndian;
            bw.WriteASCII("FLVER\0");
            bw.WriteASCII(Header.BigEndian ? "B\0" : "L\0");
            bw.WriteInt32(Header.Version);

            bw.ReserveInt32("DataOffset");
            bw.ReserveInt32("DataSize");
            bw.WriteInt32(Dummies.Count);
            bw.WriteInt32(Materials.Count);
            bw.WriteInt32(Bones.Count);
            bw.WriteInt32(Meshes.Count);
            bw.WriteInt32(Meshes.Sum(m => m.VertexBuffers.Count));
            bw.WriteVector3(Header.BoundingBoxMin);
            bw.WriteVector3(Header.BoundingBoxMax);

            int trueFaceCount = 0;
            int totalFaceCount = 0;
            foreach (Mesh mesh in Meshes)
            {
                bool allowPrimitiveRestarts = mesh.Vertices.Length < ushort.MaxValue;
                foreach (FaceSet faceSet in mesh.FaceSets)
                {
                    faceSet.AddFaceCounts(allowPrimitiveRestarts, ref trueFaceCount, ref totalFaceCount);
                }
            }
            bw.WriteInt32(trueFaceCount);
            bw.WriteInt32(totalFaceCount);

            byte vertexIndicesSize = 0;
            if (Header.Version < 0x20013)
            {
                vertexIndicesSize = 16;
                foreach (Mesh mesh in Meshes)
                {
                    foreach (FaceSet fs in mesh.FaceSets)
                    {
                        vertexIndicesSize = (byte)Math.Max(vertexIndicesSize, fs.GetVertexIndexSize());
                    }
                }
            }

            bw.WriteByte(vertexIndicesSize);
            bw.WriteBoolean(Header.Unicode);
            bw.WriteBoolean(Header.Unk4A);
            bw.WriteByte(0);

            bw.WriteInt32(Header.Unk4C);

            bw.WriteInt32(Meshes.Sum(m => m.FaceSets.Count));
            bw.WriteInt32(BufferLayouts.Count);
            bw.WriteInt32(Materials.Sum(m => m.Textures.Count));

            bw.WriteByte(Header.Unk5C);
            bw.WriteByte(Header.Unk5D);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(Header.Unk68);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);

            foreach (FLVER.Dummy dummy in Dummies)
                dummy.Write(bw, Header.Version);

            for (int i = 0; i < Materials.Count; i++)
                Materials[i].Write(bw, i);

            for (int i = 0; i < Bones.Count; i++)
                Bones[i].Write(bw, i);

            for (int i = 0; i < Meshes.Count; i++)
                Meshes[i].Write(bw, i);

            int faceSetIndex = 0;
            foreach (Mesh mesh in Meshes)
            {
                for (int i = 0; i < mesh.FaceSets.Count; i++)
                {
                    int indexSize = vertexIndicesSize;
                    if (indexSize == 0)
                        indexSize = mesh.FaceSets[i].GetVertexIndexSize();

                    mesh.FaceSets[i].Write(bw, Header, indexSize, faceSetIndex + i);
                }
                faceSetIndex += mesh.FaceSets.Count;
            }

            int vertexBufferIndex = 0;
            foreach (Mesh mesh in Meshes)
            {
                for (int i = 0; i < mesh.VertexBuffers.Count; i++)
                    mesh.VertexBuffers[i].Write(bw, Header, vertexBufferIndex + i, i, BufferLayouts, mesh.Vertices.Length);
                vertexBufferIndex += mesh.VertexBuffers.Count;
            }

            for (int i = 0; i < BufferLayouts.Count; i++)
                BufferLayouts[i].Write(bw, i);

            int textureIndex = 0;
            for (int i = 0; i < Materials.Count; i++)
            {
                Materials[i].WriteTextures(bw, i, textureIndex);
                textureIndex += Materials[i].Textures.Count;
            }

            if (Header.Version >= 0x2001A)
                SekiroUnk.Write(bw);

            bw.Pad(0x10);
            for (int i = 0; i < BufferLayouts.Count; i++)
                BufferLayouts[i].WriteMembers(bw, i);

            bw.Pad(0x10);
            for (int i = 0; i < Meshes.Count; i++)
                Meshes[i].WriteBoundingBox(bw, i, Header);

            bw.Pad(0x10);
            int boneIndicesStart = (int)bw.Position;
            for (int i = 0; i < Meshes.Count; i++)
                Meshes[i].WriteBoneIndices(bw, i, boneIndicesStart);

            bw.Pad(0x10);
            faceSetIndex = 0;
            for (int i = 0; i < Meshes.Count; i++)
            {
                bw.FillInt32($"MeshFaceSetIndices{i}", (int)bw.Position);
                for (int j = 0; j < Meshes[i].FaceSets.Count; j++)
                    bw.WriteInt32(faceSetIndex + j);
                faceSetIndex += Meshes[i].FaceSets.Count;
            }

            bw.Pad(0x10);
            vertexBufferIndex = 0;
            for (int i = 0; i < Meshes.Count; i++)
            {
                bw.FillInt32($"MeshVertexBufferIndices{i}", (int)bw.Position);
                for (int j = 0; j < Meshes[i].VertexBuffers.Count; j++)
                    bw.WriteInt32(vertexBufferIndex + j);
                vertexBufferIndex += Meshes[i].VertexBuffers.Count;
            }

            bw.Pad(0x10);
            var gxOffsets = new List<int>();
            foreach (GXList gxList in GXLists)
            {
                gxOffsets.Add((int)bw.Position);
                gxList.Write(bw, Header);
            }
            for (int i = 0; i < Materials.Count; i++)
            {
                Materials[i].FillGXOffset(bw, i, gxOffsets);
            }

            bw.Pad(0x10);
            textureIndex = 0;
            for (int i = 0; i < Materials.Count; i++)
            {
                Material material = Materials[i];
                material.WriteStrings(bw, Header, i);

                for (int j = 0; j < material.Textures.Count; j++)
                    material.Textures[j].WriteStrings(bw, Header, textureIndex + j);
                textureIndex += material.Textures.Count;
            }

            bw.Pad(0x10);
            for (int i = 0; i < Bones.Count; i++)
                Bones[i].WriteStrings(bw, Header.Unicode, i);

            int alignment = Header.Version <= 0x2000E ? 0x20 : 0x10;
            bw.Pad(alignment);
            if (Header.Version == 0x2000F || Header.Version == 0x20010)
                bw.Pad(0x20);

            int dataStart = (int)bw.Position;
            bw.FillInt32("DataOffset", dataStart);

            faceSetIndex = 0;
            vertexBufferIndex = 0;
            for (int i = 0; i < Meshes.Count; i++)
            {
                Mesh mesh = Meshes[i];
                for (int j = 0; j < mesh.FaceSets.Count; j++)
                {
                    int indexSize = vertexIndicesSize;
                    if (indexSize == 0)
                        indexSize = mesh.FaceSets[j].GetVertexIndexSize();

                    bw.Pad(alignment);
                    mesh.FaceSets[j].WriteVertices(bw, indexSize, faceSetIndex + j, dataStart);
                }
                faceSetIndex += mesh.FaceSets.Count;

                foreach (FLVER.Vertex vertex in mesh.Vertices)
                    vertex.PrepareWrite();

                for (int j = 0; j < mesh.VertexBuffers.Count; j++)
                {
                    bw.Pad(alignment);
                    mesh.VertexBuffers[j].WriteBuffer(bw, vertexBufferIndex + j, BufferLayouts, mesh.Vertices, dataStart, Header);
                }

                foreach (FLVER.Vertex vertex in mesh.Vertices)
                    vertex.FinishWrite();

                vertexBufferIndex += mesh.VertexBuffers.Count;
            }

            bw.Pad(alignment);
            bw.FillInt32("DataSize", (int)bw.Position - dataStart);
            if (Header.Version == 0x2000F || Header.Version == 0x20010)
                bw.Pad(0x20);
        }
    }
}
