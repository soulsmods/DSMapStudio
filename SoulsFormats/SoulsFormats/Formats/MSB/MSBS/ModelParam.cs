using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsFormats
{
    public partial class MSBS
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum ModelType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Player = 4,
            Collision = 5,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Model files that are available for parts to use.
        /// </summary>
        public class ModelParam : Param<Model>, IMsbParam<IMsbModel>
        {
            /// <summary>
            /// Models for fixed terrain and scenery.
            /// </summary>
            public List<Model.MapPiece> MapPieces { get; set; }

            /// <summary>
            /// Models for dynamic props.
            /// </summary>
            public List<Model.Object> Objects { get; set; }

            /// <summary>
            /// Models for non-player entities.
            /// </summary>
            public List<Model.Enemy> Enemies { get; set; }

            /// <summary>
            /// Models for player spawn points, I think.
            /// </summary>
            public List<Model.Player> Players { get; set; }

            /// <summary>
            /// Models for physics collision.
            /// </summary>
            public List<Model.Collision> Collisions { get; set; }

            /// <summary>
            /// Creates an empty ModelParam with the given version.
            /// </summary>
            public ModelParam(int unk00 = 0x23) : base(unk00, "MODEL_PARAM_ST")
            {
                MapPieces = new List<Model.MapPiece>();
                Objects = new List<Model.Object>();
                Enemies = new List<Model.Enemy>();
                Players = new List<Model.Player>();
                Collisions = new List<Model.Collision>();
            }

            internal override Model ReadEntry(BinaryReaderEx br)
            {
                ModelType type = br.GetEnum32<ModelType>(br.Position + 8);
                switch (type)
                {
                    case ModelType.MapPiece:
                        var mapPiece = new Model.MapPiece(br);
                        MapPieces.Add(mapPiece);
                        return mapPiece;

                    case ModelType.Object:
                        var obj = new Model.Object(br);
                        Objects.Add(obj);
                        return obj;

                    case ModelType.Enemy:
                        var enemy = new Model.Enemy(br);
                        Enemies.Add(enemy);
                        return enemy;

                    case ModelType.Player:
                        var player = new Model.Player(br);
                        Players.Add(player);
                        return player;

                    case ModelType.Collision:
                        var collision = new Model.Collision(br);
                        Collisions.Add(collision);
                        return collision;

                    default:
                        throw new NotImplementedException($"Unimplemented model type: {type}");
                }
            }

            /// <summary>
            /// Returns every Model in the order they will be written.
            /// </summary>
            public override List<Model> GetEntries()
            {
                return SFUtil.ConcatAll<Model>(
                    MapPieces, Objects, Enemies, Players, Collisions);
            }
            IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries() => GetEntries();

            public void Add(IMsbModel item)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// A model file available for parts to reference.
        /// </summary>
        public abstract class Model : Entry, IMsbModel
        {
            /// <summary>
            /// The type of this Model.
            /// </summary>
            public abstract ModelType Type { get; }

            internal abstract bool HasTypeData { get; }

            /// <summary>
            /// A path to a .sib file, presumed to be some kind of editor placeholder.
            /// </summary>
            public string Placeholder { get; set; }

            private int InstanceCount;

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk1C { get; set; }

            internal Model()
            {
                Name = "";
                Placeholder = "";
            }

            internal Model(BinaryReaderEx br)
            {
                long start = br.Position;
                long nameOffset = br.ReadInt64();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                long sibOffset = br.ReadInt64();
                InstanceCount = br.ReadInt32();
                Unk1C = br.ReadInt32();
                long typeDataOffset = br.ReadInt64();

                Name = br.GetUTF16(start + nameOffset);
                Placeholder = br.GetUTF16(start + sibOffset);
                br.Position = start + typeDataOffset;
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.ReserveInt64("SibOffset");
                bw.WriteInt32(InstanceCount);
                bw.WriteInt32(Unk1C);
                bw.ReserveInt64("TypeDataOffset");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(MSB.ReambiguateName(Name), true);
                bw.FillInt64("SibOffset", bw.Position - start);
                bw.WriteUTF16(Placeholder, true);
                bw.Pad(8);

                if (HasTypeData)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    WriteTypeData(bw);
                }
                else
                {
                    bw.FillInt64("TypeDataOffset", 0);
                }
            }

            internal virtual void WriteTypeData(BinaryWriterEx bw)
            {
                throw new InvalidOperationException("Type data should not be written for models with no type data.");
            }

            internal void CountInstances(List<Part> parts)
            {
                InstanceCount = parts.Count(p => p.ModelName == Name);
            }

            /// <summary>
            /// Returns the type and name of the model as a string.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{Type} {Name}";
            }

            /// <summary>
            /// A model for fixed terrain or scenery.
            /// </summary>
            public class MapPiece : Model
            {
                /// <summary>
                /// ModelType.MapPiece
                /// </summary>
                public override ModelType Type => ModelType.MapPiece;

                internal override bool HasTypeData => true;

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool UnkT00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool UnkT01 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public bool UnkT02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public float UnkT18 { get; set; }

                /// <summary>
                /// Creates a MapPiece with default values.
                /// </summary>
                public MapPiece() : base() { }

                internal MapPiece(BinaryReaderEx br) : base(br)
                {
                    UnkT00 = br.ReadBoolean();
                    UnkT01 = br.ReadBoolean();
                    UnkT02 = br.ReadBoolean();
                    br.AssertByte(0);
                    UnkT04 = br.ReadSingle();
                    UnkT08 = br.ReadSingle();
                    UnkT0C = br.ReadSingle();
                    UnkT10 = br.ReadSingle();
                    UnkT14 = br.ReadSingle();
                    UnkT18 = br.ReadSingle();
                    br.AssertInt32(0);
                }

                internal override void WriteTypeData(BinaryWriterEx bw)
                {
                    bw.WriteBoolean(UnkT00);
                    bw.WriteBoolean(UnkT01);
                    bw.WriteBoolean(UnkT02);
                    bw.WriteByte(0);
                    bw.WriteSingle(UnkT04);
                    bw.WriteSingle(UnkT08);
                    bw.WriteSingle(UnkT0C);
                    bw.WriteSingle(UnkT10);
                    bw.WriteSingle(UnkT14);
                    bw.WriteSingle(UnkT18);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// A model for a dynamic prop.
            /// </summary>
            public class Object : Model
            {
                /// <summary>
                /// ModelType.Object
                /// </summary>
                public override ModelType Type => ModelType.Object;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates an Object with default values.
                /// </summary>
                public Object() : base() { }

                internal Object(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A model for a non-player entity.
            /// </summary>
            public class Enemy : Model
            {
                /// <summary>
                /// ModelType.Enemy
                /// </summary>
                public override ModelType Type => ModelType.Enemy;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates an Enemy with default values.
                /// </summary>
                public Enemy() : base() { }

                internal Enemy(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A model for a player spawn point?
            /// </summary>
            public class Player : Model
            {
                /// <summary>
                /// ModelType.Player
                /// </summary>
                public override ModelType Type => ModelType.Player;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a Player with default values.
                /// </summary>
                public Player() : base() { }

                internal Player(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A model for collision physics.
            /// </summary>
            public class Collision : Model
            {
                /// <summary>
                /// ModelType.Collision
                /// </summary>
                public override ModelType Type => ModelType.Collision;

                internal override bool HasTypeData => false;

                /// <summary>
                /// Creates a Collision with default values.
                /// </summary>
                public Collision() : base() { }

                internal Collision(BinaryReaderEx br) : base(br) { }
            }
        }
    }
}
