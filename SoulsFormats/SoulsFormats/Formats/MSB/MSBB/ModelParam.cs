using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsFormats
{
    public partial class MSBB
    {
        /// <summary>
        /// A section containing all the models available to parts in this map.
        /// </summary>
        public class ModelParam : Section<Model>, IMsbParam<IMsbModel>
        {
            internal override string Type => "MODEL_PARAM_ST";

            /// <summary>
            /// Map piece models in this section.
            /// </summary>
            public List<Model.MapPiece> MapPieces;

            /// <summary>
            /// Object models in this section.
            /// </summary>
            public List<Model.Object> Objects;

            /// <summary>
            /// Enemy models in this section.
            /// </summary>
            public List<Model.Enemy> Enemies;

            /// <summary>
            /// Item models in this section.
            /// </summary>
            public List<Model.Item> Items;

            /// <summary>
            /// Player models in this section.
            /// </summary>
            public List<Model.Player> Players;

            /// <summary>
            /// Collision models in this section.
            /// </summary>
            public List<Model.Collision> Collisions;

            /// <summary>
            /// Navmesh models in this section.
            /// </summary>
            public List<Model.Navmesh> Navmeshes;

            /// <summary>
            /// Other models in this section.
            /// </summary>
            public List<Model.Other> Others;

            /// <summary>
            /// Creates a new ModelSection with no models.
            /// </summary>
            public ModelParam(int unk1 = 3) : base(unk1)
            {
                MapPieces = new List<Model.MapPiece>();
                Objects = new List<Model.Object>();
                Enemies = new List<Model.Enemy>();
                Items = new List<Model.Item>();
                Players = new List<Model.Player>();
                Collisions = new List<Model.Collision>();
                Navmeshes = new List<Model.Navmesh>();
                Others = new List<Model.Other>();
            }

            /// <summary>
            /// Returns every model in the order they will be written.
            /// </summary>
            public override List<Model> GetEntries()
            {
                return SFUtil.ConcatAll<Model>(
                    MapPieces, Objects, Enemies, Items, Players, Collisions, Navmeshes, Others);
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

                    // Potential from mistake in m36 :trashcat:
                    case ModelType.Item:
                        var item = new Model.Item(br);
                        Items.Add(item);
                        return item;

                    case ModelType.Player:
                        var player = new Model.Player(br);
                        Players.Add(player);
                        return player;

                    case ModelType.Collision:
                        var collision = new Model.Collision(br);
                        Collisions.Add(collision);
                        return collision;

                    case ModelType.Navmesh:
                        var navmesh = new Model.Navmesh(br);
                        Navmeshes.Add(navmesh);
                        return navmesh;

                    case ModelType.Other:
                        var other = new Model.Other(br);
                        Others.Add(other);
                        return other;

                    default:
                        throw new NotImplementedException($"Unsupported model type: {type}");
                }
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, Model entry)
            {
                entry.Write(bw, id);
            }

            IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries() => GetEntries();

            public void Add(IMsbModel item)
            {
                switch (item)
                {
                    case Model.MapPiece m:
                        MapPieces.Add(m);
                        break;
                    case Model.Object m:
                        Objects.Add(m);
                        break;
                    case Model.Enemy m:
                        Enemies.Add(m);
                        break;
                    case Model.Player m:
                        Players.Add(m);
                        break;
                    case Model.Collision m:
                        Collisions.Add(m);
                        break;
                    case Model.Navmesh m:
                        Navmeshes.Add(m);
                        break;
                    case Model.Other m:
                        Others.Add(m);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        internal enum ModelType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Item = 3,
            Player = 4,
            Collision = 5,
            Navmesh = 6,
            DummyObject = 7,
            DummyEnemy = 8,
            Other = 0xFFFFFFFF
        }

        /// <summary>
        /// A model available for use by parts in this map.
        /// </summary>
        public abstract class Model : Entry, IMsbModel
        {
            internal abstract ModelType Type { get; }

            /// <summary>
            /// The name of this model.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// The placeholder used for this model in MapStudio.
            /// </summary>
            public string Placeholder { get; set; }

            private int InstanceCount;

            internal Model(string name)
            {
                Name = name;
                Placeholder = "";
            }

            internal Model(Model clone)
            {
                Name = clone.Name;
                Placeholder = clone.Placeholder;
            }

            internal Model(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                long placeholderOffset = br.ReadInt64();
                InstanceCount = br.ReadInt32();
                br.AssertInt32(0);
                br.AssertInt32(0);
                br.AssertInt32(0);

                Name = br.GetUTF16(start + nameOffset);
                Placeholder = br.GetUTF16(start + placeholderOffset);
            }

            internal void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.ReserveInt64("PlaceholderOffset");
                bw.WriteInt32(InstanceCount);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(ReambiguateName(Name), true);
                bw.FillInt64("PlaceholderOffset", bw.Position - start);
                bw.WriteUTF16(Placeholder, true);
                bw.Pad(8);
            }

            internal void CountInstances(List<Part> parts)
            {
                InstanceCount = parts.Count(p => p.ModelName == Name);
            }

            /// <summary>
            /// Returns the model type, ID and name of this model.
            /// </summary>
            public override string ToString()
            {
                return $"{Type} : {Name}";
            }

            /// <summary>
            /// A fixed part of the level geometry.
            /// </summary>
            public class MapPiece : Model
            {
                internal override ModelType Type => ModelType.MapPiece;

                /// <summary>
                /// Creates a new MapPiece with the given ID and name.
                /// </summary>
                public MapPiece(string name) : base(name)
                {
                }

                /// <summary>
                /// Creates a new MapPiece with values copied from another.
                /// </summary>
                public MapPiece(MapPiece clone) : base(clone)
                {
                }

                internal MapPiece(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A dynamic or interactible entity.
            /// </summary>
            public class Object : Model
            {
                internal override ModelType Type => ModelType.Object;

                /// <summary>
                /// Creates a new Object with the given ID and name.
                /// </summary>
                public Object(string name) : base(name)
                {
                }

                /// <summary>
                /// Creates a new Object with values copied from another.
                /// </summary>
                public Object(Object clone) : base(clone)
                {
                }

                internal Object(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Any character in the map that is not the player.
            /// </summary>
            public class Enemy : Model
            {
                internal override ModelType Type => ModelType.Enemy;

                /// <summary>
                /// Creates a new Enemy with the given ID and name.
                /// </summary>
                public Enemy(string name) : base(name) { }

                /// <summary>
                /// Creates a new Enemy with values copied from another.
                /// </summary>
                public Enemy(Enemy clone) : base(clone) { }

                internal Enemy(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// This only exists because one BB map seems to use this by accident :trashcat:
            /// </summary>
            public class Item : Model
            {
                internal override ModelType Type => ModelType.Item;

                /// <summary>
                /// Creates a new Item with the given ID and name.
                /// </summary>
                public Item(string name) : base(name) { }

                /// <summary>
                /// Creates a new Item with values copied from another.
                /// </summary>
                public Item(Item clone) : base(clone) { }

                internal Item(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// The player character.
            /// </summary>
            public class Player : Model
            {
                internal override ModelType Type => ModelType.Player;

                /// <summary>
                /// Creates a new Player with the given ID and name.
                /// </summary>
                public Player(string name) : base(name) { }

                /// <summary>
                /// Creates a new Player with values copied from another.
                /// </summary>
                public Player(Player clone) : base(clone) { }

                internal Player(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// The invisible physical surface of the map.
            /// </summary>
            public class Collision : Model
            {
                internal override ModelType Type => ModelType.Collision;

                /// <summary>
                /// Creates a new Collision with the given ID and name.
                /// </summary>
                public Collision(string name) : base(name) { }

                /// <summary>
                /// Creates a new Collision with values copied from another.
                /// </summary>
                public Collision(Collision clone) : base(clone) { }

                internal Collision(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Navimesh
            /// </summary>
            public class Navmesh : Model
            {
                internal override ModelType Type => ModelType.Navmesh;

                /// <summary>
                /// Creates a new Navmesh with the given ID and name.
                /// </summary>
                public Navmesh(string name) : base(name) { }

                /// <summary>
                /// Creates a new Navmesh with values copied from another.
                /// </summary>
                public Navmesh(Navmesh clone) : base(clone) { }

                internal Navmesh(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Other : Model
            {
                internal override ModelType Type => ModelType.Other;

                /// <summary>
                /// Creates a new Other with the given ID and name.
                /// </summary>
                public Other(string name) : base(name) { }

                /// <summary>
                /// Creates a new Other with values copied from another.
                /// </summary>
                public Other(Other clone) : base(clone) { }

                internal Other(BinaryReaderEx br) : base(br) { }
            }
        }
    }
}
