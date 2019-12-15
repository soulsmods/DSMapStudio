using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB3
    {
        /// <summary>
        /// A section containing fixed poses for different Parts in the map.
        /// </summary>
        public class MapstudioPartsPose : Param<PartsPose>
        {
            internal override string Type => "MAPSTUDIO_PARTS_POSE_ST";

            /// <summary>
            /// Parts pose entries in this section.
            /// </summary>
            public List<PartsPose> Poses;

            /// <summary>
            /// Creates a new PartsPoseSection with no entries.
            /// </summary>
            public MapstudioPartsPose(int unk1 = 0) : base(unk1)
            {
                Poses = new List<PartsPose>();
            }

            /// <summary>
            /// Returns every parts pose in the order they will be written.
            /// </summary>
            public override List<PartsPose> GetEntries()
            {
                return Poses;
            }

            internal override PartsPose ReadEntry(BinaryReaderEx br)
            {
                var pose = new PartsPose(br);
                Poses.Add(pose);
                return pose;
            }

            internal override void WriteEntry(BinaryWriterEx bw, int index, PartsPose entry)
            {
                entry.Write(bw);
            }
        }

        /// <summary>
        /// A set of bone transforms to pose an individual Part in the map.
        /// </summary>
        public class PartsPose
        {
            /// <summary>
            /// The name of the part to pose.
            /// </summary>
            public string PartName;
            private short PartIndex;

            /// <summary>
            /// Transforms for each bone.
            /// </summary>
            public List<Bone> Bones;

            /// <summary>
            /// Creates a new PartsPose with no members.
            /// </summary>
            public PartsPose(string partName)
            {
                PartName = partName;
                Bones = new List<Bone>();
            }

            internal PartsPose(BinaryReaderEx br)
            {
                PartIndex = br.ReadInt16();
                short boneCount = br.ReadInt16();
                br.AssertInt32(0);
                br.AssertInt64(0x10);

                Bones = new List<Bone>(boneCount);
                for (int i = 0; i < boneCount; i++)
                    Bones.Add(new Bone(br));
            }

            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteInt16(PartIndex);
                bw.WriteInt16((short)Bones.Count);
                bw.WriteInt32(0);
                bw.WriteInt64(0x10);

                foreach (var member in Bones)
                    member.Write(bw);
            }

            internal void GetNames(MSB3 msb, Entries entries)
            {
                PartName = MSB.FindName(entries.Parts, PartIndex);
            }

            internal void GetIndices(MSB3 msb, Entries entries)
            {
                PartIndex = (short)MSB.FindIndex(entries.Parts, PartName);
            }

            /// <summary>
            /// A transform for one bone in a model.
            /// </summary>
            public class Bone
            {
                /// <summary>
                /// An index into the BoneNames section.
                /// </summary>
                public int BoneNamesIndex;

                /// <summary>
                /// Translation of the bone.
                /// </summary>
                public Vector3 Translation;

                /// <summary>
                /// Rotation of the bone.
                /// </summary>
                public Vector3 Rotation;

                /// <summary>
                /// Scale of the bone.
                /// </summary>
                public Vector3 Scale;

                /// <summary>
                /// Creates a new Bone with the given bone name index and default transforms.
                /// </summary>
                public Bone(int boneNamesIndex)
                {
                    BoneNamesIndex = boneNamesIndex;
                }

                internal Bone(BinaryReaderEx br)
                {
                    BoneNamesIndex = br.ReadInt32();
                    Translation = br.ReadVector3();
                    Rotation = br.ReadVector3();
                    Scale = br.ReadVector3();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(BoneNamesIndex);
                    bw.WriteVector3(Translation);
                    bw.WriteVector3(Rotation);
                    bw.WriteVector3(Scale);
                }

                /// <summary>
                /// Returns the bone name index and transforms of this bone.
                /// </summary>
                public override string ToString()
                {
                    return $"{BoneNamesIndex} : {Translation} {Rotation} {Scale}";
                }
            }
        }
    }
}
