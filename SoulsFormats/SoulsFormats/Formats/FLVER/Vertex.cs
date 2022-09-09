﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// Common classes for FLVER0 and FLVER2.
    /// </summary>
    public static partial class FLVER
    {
        /// <summary>
        /// A single point in a mesh.
        /// </summary>
        public unsafe struct Vertex
        {
            /// <summary>
            /// Where the vertex is.
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Weight of the vertex's attachment to bones; must be 4 length.
            /// </summary>
            public VertexBoneWeights BoneWeights;

            /// <summary>
            /// Whether the vertex uses the BoneWeights struct.
            /// </summary>
            public bool UsesBoneWeights;

            /// <summary>
            /// Bones the vertex is weighted to, indexing the parent mesh's bone indices; must be 4 length.
            /// </summary>
            public VertexBoneIndices BoneIndices;

            /// <summary>
            /// Whether the vertex uses the BoneIndices struct.
            /// </summary>
            public bool UsesBoneIndices;

            /// <summary>
            /// Vector pointing away from the surface.
            /// </summary>
            public Vector3 Normal;

            /// <summary>
            /// Fourth component of the normal, read without transforming into a float; used as a bone index for binding to a single bone.
            /// </summary>
            public int NormalW;

            /// <summary>
            /// Texture coordinates of the vertex.
            /// </summary>
            //public List<Vector3> UVs;
            public byte UVCount;
            private fixed float UVs[60];

            /// <summary>
            /// Vector pointing perpendicular to the normal.
            /// </summary>
            //public List<Vector4> Tangents;
            public byte TangentCount;
            private fixed float Tangents[60];

            /// <summary>
            /// Vector pointing perpendicular to the normal and tangent.
            /// </summary>
            public Vector4 Bitangent;

            /// <summary>
            /// Data used for alpha, blending, etc.
            /// </summary>
            public List<VertexColor> Colors;



            private Queue<Vector3> uvQueue;
            private Queue<Vector4> tangentQueue;
            private Queue<VertexColor> colorQueue;

            public Vector3 GetUV(byte index)
            {
                if (index > UVCount)
                {
                    throw new ArgumentOutOfRangeException("Index is too big");
                }
                return new Vector3(UVs[index * 3], UVs[index * 3 + 1], UVs[index * 3 + 2]);
            }

            public void AddUV(Vector3 uv)
            {
                if (UVCount >= 20)
                {
                    throw new Exception("Increase UV count");
                }
                UVs[UVCount * 3] = uv.X;
                UVs[UVCount * 3 + 1] = uv.Y;
                UVs[UVCount * 3 + 2] = uv.Z;
                UVCount++;
            }

            public Vector4 GetTangent(byte index)
            {
                if (index > TangentCount)
                {
                    throw new ArgumentOutOfRangeException("Index is too big");
                }
                return new Vector4(Tangents[index * 4], Tangents[index * 4 + 1], Tangents[index * 4 + 2], Tangents[index * 4 + 3]);
            }

            public void AddTangent(Vector4 tangent)
            {
                if (TangentCount >= 15)
                {
                    throw new Exception("Increase Tangent count");
                }
                Tangents[UVCount * 4] = tangent.X;
                Tangents[UVCount * 4 + 1] = tangent.Y;
                Tangents[UVCount * 4 + 2] = tangent.Z;
                Tangents[UVCount * 4 + 3] = tangent.W;
                TangentCount++;
            }

            /// <summary>
            /// Create a Vertex with null or empty values.
            /// </summary>
            public Vertex(int uvCapacity = 0, int tangentCapacity = 0, int colorCapacity = 0)
            {
                Position = Vector3.Zero;
                BoneWeights = new VertexBoneWeights();
                BoneIndices = new VertexBoneIndices();
                Normal = Vector3.UnitX;
                NormalW = 0;
                Bitangent = Vector4.UnitW;

                uvQueue = null;
                tangentQueue = null;
                colorQueue = null;

                UVCount = 0;
                TangentCount = 0;
                //UVs = new List<Vector3>(uvCapacity);
                //Tangents = new List<Vector4>(tangentCapacity);
                //Colors = new List<VertexColor>(colorCapacity);
                //Tangents = null;
                Colors = null;
                UsesBoneIndices = false;
                UsesBoneWeights = false;
            }

            /// <summary>
            /// Creates a new Vertex with values copied from another.
            /// </summary>
            public Vertex(Vertex clone)
            {
                Position = clone.Position;
                BoneWeights = clone.BoneWeights;
                BoneIndices = clone.BoneIndices;
                Normal = clone.Normal;
                NormalW = clone.NormalW;
                UVCount = clone.UVCount;
                TangentCount = clone.TangentCount;
                //UVs = new List<Vector3>(clone.UVs);
                //Tangents = new List<Vector4>(clone.Tangents);
                Bitangent = clone.Bitangent;
                Colors = new List<VertexColor>(clone.Colors);
                UsesBoneIndices = clone.UsesBoneIndices;
                UsesBoneWeights = clone.UsesBoneWeights;

                uvQueue = null;
                tangentQueue = null;
                colorQueue = null;
            }

            /// <summary>
            /// Must be called before writing any buffers. Queues list types so they will be split across buffers properly.
            /// </summary>
            internal void PrepareWrite()
            {
                //uvQueue = new Queue<Vector3>(UVs);
                //tangentQueue = new Queue<Vector4>(Tangents);
                colorQueue = new Queue<VertexColor>(Colors);
            }

            /// <summary>
            /// Should be called after writing all buffers. Throws out queues to free memory.
            /// </summary>
            internal void FinishWrite()
            {
                uvQueue = null;
                tangentQueue = null;
                colorQueue = null;
            }

            internal void Read(BinaryReaderEx br, List<LayoutMember> layout, float uvFactor)
            {
                foreach (LayoutMember member in layout)
                {
                    if (member.Semantic == LayoutSemantic.Position)
                    {
                        if (member.Type == LayoutType.Float3)
                        {
                            Position = br.ReadVector3();
                        }
                        else if (member.Type == LayoutType.Float4)
                        {
                            Position = br.ReadVector3();
                            br.AssertSingle(0);
                        }
                        else if (member.Type == LayoutType.EdgeCompressed)
                        {
                            // Sit in a corner and cry
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.BoneWeights)
                    {
                        if (member.Type == LayoutType.Byte4A)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneWeights[i] = br.ReadSByte() / 127f;
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneWeights[i] = br.ReadByte() / 255f;
                        }
                        else if (member.Type == LayoutType.UVPair)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneWeights[i] = br.ReadInt16() / 32767f;
                        }
                        else if (member.Type == LayoutType.Short4toFloat4A)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneWeights[i] = br.ReadInt16() / 32767f;
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");

                        UsesBoneWeights = true;
                    }
                    else if (member.Semantic == LayoutSemantic.BoneIndices)
                    {
                        if (member.Type == LayoutType.Byte4B)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneIndices[i] = br.ReadByte();
                        }
                        else if (member.Type == LayoutType.ShortBoneIndices)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneIndices[i] = br.ReadUInt16();
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            for (int i = 0; i < 4; i++)
                                BoneIndices[i] = br.ReadByte();
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");
                            
                        UsesBoneIndices = true;
                    }
                    else if (member.Semantic == LayoutSemantic.Normal)
                    {
                        if (member.Type == LayoutType.Float3)
                        {
                            Normal = br.ReadVector3();
                        }
                        else if (member.Type == LayoutType.Float4)
                        {
                            Normal = br.ReadVector3();
                            float w = br.ReadSingle();
                            NormalW = (int)w;
                            if (w != NormalW)
                                throw new InvalidDataException($"Float4 Normal W was not a whole number: {w}");
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            Normal = ReadByteNormXYZ(br);
                            NormalW = br.ReadByte();
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            Normal = ReadByteNormXYZ(br);
                            NormalW = br.ReadByte();
                        }
                        else if (member.Type == LayoutType.Short2toFloat2)
                        {
                            NormalW = br.ReadByte();
                            Normal = ReadSByteNormZYX(br);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            Normal = ReadByteNormXYZ(br);
                            NormalW = br.ReadByte();
                        }
                        else if (member.Type == LayoutType.Short4toFloat4A)
                        {
                            Normal = ReadShortNormXYZ(br);
                            NormalW = br.ReadInt16();
                        }
                        else if (member.Type == LayoutType.Short4toFloat4B)
                        {
                            //Normal = ReadUShortNormXYZ(br);
                            Normal = ReadFloat16NormXYZ(br);
                            NormalW = br.ReadInt16();
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            Normal = ReadByteNormXYZ(br);
                            NormalW = br.ReadByte();
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.UV)
                    {
                        if (member.Type == LayoutType.Float2)
                        {
                            AddUV(new Vector3(br.ReadVector2(), 0));
                        }
                        else if (member.Type == LayoutType.Float3)
                        {
                            AddUV(br.ReadVector3());
                        }
                        else if (member.Type == LayoutType.Float4)
                        {
                            AddUV(new Vector3(br.ReadVector2(), 0));
                            AddUV(new Vector3(br.ReadVector2(), 0));
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                        }
                        else if (member.Type == LayoutType.Short2toFloat2)
                        {
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                        }
                        else if (member.Type == LayoutType.UV)
                        {
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                        }
                        else if (member.Type == LayoutType.UVPair)
                        {
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                            AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), 0) / uvFactor);
                        }
                        else if (member.Type == LayoutType.Short4toFloat4B)
                        {
                            //AddUV(new Vector3(br.ReadInt16(), br.ReadInt16(), br.ReadInt16()) / uvFactor);
                            AddUV(ReadFloat16NormXYZ(br));
                            br.AssertInt16(0);
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.Tangent)
                    {
                        if (member.Type == LayoutType.Float4)
                        {
                            //Tangents.Add(br.ReadVector4());
                            AddTangent(br.ReadVector4());
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            //Tangents.Add(ReadByteNormXYZW(br));
                            AddTangent(ReadByteNormXYZW(br));
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            //Tangents.Add(ReadByteNormXYZW(br));
                            AddTangent(ReadByteNormXYZW(br));
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            //Tangents.Add(ReadByteNormXYZW(br));
                            AddTangent(ReadByteNormXYZW(br));
                        }
                        else if (member.Type == LayoutType.Short4toFloat4A)
                        {
                            //Tangents.Add(ReadShortNormXYZW(br));
                            AddTangent(ReadShortNormXYZW(br));
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            //Tangents.Add(ReadByteNormXYZW(br));
                            AddTangent(ReadByteNormXYZW(br));
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");

                    }
                    else if (member.Semantic == LayoutSemantic.Bitangent)
                    {
                        if (member.Type == LayoutType.Byte4A)
                        {
                            Bitangent = ReadByteNormXYZW(br);
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            Bitangent = ReadByteNormXYZW(br);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            Bitangent = ReadByteNormXYZW(br);
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            Bitangent = ReadByteNormXYZW(br);
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.VertexColor)
                    {
                        if (member.Type == LayoutType.Float4)
                        {
                            //Colors.Add(VertexColor.ReadFloatRGBA(br));
                            VertexColor.ReadFloatRGBA(br);
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            // Definitely RGBA in DeS
                            //Colors.Add(VertexColor.ReadByteRGBA(br));
                            VertexColor.ReadByteRGBA(br);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            // Definitely RGBA in DS1
                            //Colors.Add(VertexColor.ReadByteRGBA(br));
                            VertexColor.ReadByteRGBA(br);
                        }
                        else
                            throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");

                    }
                    else
                        throw new NotImplementedException($"Read not implemented for {member.Type} {member.Semantic}.");
                }
            }

            #region Read Helpers
            public static float ReadByteNorm(BinaryReaderEx br)
                => (br.ReadByte() - 127) / 127f;

            public static Vector3 ReadByteNormXYZ(BinaryReaderEx br)
                => new Vector3(ReadByteNorm(br), ReadByteNorm(br), ReadByteNorm(br));

            public static Vector4 ReadByteNormXYZW(BinaryReaderEx br)
                => new Vector4(ReadByteNorm(br), ReadByteNorm(br), ReadByteNorm(br), ReadByteNorm(br));

            public static float ReadSByteNorm(BinaryReaderEx br)
                => br.ReadSByte() / 127f;

            public static Vector3 ReadSByteNormZYX(BinaryReaderEx br)
            {
                float z = ReadSByteNorm(br);
                float y = ReadSByteNorm(br);
                float x = ReadSByteNorm(br);
                return new Vector3(x, y, z);
            }

            public static float ReadShortNorm(BinaryReaderEx br)
                => br.ReadInt16() / 32767f;

            public static Vector3 ReadShortNormXYZ(BinaryReaderEx br)
                => new Vector3(ReadShortNorm(br), ReadShortNorm(br), ReadShortNorm(br));

            public static Vector4 ReadShortNormXYZW(BinaryReaderEx br)
                => new Vector4(ReadShortNorm(br), ReadShortNorm(br), ReadShortNorm(br), ReadShortNorm(br));

            public static float ReadUShortNorm(BinaryReaderEx br)
                => (br.ReadUInt16() - 32767) / 32767f;

            public static unsafe float ReadFloat16(BinaryReaderEx br)
            {
                // Only handles normalized float 16
                ushort f = br.ReadUInt16();
                uint sign = (uint)(f >> 15);
                uint exp = (uint)((f >> 10) & 0x1F);
                uint mantissa = (uint)(f & 0x3F);
                uint nmantissa = mantissa << 13;
                uint nexp = 127 - 15 + exp;
                uint nf = (sign << 31) | (nexp << 23) | nmantissa;
                return *(float*)&nf;
            }

            public static Vector3 ReadUShortNormXYZ(BinaryReaderEx br)
                => new Vector3(ReadUShortNorm(br), ReadUShortNorm(br), ReadUShortNorm(br));

            public static Vector3 ReadFloat16NormXYZ(BinaryReaderEx br)
                => new Vector3(ReadFloat16(br), ReadFloat16(br), ReadFloat16(br));
            #endregion

            internal void Write(BinaryWriterEx bw, List<LayoutMember> layout, float uvFactor)
            {
                foreach (LayoutMember member in layout)
                {
                    if (member.Semantic == LayoutSemantic.Position)
                    {
                        if (member.Type == LayoutType.Float3)
                        {
                            bw.WriteVector3(Position);
                        }
                        else if (member.Type == LayoutType.Float4)
                        {
                            bw.WriteVector3(Position);
                            bw.WriteSingle(0);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.BoneWeights)
                    {
                        if (member.Type == LayoutType.Byte4A)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteSByte((sbyte)Math.Round(BoneWeights[i] * 127));
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteByte((byte)Math.Round(BoneWeights[i] * 255));
                        }
                        else if (member.Type == LayoutType.UVPair)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteInt16((short)Math.Round(BoneWeights[i] * 32767));
                        }
                        else if (member.Type == LayoutType.Short4toFloat4A)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteInt16((short)Math.Round(BoneWeights[i] * 32767));
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.BoneIndices)
                    {
                        if (member.Type == LayoutType.Byte4B)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteByte((byte)BoneIndices[i]);
                        }
                        else if (member.Type == LayoutType.ShortBoneIndices)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteUInt16((ushort)BoneIndices[i]);
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            for (int i = 0; i < 4; i++)
                                bw.WriteByte((byte)BoneIndices[i]);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.Normal)
                    {
                        if (member.Type == LayoutType.Float3)
                        {
                            bw.WriteVector3(Normal);
                        }
                        else if (member.Type == LayoutType.Float4)
                        {
                            bw.WriteVector3(Normal);
                            bw.WriteSingle(NormalW);
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            WriteByteNormXYZ(bw, Normal);
                            bw.WriteByte((byte)NormalW);
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            WriteByteNormXYZ(bw, Normal);
                            bw.WriteByte((byte)NormalW);
                        }
                        else if (member.Type == LayoutType.Short2toFloat2)
                        {
                            bw.WriteByte((byte)NormalW);
                            WriteSByteNormZYX(bw, Normal);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            WriteByteNormXYZ(bw, Normal);
                            bw.WriteByte((byte)NormalW);
                        }
                        else if (member.Type == LayoutType.Short4toFloat4A)
                        {
                            WriteShortNormXYZ(bw, Normal);
                            bw.WriteInt16((short)NormalW);
                        }
                        else if (member.Type == LayoutType.Short4toFloat4B)
                        {
                            WriteUShortNormXYZ(bw, Normal);
                            bw.WriteInt16((short)NormalW);
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            WriteByteNormXYZ(bw, Normal);
                            bw.WriteByte((byte)NormalW);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.UV)
                    {
                        Vector3 uv = uvQueue.Dequeue() * uvFactor;
                        if (member.Type == LayoutType.Float2)
                        {
                            bw.WriteSingle(uv.X);
                            bw.WriteSingle(uv.Y);
                        }
                        else if (member.Type == LayoutType.Float3)
                        {
                            bw.WriteVector3(uv);
                        }
                        else if (member.Type == LayoutType.Float4)
                        {
                            bw.WriteSingle(uv.X);
                            bw.WriteSingle(uv.Y);

                            uv = uvQueue.Dequeue() * uvFactor;
                            bw.WriteSingle(uv.X);
                            bw.WriteSingle(uv.Y);
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                        }
                        else if (member.Type == LayoutType.Short2toFloat2)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                        }
                        else if (member.Type == LayoutType.UV)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                        }
                        else if (member.Type == LayoutType.UVPair)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));

                            uv = uvQueue.Dequeue() * uvFactor;
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                        }
                        else if (member.Type == LayoutType.Short4toFloat4B)
                        {
                            bw.WriteInt16((short)Math.Round(uv.X));
                            bw.WriteInt16((short)Math.Round(uv.Y));
                            bw.WriteInt16((short)Math.Round(uv.Z));
                            bw.WriteInt16(0);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.Tangent)
                    {
                        Vector4 tangent = tangentQueue.Dequeue();
                        if (member.Type == LayoutType.Float4)
                        {
                            bw.WriteVector4(tangent);
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            WriteByteNormXYZW(bw, tangent);
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            WriteByteNormXYZW(bw, tangent);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            WriteByteNormXYZW(bw, tangent);
                        }
                        else if (member.Type == LayoutType.Short4toFloat4A)
                        {
                            WriteShortNormXYZW(bw, tangent);
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            WriteByteNormXYZW(bw, tangent);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.Bitangent)
                    {
                        if (member.Type == LayoutType.Byte4A)
                        {
                            WriteByteNormXYZW(bw, Bitangent);
                        }
                        else if (member.Type == LayoutType.Byte4B)
                        {
                            WriteByteNormXYZW(bw, Bitangent);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            WriteByteNormXYZW(bw, Bitangent);
                        }
                        else if (member.Type == LayoutType.Byte4E)
                        {
                            WriteByteNormXYZW(bw, Bitangent);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else if (member.Semantic == LayoutSemantic.VertexColor)
                    {
                        VertexColor color = colorQueue.Dequeue();
                        if (member.Type == LayoutType.Float4)
                        {
                            color.WriteFloatRGBA(bw);
                        }
                        else if (member.Type == LayoutType.Byte4A)
                        {
                            color.WriteByteRGBA(bw);
                        }
                        else if (member.Type == LayoutType.Byte4C)
                        {
                            color.WriteByteRGBA(bw);
                        }
                        else
                            throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                    }
                    else
                        throw new NotImplementedException($"Write not implemented for {member.Type} {member.Semantic}.");
                }
            }

            #region Write Helpers
            private static void WriteByteNorm(BinaryWriterEx bw, float value)
                => bw.WriteByte((byte)Math.Round(value * 127 + 127));

            private static void WriteByteNormXYZ(BinaryWriterEx bw, Vector3 value)
            {
                WriteByteNorm(bw, value.X);
                WriteByteNorm(bw, value.Y);
                WriteByteNorm(bw, value.Z);
            }

            private static void WriteByteNormXYZW(BinaryWriterEx bw, Vector4 value)
            {
                WriteByteNorm(bw, value.X);
                WriteByteNorm(bw, value.Y);
                WriteByteNorm(bw, value.Z);
                WriteByteNorm(bw, value.W);
            }

            private static void WriteSByteNorm(BinaryWriterEx bw, float value)
                => bw.WriteSByte((sbyte)Math.Round(value * 127));

            private static void WriteSByteNormZYX(BinaryWriterEx bw, Vector3 value)
            {
                WriteSByteNorm(bw, value.Z);
                WriteSByteNorm(bw, value.Y);
                WriteSByteNorm(bw, value.X);
            }

            private static void WriteShortNorm(BinaryWriterEx bw, float value)
                => bw.WriteInt16((short)Math.Round(value * 32767));

            private static void WriteShortNormXYZ(BinaryWriterEx bw, Vector3 value)
            {
                WriteShortNorm(bw, value.X);
                WriteShortNorm(bw, value.Y);
                WriteShortNorm(bw, value.Z);
            }

            private static void WriteShortNormXYZW(BinaryWriterEx bw, Vector4 value)
            {
                WriteShortNorm(bw, value.X);
                WriteShortNorm(bw, value.Y);
                WriteShortNorm(bw, value.Z);
                WriteShortNorm(bw, value.W);
            }

            private static void WriteUShortNorm(BinaryWriterEx bw, float value)
                => bw.WriteUInt16((ushort)Math.Round(value * 32767 + 32767));

            private static void WriteUShortNormXYZ(BinaryWriterEx bw, Vector3 value)
            {
                WriteUShortNorm(bw, value.X);
                WriteUShortNorm(bw, value.Y);
                WriteUShortNorm(bw, value.Z);
            }
            #endregion
        }
    }
}
