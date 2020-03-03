using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsFormats
{
    public partial class MSB1
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum ModelType : uint
        {
            MapPiece = 0,
            Object = 1,
            Enemy = 2,
            Player = 4,
            Collision = 5,
            Navmesh = 6,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Model files that are available for parts to use.
        /// </summary>
        public class ModelParam : Param<Model>, IMsbParam<IMsbModel>
        {
            internal override string Name => "MODEL_PARAM_ST";

            /// <summary>
            /// Models for fixed terrain and scenery.
            /// </summary>
            public List<Model> MapPieces { get; set; }

            /// <summary>
            /// Models for dynamic props.
            /// </summary>
            public List<Model> Objects { get; set; }

            /// <summary>
            /// Models for non-player entities.
            /// </summary>
            public List<Model> Enemies { get; set; }

            /// <summary>
            /// Models for player spawn points, I think.
            /// </summary>
            public List<Model> Players { get; set; }

            /// <summary>
            /// Models for physics collision.
            /// </summary>
            public List<Model> Collisions { get; set; }

            /// <summary>
            /// Models for AI navigation.
            /// </summary>
            public List<Model> Navmeshes { get; set; }

            /// <summary>
            /// Creates an empty ModelParam.
            /// </summary>
            public ModelParam() : base()
            {
                MapPieces = new List<Model>();
                Objects = new List<Model>();
                Enemies = new List<Model>();
                Players = new List<Model>();
                Collisions = new List<Model>();
                Navmeshes = new List<Model>();
            }

            internal override Model ReadEntry(BinaryReaderEx br)
            {
                ModelType type = br.GetEnum32<ModelType>(br.Position + 4);
                var model = new Model(br);
                if (type == ModelType.MapPiece)
                    MapPieces.Add(model);
                else if (type == ModelType.Object)
                    Objects.Add(model);
                else if (type == ModelType.Enemy)
                    Enemies.Add(model);
                else if (type == ModelType.Player)
                    Players.Add(model);
                else if (type == ModelType.Collision)
                    Collisions.Add(model);
                else if (type == ModelType.Navmesh)
                    Navmeshes.Add(model);
                else
                    throw new NotImplementedException($"Unimplemented model type: {type}");
                return model;
            }

            /// <summary>
            /// Returns every Model in the order they will be written.
            /// </summary>
            public override List<Model> GetEntries()
            {
                return SFUtil.ConcatAll(
                    MapPieces, Objects, Enemies, Players, Collisions, Navmeshes);
            }
            IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries() => GetEntries();

            internal void DiscriminateModels()
            {
                for (int i = 0; i < MapPieces.Count; i++)
                    MapPieces[i].Discriminate(ModelType.MapPiece, i);
                for (int i = 0; i < Objects.Count; i++)
                    Objects[i].Discriminate(ModelType.Object, i);
                for (int i = 0; i < Enemies.Count; i++)
                    Enemies[i].Discriminate(ModelType.Enemy, i);
                for (int i = 0; i < Players.Count; i++)
                    Players[i].Discriminate(ModelType.Player, i);
                for (int i = 0; i < Collisions.Count; i++)
                    Collisions[i].Discriminate(ModelType.Collision, i);
                for (int i = 0; i < Navmeshes.Count; i++)
                    Navmeshes[i].Discriminate(ModelType.Navmesh, i);
            }

            public void Add(IMsbModel item)
            {
                var m = (Model)item;
                switch (m.Type)
                {
                    case ModelType.MapPiece:
                        MapPieces.Add(m);
                        break;
                    case ModelType.Object:
                        Objects.Add(m);
                        break;
                    case ModelType.Enemy:
                        Enemies.Add(m);
                        break;
                    case ModelType.Player:
                        Players.Add(m);
                        break;
                    case ModelType.Collision:
                        Collisions.Add(m);
                        break;
                    case ModelType.Navmesh:
                        Navmeshes.Add(m);
                        break;
                    default:
                        throw new ArgumentException(
                            message: "Item is not recognized",
                            paramName: nameof(item));
                }
            }
        }

        /// <summary>
        /// A model file available for parts to reference.
        /// </summary>
        public class Model : Entry, IMsbModel
        {
            // Since models have no type data in DS1, this is just set before writing based on the list it's in.
            public ModelType Type { get; set; }
            private int ID;

            /// <summary>
            /// A path to a .sib file, presumed to be some kind of editor placeholder.
            /// </summary>
            public string Placeholder { get; set; }

            private int InstanceCount;

            /// <summary>
            /// Creates a Model with default values.
            /// </summary>
            public Model()
            {
                Name = "";
                Placeholder = "";
            }

            internal Model(BinaryReaderEx br)
            {
                long start = br.Position;
                int nameOffset = br.ReadInt32();
                Type = br.ReadEnum32<ModelType>();
                br.ReadInt32();
                int sibOffset = br.ReadInt32();
                InstanceCount = br.ReadInt32();
                br.AssertInt32(0);
                br.AssertInt32(0);
                br.AssertInt32(0);

                Name = br.GetShiftJIS(start + nameOffset);
                Placeholder = br.GetShiftJIS(start + sibOffset);
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt32("NameOffset");
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(ID);
                bw.ReserveInt32("SibOffset");
                bw.WriteInt32(InstanceCount);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);

                bw.FillInt32("NameOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(MSB.ReambiguateName(Name), true);
                bw.FillInt32("SibOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(Placeholder, true);
                bw.Pad(4);
            }

            internal void CountInstances(List<Part> parts)
            {
                InstanceCount = parts.Count(p => p.ModelName == Name);
            }

            internal void Discriminate(ModelType type, int id)
            {
                Type = type;
                ID = id;
            }
        }
    }
}
