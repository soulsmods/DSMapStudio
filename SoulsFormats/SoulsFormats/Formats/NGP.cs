using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// A navmesh format used in DS2. Pretty much only the mesh shape itself is being read right now. Extension: .ngp
    /// </summary>
    public class NGP : SoulsFile<NGP>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public bool BigEndian { get; set; }

        public short Version { get; set; }

        public int Unk1C { get; set; }

        public List<Mesh> Meshes { get; set; }

        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            string magic = br.GetASCII(0, 4);
            return magic == "NVG2";
        }

        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            BigEndian = br.GetInt16(4) == 0x100;
            br.BigEndian = BigEndian;

            br.AssertASCII("NVG2");
            Version = br.AssertInt16(1, 2);
            br.AssertInt16(0);
            int meshCount = br.ReadInt32();
            int count0C = br.ReadInt32();
            int count10 = br.ReadInt32();
            int count14 = br.ReadInt32();
            int count18 = br.ReadInt32();
            Unk1C = br.ReadInt32();

            br.VarintLong = Version == 2;
            long offset20 = br.ReadVarint();
            long offset28 = br.ReadVarint();
            long offset30 = br.ReadVarint();
            long offset38 = br.ReadVarint();
            long[] meshOffsets = br.ReadVarints(meshCount);

            Meshes = new List<Mesh>(meshCount);
            for (int i = 0; i < meshCount; i++)
            {
                br.Position = meshOffsets[i];
                Meshes.Add(new Mesh(br, Version));
            }
        }

        public class Mesh
        {
            public int Unk00 { get; set; }

            public int Unk08 { get; set; }

            public Vector3 BoundingBoxMin { get; set; }

            public Vector3 BoundingBoxMax { get; set; }

            public List<Vector3> Vertices { get; set; }

            public List<Face> Faces { get; set; }

            internal Mesh(BinaryReaderEx br, short version)
            {
                Unk00 = br.ReadInt32();
                br.ReadInt32(); // Length of this mesh (including child structs)
                Unk08 = br.ReadInt32();
                if (version == 2)
                    br.AssertInt32(0);
                BoundingBoxMin = br.ReadVector3();
                BoundingBoxMax = br.ReadVector3();
                int vertexCount = br.ReadInt32();
                short faceCount = br.ReadInt16();
                short count30 = br.ReadInt16();
                short unk30 = br.ReadInt16();
                short unk32 = br.ReadInt16();
                br.AssertByte(1);
                br.AssertByte(0);
                br.AssertByte(0);
                br.AssertByte(0);
                if (version == 2)
                    br.AssertInt64(0);
                long verticesOffset = br.ReadVarint();
                long offset48 = br.ReadVarint();
                long facesOffset = br.ReadVarint();
                long offset58 = br.ReadVarint();
                long offset60 = br.ReadVarint();
                long offset68 = br.ReadVarint();

                br.Position = verticesOffset;
                Vertices = new List<Vector3>(vertexCount);
                for (int i = 0; i < vertexCount; i++)
                    Vertices.Add(br.ReadVector3());

                br.Position = facesOffset;
                Faces = new List<Face>(faceCount);
                for (int i = 0; i < faceCount; i++)
                    Faces.Add(new Face(br));
            }
        }

        public class Face
        {
            public short V1 { get; set; }

            public short V2 { get; set; }

            public short V3 { get; set; }

            public short Unk06 { get; set; }

            public short Unk08 { get; set; }

            public short Unk0A { get; set; }

            internal Face(BinaryReaderEx br)
            {
                V1 = br.ReadInt16();
                V2 = br.ReadInt16();
                V3 = br.ReadInt16();
                Unk06 = br.ReadInt16();
                Unk08 = br.ReadInt16();
                Unk0A = br.ReadInt16();
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
