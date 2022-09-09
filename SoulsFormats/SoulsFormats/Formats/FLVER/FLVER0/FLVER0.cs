using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// 3D models from Armored Core: For Answer to Another Century's Episode R. Extension: .flv, .flver
    /// </summary>
    public partial class FLVER0 : SoulsFile<FLVER0>, IFlver
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool BigEndian { get; set; }

        public int Version { get; set; }

        public Vector3 BoundingBoxMin { get; set; }

        public Vector3 BoundingBoxMax { get; set; }

        public byte VertexIndexSize { get; set; }

        public bool Unicode { get; set; }

        public byte Unk4A { get; set; }

        public byte Unk4B { get; set; }

        public int Unk4C { get; set; }

        public int Unk5C { get; set; }

        public List<FLVER.Dummy> Dummies { get; set; }
        IReadOnlyList<FLVER.Dummy> IFlver.Dummies => Dummies;

        public List<Material> Materials { get; set; }
        IReadOnlyList<IFlverMaterial> IFlver.Materials => Materials;

        public List<FLVER.Bone> Bones { get; set; }
        IReadOnlyList<FLVER.Bone> IFlver.Bones => Bones;

        public List<Mesh> Meshes { get; set; }
        IReadOnlyList<IFlverMesh> IFlver.Meshes => Meshes;

        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 0xC)
                return false;

            string magic = br.ReadASCII(6);
            string endian = br.ReadASCII(2);
            if (endian == "L\0")
                br.BigEndian = false;
            else if (endian == "B\0")
                br.BigEndian = true;
            int version = br.ReadInt32();
            return magic == "FLVER\0" && version >= 0x00000 && version < 0x20000;
        }

        protected override void Read(BinaryReaderEx br)
        {
            br.AssertASCII("FLVER\0");
            BigEndian = br.AssertASCII("L\0", "B\0") == "B\0";
            br.BigEndian = BigEndian;

            // 10002, 10003 - Another Century's Episode R
            Version = br.AssertInt32(0x0E, 0x0F, 0x10, 0x12, 0x13, 0x14, 0x15,
                0x10002, 0x10003);
            int dataOffset = br.ReadInt32();
            br.ReadInt32(); // Data length
            int dummyCount = br.ReadInt32();
            int materialCount = br.ReadInt32();
            int boneCount = br.ReadInt32();
            int meshCount = br.ReadInt32();
            br.ReadInt32(); // Vertex buffer count
            BoundingBoxMin = br.ReadVector3();
            BoundingBoxMax = br.ReadVector3();
            br.ReadInt32(); // Face count not including motion blur meshes or degenerate faces
            br.ReadInt32(); // Total face count
            VertexIndexSize = br.AssertByte(16, 32);
            Unicode = br.ReadBoolean();
            Unk4A = br.ReadByte();
            Unk4B = br.ReadByte();
            Unk4C = br.ReadInt32();
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            Unk5C = br.ReadByte();
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertPattern(0x20, 0x00);

            Dummies = new List<FLVER.Dummy>(dummyCount);
            for (int i = 0; i < dummyCount; i++)
                Dummies.Add(new FLVER.Dummy(br, Version));

            Materials = new List<Material>(materialCount);
            for (int i = 0; i < materialCount; i++)
                Materials.Add(new Material(br, this));

            Bones = new List<FLVER.Bone>(boneCount);
            for (int i = 0; i < boneCount; i++)
                Bones.Add(new FLVER.Bone(br, Unicode));

            Meshes = new List<Mesh>(meshCount);
            for (int i = 0; i < meshCount; i++)
                Meshes.Add(new Mesh(br, this, dataOffset));
        }

        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = BigEndian;
            bw.WriteASCII("FLVER\0");
            bw.WriteASCII(BigEndian ? "B\0" : "L\0");
            bw.WriteInt32(Version);

            bw.ReserveInt32("DataOffset");
            bw.ReserveInt32("DataSize");
            bw.WriteInt32(Dummies.Count);
            bw.WriteInt32(Materials.Count);
            bw.WriteInt32(Bones.Count);
            bw.WriteInt32(Meshes.Count);
            bw.WriteInt32(Meshes.Count); //Vert buffer count. Currently based on reads, there should only be one per mesh
            bw.WriteVector3(BoundingBoxMin);
            bw.WriteVector3(BoundingBoxMax);

            int triCount = 0;
            int indicesCount = 0;
            for (int i = 0; i < Meshes.Count; i++)
            {
                triCount += Meshes[i].GetFaces(Version).Count;
                indicesCount += Meshes[i].VertexIndices.Count;
            }
            bw.WriteInt32(triCount);
            bw.WriteInt32(indicesCount); //Not technically correct, but should be valid for the buffer size

            byte vertexIndicesSize = 16;
            foreach (Mesh mesh in Meshes)
            {
                vertexIndicesSize = (byte)Math.Max(vertexIndicesSize, mesh.GetVertexIndexSize());
            }

            bw.WriteByte(vertexIndicesSize);
            bw.WriteBoolean(Unicode);
            bw.WriteBoolean(Unk4A > 0);
            bw.WriteByte(0);

            bw.WriteInt32(Unk4C);

            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteByte((byte)Unk5C);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteBytes(new byte[0x20]);

            foreach (FLVER.Dummy dummy in Dummies)
                dummy.Write(bw, Version);

            for (int i = 0; i < Materials.Count; i++)
                Materials[i].Write(bw, i);

            for (int i = 0; i < Bones.Count; i++)
                Bones[i].Write(bw, i);

            for (int i = 0; i < Meshes.Count; i++)
                Meshes[i].Write(bw, this, i);

            for (int i = 0; i < Materials.Count; i++)
                Materials[i].WriteSubStructs(bw, Unicode, i);

            for (int i = 0; i < Bones.Count; i++)
                Bones[i].WriteStrings(bw, Unicode, i);

            for (int i = 0; i < Meshes.Count; i++)
                Meshes[i].WriteVertexBufferHeader(bw, this, i);

            bw.Pad(0x20);
            int dataOffset = (int)bw.Position;
            bw.FillInt32("DataOffset", dataOffset);

            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].WriteVertexIndices(bw, this, dataOffset, i);
                bw.Pad(0x20);
                Meshes[i].WriteVertexBufferData(bw, this, dataOffset, i);
                bw.Pad(0x20);
            }

            bw.FillInt32("DataSize", (int)bw.Position - dataOffset);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
