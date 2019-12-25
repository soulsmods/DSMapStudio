using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSB2
    {
        public enum ModelType : ushort
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            MapPiece = 0,
            Object = 1,
            Collision = 3,
            Navmesh = 4,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// Models available for parts in the map to use.
        /// </summary>
        public class ModelParam : Param<Model>, IMsbParam<IMsbModel>
        {
            internal override string Name => "MODEL_PARAM_ST";
            internal override int Version => 5;

            /// <summary>
            /// Models for static visual elements.
            /// </summary>
            public List<Model> MapPieces { get; set; }

            /// <summary>
            /// Models for dynamic or interactible elements.
            /// </summary>
            public List<Model> Objects { get; set; }

            /// <summary>
            /// Models for invisible but physical surfaces.
            /// </summary>
            public List<Model> Collisions { get; set; }

            /// <summary>
            /// Models for AI navigation.
            /// </summary>
            public List<Model> Navmeshes { get; set; }

            /// <summary>
            /// Creates an empty ModelParam.
            /// </summary>
            public ModelParam()
            {
                MapPieces = new List<Model>();
                Objects = new List<Model>();
                Collisions = new List<Model>();
                Navmeshes = new List<Model>();
            }

            internal override Model ReadEntry(BinaryReaderEx br)
            {
                var model = new Model(br);
                switch (model.Type)
                {
                    case ModelType.MapPiece:
                        MapPieces.Add(model);
                        return model;

                    case ModelType.Object:
                        Objects.Add(model);
                        return model;

                    case ModelType.Collision:
                        Collisions.Add(model);
                        return model;

                    case ModelType.Navmesh:
                        Navmeshes.Add(model);
                        return model;

                    default:
                        throw new NotImplementedException($"Unimplemented model type: {model.Type}");
                }
            }

            /// <summary>
            /// Returns every Model in the order they'll be written.
            /// </summary>
            public override List<Model> GetEntries()
            {
                return SFUtil.ConcatAll(
                    MapPieces, Objects, Collisions, Navmeshes);
            }
            IReadOnlyList<IMsbModel> IMsbParam<IMsbModel>.GetEntries() => GetEntries();

            internal void DiscriminateModels()
            {
                for (short i = 0; i < MapPieces.Count; i++)
                    MapPieces[i].Discriminate(ModelType.MapPiece, i);
                for (short i = 0; i < Objects.Count; i++)
                    Objects[i].Discriminate(ModelType.Object, i);
                for (short i = 0; i < Collisions.Count; i++)
                    Collisions[i].Discriminate(ModelType.Collision, i);
                for (short i = 0; i < Navmeshes.Count; i++)
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
        public class Model : NamedEntry, IMsbModel
        {
            public ModelType Type;
            internal short Index;

            /// <summary>
            /// Creates a Model with the given name.
            /// </summary>
            public Model(string name = "")
            {
                Name = name;
            }

            internal Model(BinaryReaderEx br)
            {
                long start = br.Position;
                long nameOffset = br.ReadInt64();
                Type = br.ReadEnum16<ModelType>();
                br.ReadInt16(); // Index
                br.AssertInt32(0);
                long typeDataOffset = br.ReadInt64();
                br.AssertInt64(0);

                Name = br.GetUTF16(start + nameOffset);

                if (Type == ModelType.Object)
                {
                    br.Position = start + typeDataOffset;
                    br.AssertInt64(0);
                }
            }

            internal override void Write(BinaryWriterEx bw, int index)
            {
                long start = bw.Position;
                bw.ReserveInt64("NameOffset");
                bw.WriteUInt16((ushort)Type);
                bw.WriteInt16(Index);
                bw.WriteInt32(0);
                bw.ReserveInt64("TypeDataOffset");
                bw.WriteInt64(0);

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(MSB.ReambiguateName(Name), true);
                bw.Pad(8);

                if (Type == ModelType.Object)
                {
                    bw.FillInt64("TypeDataOffset", bw.Position - start);
                    bw.WriteInt64(0);
                }
                else
                {
                    bw.FillInt64("TypeDataOffset", 0);
                }
            }

            internal void Discriminate(ModelType type, short index)
            {
                Type = type;
                Index = index;
            }

            /// <summary>
            /// Returns a string representation of the model.
            /// </summary>
            public override string ToString()
            {
                return $"{Name}";
            }
        }
    }
}
