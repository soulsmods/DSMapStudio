using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// An rendering configuration file for various game assets, only used in DS2. Extension: .acb
    /// </summary>
    public class ACB : SoulsFile<ACB>
    {
        /// <summary>
        /// Assets configured by this ACB.
        /// </summary>
        public List<Asset> Assets { get; set; }

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            return br.GetASCII(0, 4) == "ACB\0";
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("ACB\0");
            br.AssertInt32(0x00000102);
            int assetCount = br.ReadInt32();
            br.ReadInt32(); // Offset index offset

            Assets = new List<Asset>(assetCount);
            foreach (int assetOffset in br.ReadInt32s(assetCount))
            {
                br.Position = assetOffset;
                AssetType type = br.GetEnum16<AssetType>(br.Position + 8);
                if (type == AssetType.General)
                    Assets.Add(new Asset.General(br));
                else if (type == AssetType.Model)
                    Assets.Add(new Asset.Model(br));
                else if (type == AssetType.Texture)
                    Assets.Add(new Asset.Texture(br));
                else if (type == AssetType.GITexture)
                    Assets.Add(new Asset.GITexture(br));
                else if (type == AssetType.Motion)
                    Assets.Add(new Asset.Motion(br));
                else
                    throw new NotImplementedException($"Unsupported asset type: {type}");
            }
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            var offsetIndex = new List<int>();
            var memberOffsetsIndex = new SortedDictionary<int, List<int>>();

            bw.BigEndian = false;
            bw.WriteASCII("ACB\0");
            bw.WriteInt32(0x00000102);
            bw.WriteInt32(Assets.Count);
            bw.ReserveInt32("OffsetIndexOffset");

            for (int i = 0; i < Assets.Count; i++)
            {
                offsetIndex.Add((int)bw.Position);
                bw.ReserveInt32($"AssetOffset{i}");
            }

            for (int i = 0; i < Assets.Count; i++)
            {
                bw.FillInt32($"AssetOffset{i}", (int)bw.Position);
                Assets[i].Write(bw, i, offsetIndex, memberOffsetsIndex);
            }

            for (int i = 0; i < Assets.Count; i++)
            {
                if (Assets[i] is Asset.Model model)
                {
                    model.WriteMembers(bw, i, offsetIndex, memberOffsetsIndex);
                }
            }

            for (int i = 0; i < Assets.Count; i++)
            {
                Assets[i].WritePaths(bw, i);
            }

            for (int i = 0; i < Assets.Count; i++)
            {
                if (Assets[i] is Asset.Model model && model.Members != null)
                {
                    for (int j = 0; j < model.Members.Count; j++)
                    {
                        model.Members[j].WriteText(bw, i, j);
                    }
                }
            }

            bw.Pad(4);
            bw.FillInt32("OffsetIndexOffset", (int)bw.Position);
            bw.WriteInt32s(offsetIndex);
            foreach (List<int> offsets in memberOffsetsIndex.Values)
                bw.WriteInt32s(offsets);
        }

        /// <summary>
        /// The specific type of an asset.
        /// </summary>
        public enum AssetType : ushort
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            General = 1,
            Model = 2,
            Texture = 3,
            GITexture = 4,
            Motion = 5,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// A model, texture, or miscellanous asset configuration.
        /// </summary>
        public abstract class Asset
        {
            /// <summary>
            /// The specific type of this asset.
            /// </summary>
            public abstract AssetType Type { get; }

            /// <summary>
            /// Full network path to the source file.
            /// </summary>
            public string AbsolutePath { get; set; }

            /// <summary>
            /// Relative path to the source file.
            /// </summary>
            public string RelativePath { get; set; }

            internal Asset()
            {
                AbsolutePath = "";
                RelativePath = "";
            }

            internal Asset(BinaryReaderEx br)
            {
                int absolutePathOffset = br.ReadInt32();
                int relativePathOffset = br.ReadInt32();
                br.AssertUInt16((ushort)Type);

                AbsolutePath = br.GetUTF16(absolutePathOffset);
                RelativePath = br.GetUTF16(relativePathOffset);
            }

            internal virtual void Write(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
            {
                offsetIndex.Add((int)bw.Position);
                bw.ReserveInt32($"AbsolutePathOffset{index}");
                offsetIndex.Add((int)bw.Position);
                bw.ReserveInt32($"RelativePathOffset{index}");
                bw.WriteUInt16((ushort)Type);
            }

            internal void WritePaths(BinaryWriterEx bw, int index)
            {
                bw.FillInt32($"AbsolutePathOffset{index}", (int)bw.Position);
                bw.WriteUTF16(AbsolutePath, true);

                bw.FillInt32($"RelativePathOffset{index}", (int)bw.Position);
                bw.WriteUTF16(RelativePath, true);
            }

            /// <summary>
            /// Returns a string representation of the entry.
            /// </summary>
            public override string ToString()
            {
                return $"{Type}: {RelativePath} | {AbsolutePath}";
            }

            /// <summary>
            /// Miscellaneous assets including collisions and lighting configs.
            /// </summary>
            public class General : Asset
            {
                /// <summary>
                /// AssetType.General
                /// </summary>
                public override AssetType Type => AssetType.General;

                /// <summary>
                /// Creates a General with default values.
                /// </summary>
                public General() : base() { }

                internal General(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt16(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
                {
                    base.Write(bw, index, offsetIndex, membersOffsetIndex);
                    bw.WriteInt16(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Rendering options for 3D models.
            /// </summary>
            public class Model : Asset
            {
                /// <summary>
                /// AssetType.Model
                /// </summary>
                public override AssetType Type => AssetType.Model;

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk0A { get; set; }

                /// <summary>
                /// Unknown; may be null.
                /// </summary>
                public List<Member> Members { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk1C { get; set; }

                /// <summary>
                /// Whether the model appears in reflective surfaces like water.
                /// </summary>
                public bool Reflectible { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool Unk1F { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk20 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk24 { get; set; }

                /// <summary>
                /// If true, the model does not cast shadows.
                /// </summary>
                public bool DisableShadowSource { get; set; }

                /// <summary>
                /// If true, shadows will not be cast on the model.
                /// </summary>
                public bool DisableShadowTarget { get; set; }

                /// <summary>
                /// Unknown; makes things render in reverse order or reverses culling or something.
                /// </summary>
                public bool Unk27 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float Unk28 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool Unk2C { get; set; }

                /// <summary>
                /// If true, the model is always centered on the camera position. Used for skyboxes.
                /// </summary>
                public bool FixToCamera { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool Unk2E { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk30 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk32 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk34 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool Unk35 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool Unk36 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool Unk37 { get; set; }

                /// <summary>
                /// Creates a Model with default values.
                /// </summary>
                public Model() : base()
                {
                    Reflectible = true;
                }

                internal Model(BinaryReaderEx br) : base(br)
                {
                    Unk0A = br.ReadInt16();
                    int membersOffset = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    Unk1C = br.ReadInt16();
                    Reflectible = br.ReadBoolean();
                    Unk1F = br.ReadBoolean();
                    Unk20 = br.ReadInt32();
                    Unk24 = br.ReadByte();
                    DisableShadowSource = br.ReadBoolean();
                    DisableShadowTarget = br.ReadBoolean();
                    Unk27 = br.ReadBoolean();
                    Unk28 = br.ReadSingle();
                    Unk2C = br.ReadBoolean();
                    FixToCamera = br.ReadBoolean();
                    Unk2E = br.ReadBoolean();
                    br.AssertByte(0);
                    Unk30 = br.ReadInt16();
                    Unk32 = br.ReadInt16();
                    Unk34 = br.ReadByte();
                    Unk35 = br.ReadBoolean();
                    Unk36 = br.ReadBoolean();
                    Unk37 = br.ReadBoolean();
                    br.AssertPattern(0x18, 0x00);

                    if (membersOffset != 0)
                    {
                        br.Position = membersOffset;
                        br.AssertInt16(-1);
                        short memberCount = br.ReadInt16();
                        int memberOffsetsOffset = br.ReadInt32();

                        br.Position = memberOffsetsOffset;
                        Members = new List<Member>(memberCount);
                        foreach (int memberOffset in br.ReadInt32s(memberCount))
                        {
                            br.Position = memberOffset;
                            Members.Add(new Member(br));
                        }
                    }
                }

                internal override void Write(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
                {
                    base.Write(bw, index, offsetIndex, membersOffsetIndex);
                    bw.WriteInt16(Unk0A);
                    membersOffsetIndex[index] = new List<int>();
                    if (Members != null)
                        membersOffsetIndex[index].Add((int)bw.Position);
                    bw.ReserveInt32($"MembersOffset{index}");
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt16(Unk1C);
                    bw.WriteBoolean(Reflectible);
                    bw.WriteBoolean(Unk1F);
                    bw.WriteInt32(Unk20);
                    bw.WriteByte(Unk24);
                    bw.WriteBoolean(DisableShadowSource);
                    bw.WriteBoolean(DisableShadowTarget);
                    bw.WriteBoolean(Unk27);
                    bw.WriteSingle(Unk28);
                    bw.WriteBoolean(Unk2C);
                    bw.WriteBoolean(FixToCamera);
                    bw.WriteBoolean(Unk2E);
                    bw.WriteByte(0);
                    bw.WriteInt16(Unk30);
                    bw.WriteInt16(Unk32);
                    bw.WriteByte(Unk34);
                    bw.WriteBoolean(Unk35);
                    bw.WriteBoolean(Unk36);
                    bw.WriteBoolean(Unk37);
                    bw.WritePattern(0x18, 0x00);
                }

                internal void WriteMembers(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
                {
                    if (Members == null)
                    {
                        bw.FillInt32($"MembersOffset{index}", 0);
                    }
                    else
                    {
                        bw.FillInt32($"MembersOffset{index}", (int)bw.Position);
                        bw.WriteInt16(-1);
                        bw.WriteInt16((short)Members.Count);
                        membersOffsetIndex[index].Add((int)bw.Position);
                        bw.ReserveInt32($"MemberOffsetsOffset{index}");

                        // :^)
                        bw.FillInt32($"MemberOffsetsOffset{index}", (int)bw.Position);
                        for (int i = 0; i < Members.Count; i++)
                        {
                            membersOffsetIndex[index].Add((int)bw.Position);
                            bw.ReserveInt32($"MemberOffset{index}:{i}");
                        }

                        for (int i = 0; i < Members.Count; i++)
                        {
                            bw.FillInt32($"MemberOffset{index}:{i}", (int)bw.Position);
                            Members[i].Write(bw, index, i, offsetIndex);
                        }
                    }
                }

                /// <summary>
                /// Unknown.
                /// </summary>
                public class Member
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public string Text { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public int Unk04 { get; set; }

                    /// <summary>
                    /// Creates a Member with default values.
                    /// </summary>
                    public Member()
                    {
                        Text = "";
                    }

                    internal Member(BinaryReaderEx br)
                    {
                        int textOffset = br.ReadInt32();
                        Unk04 = br.ReadInt32();

                        Text = br.GetUTF16(textOffset);
                    }

                    internal void Write(BinaryWriterEx bw, int entryIndex, int memberIndex, List<int> offsetIndex)
                    {
                        offsetIndex.Add((int)bw.Position);
                        bw.ReserveInt32($"MemberTextOffset{entryIndex}:{memberIndex}");
                        bw.WriteInt32(Unk04);
                    }

                    internal void WriteText(BinaryWriterEx bw, int entryIndex, int memberIndex)
                    {
                        bw.FillInt32($"MemberTextOffset{entryIndex}:{memberIndex}", (int)bw.Position);
                        bw.WriteUTF16(Text, true);
                    }
                }
            }

            /// <summary>
            /// Diffuse, normal, and specular maps.
            /// </summary>
            public class Texture : Asset
            {
                /// <summary>
                /// AssetType.Texture
                /// </summary>
                public override AssetType Type => AssetType.Texture;

                /// <summary>
                /// Creates a Texture with default values.
                /// </summary>
                public Texture() : base() { }

                internal Texture(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt16(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
                {
                    base.Write(bw, index, offsetIndex, membersOffsetIndex);
                    bw.WriteInt16(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Lightmaps and envmaps.
            /// </summary>
            public class GITexture : Asset
            {
                /// <summary>
                /// AssetType.GITexture
                /// </summary>
                public override AssetType Type => AssetType.GITexture;

                /// <summary>
                /// Unknown; probably 4 bytes.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown; probably 4 bytes.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Creates a GITexture with default values.
                /// </summary>
                public GITexture() : base() { }

                internal GITexture(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt16(0);
                    br.AssertInt32(0);
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                }

                internal override void Write(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
                {
                    base.Write(bw, index, offsetIndex, membersOffsetIndex);
                    bw.WriteInt16(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                }
            }

            /// <summary>
            /// Animation files used in cutscenes.
            /// </summary>
            public class Motion : Asset
            {
                /// <summary>
                /// AssetType.Motion
                /// </summary>
                public override AssetType Type => AssetType.Motion;

                /// <summary>
                /// Creates a Motion with default values.
                /// </summary>
                public Motion() : base() { }

                internal Motion(BinaryReaderEx br) : base(br)
                {
                    br.AssertInt16(0);
                    br.AssertInt32(0);
                }

                internal override void Write(BinaryWriterEx bw, int index, List<int> offsetIndex, SortedDictionary<int, List<int>> membersOffsetIndex)
                {
                    base.Write(bw, index, offsetIndex, membersOffsetIndex);
                    bw.WriteInt16(0);
                    bw.WriteInt32(0);
                }
            }
        }
    }
}
