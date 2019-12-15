using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSB2
    {
        private class MapstudioPartsPose : Param<PartPose>
        {
            internal override string Name => "MAPSTUDIO_PARTS_POSE_ST";
            internal override int Version => 0;

            public List<PartPose> Poses { get; set; }

            public MapstudioPartsPose()
            {
                Poses = new List<PartPose>();
            }

            internal override PartPose ReadEntry(BinaryReaderEx br)
            {
                var pose = new PartPose(br);
                Poses.Add(pose);
                return pose;
            }

            public override List<PartPose> GetEntries()
            {
                return Poses;
            }
        }

        /// <summary>
        /// A set of bone transforms to pose a rigged object.
        /// </summary>
        public class PartPose : Entry
        {
            /// <summary>
            /// The name of the part to be posed.
            /// </summary>
            public string PartName { get; set; }
            private short PartIndex;

            /// <summary>
            /// Transforms for each bone in the object.
            /// </summary>
            public List<Bone> Bones { get; set; }

            /// <summary>
            /// Creates an empty PartPose.
            /// </summary>
            public PartPose(string partName = null)
            {
                PartName = partName;
                Bones = new List<Bone>();
            }

            internal PartPose(BinaryReaderEx br)
            {
                PartIndex = br.ReadInt16();
                short boneCount = br.ReadInt16();
                br.AssertInt32(0);
                br.AssertInt64(0x10); // Bones offset

                Bones = new List<Bone>(boneCount);
                for (int i = 0; i < boneCount; i++)
                    Bones.Add(new Bone(br));
            }

            internal override void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt16(PartIndex);
                bw.WriteInt16((short)Bones.Count);
                bw.WriteInt32(0);
                bw.WriteInt64(0x10);

                foreach (Bone bone in Bones)
                    bone.Write(bw);
            }

            internal void GetNames(Entries entries)
            {
                PartName = MSB.FindName(entries.Parts, PartIndex);
                foreach (Bone bone in Bones)
                    bone.GetNames(entries);
            }

            internal void GetIndices(Lookups lookups, Entries entries)
            {
                PartIndex = (short)FindIndex(lookups.Parts, PartName);
                foreach (Bone bone in Bones)
                    bone.GetIndices(lookups, entries);
            }

            /// <summary>
            /// Returns a string representation of the pose.
            /// </summary>
            public override string ToString()
            {
                return $"{PartName} [{Bones?.Count} Bones]";
            }

            /// <summary>
            /// A transform for a single bone in an object.
            /// </summary>
            public class Bone
            {
                /// <summary>
                /// The name of the bone to transform.
                /// </summary>
                public string Name { get; set; }
                private int NameIndex;

                /// <summary>
                /// Translation of the bone.
                /// </summary>
                public Vector3 Translation { get; set; }

                /// <summary>
                /// Rotation of the bone, in radians.
                /// </summary>
                public Vector3 Rotation { get; set; }

                /// <summary>
                /// Scale of the bone.
                /// </summary>
                public Vector3 Scale { get; set; }

                /// <summary>
                /// Creates a Bone with default values.
                /// </summary>
                public Bone(string name = "")
                {
                    Name = name;
                    Scale = Vector3.One;
                }

                internal Bone(BinaryReaderEx br)
                {
                    NameIndex = br.ReadInt32();
                    Translation = br.ReadVector3();
                    Rotation = br.ReadVector3();
                    Scale = br.ReadVector3();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(NameIndex);
                    bw.WriteVector3(Translation);
                    bw.WriteVector3(Rotation);
                    bw.WriteVector3(Scale);
                }

                internal void GetNames(Entries entries)
                {
                    Name = MSB.FindName(entries.BoneNames, NameIndex);
                }

                internal void GetIndices(Lookups lookups, Entries entries)
                {
                    if (!lookups.BoneNames.ContainsKey(Name))
                    {
                        lookups.BoneNames[Name] = entries.BoneNames.Count;
                        entries.BoneNames.Add(new BoneName(Name));
                    }
                    NameIndex = FindIndex(lookups.BoneNames, Name);
                }

                /// <summary>
                /// Returns a string representation of the bone.
                /// </summary>
                public override string ToString()
                {
                    return $"{Name} [Trans {Translation:F2} | Rot {Rotation:F2} | Scale {Scale:F2}]";
                }
            }
        }
    }
}
