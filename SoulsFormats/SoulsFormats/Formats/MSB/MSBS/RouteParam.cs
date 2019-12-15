using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    public partial class MSBS
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum RouteType : uint
        {
            MufflingPortalLink = 3,
            MufflingBoxLink = 4,
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Unknown, but related to muffling regions somehow.
        /// </summary>
        public class RouteParam : Param<Route>
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Route.MufflingPortalLink> MufflingPortalLinks { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Route.MufflingBoxLink> MufflingBoxLinks { get; set; }

            /// <summary>
            /// Creates an empty RouteParam with the given version.
            /// </summary>
            public RouteParam(int unk00 = 0x23) : base(unk00, "ROUTE_PARAM_ST")
            {
                MufflingPortalLinks = new List<Route.MufflingPortalLink>();
                MufflingBoxLinks = new List<Route.MufflingBoxLink>();
            }

            internal override Route ReadEntry(BinaryReaderEx br)
            {
                RouteType type = br.GetEnum32<RouteType>(br.Position + 0x10);
                switch (type)
                {
                    case RouteType.MufflingPortalLink:
                        var portalLink = new Route.MufflingPortalLink(br);
                        MufflingPortalLinks.Add(portalLink);
                        return portalLink;

                    case RouteType.MufflingBoxLink:
                        var boxLink = new Route.MufflingBoxLink(br);
                        MufflingBoxLinks.Add(boxLink);
                        return boxLink;

                    default:
                        throw new NotImplementedException($"Unimplemented route type: {type}");
                }
            }

            /// <summary>
            /// Returns every Route in the order they will be written.
            /// </summary>
            public override List<Route> GetEntries()
            {
                return SFUtil.ConcatAll<Route>(
                    MufflingPortalLinks, MufflingBoxLinks);
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public abstract class Route : Entry
        {
            /// <summary>
            /// The type of this Route.
            /// </summary>
            public abstract RouteType Type { get; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk08 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk0C { get; set; }

            internal Route()
            {
                Name = "";
            }

            internal Route(BinaryReaderEx br)
            {
                long start = br.Position;
                long nameOffset = br.ReadInt64();
                Unk08 = br.ReadInt32();
                Unk0C = br.ReadInt32();
                br.AssertUInt32((uint)Type);
                br.ReadInt32(); // ID
                br.AssertPattern(0x68, 0x00);

                Name = br.GetUTF16(start + nameOffset);
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;
                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(Unk08);
                bw.WriteInt32(Unk0C);
                bw.WriteUInt32((uint)Type);
                bw.WriteInt32(id);
                bw.WritePattern(0x68, 0x00);

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(Name, true);
                bw.Pad(8);
            }

            /// <summary>
            /// Returns the name and values associated with the route as a string.
            /// </summary>
            public override string ToString()
            {
                return $"\"{Name}\" {Unk08} {Unk0C}";
            }

            /// <summary>
            /// Unknown; has something to do with muffling portals.
            /// </summary>
            public class MufflingPortalLink : Route
            {
                /// <summary>
                /// RouteType.MufflingPortalLink
                /// </summary>
                public override RouteType Type => RouteType.MufflingPortalLink;

                /// <summary>
                /// Creates a MufflingPortalLink with default values.
                /// </summary>
                public MufflingPortalLink() : base() { }

                internal MufflingPortalLink(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown; has something to do with muffling boxes.
            /// </summary>
            public class MufflingBoxLink : Route
            {
                /// <summary>
                /// RouteType.MufflingBoxLink
                /// </summary>
                public override RouteType Type => RouteType.MufflingBoxLink;

                /// <summary>
                /// Creates a MufflingBoxLink with default values.
                /// </summary>
                public MufflingBoxLink() : base() { }

                internal MufflingBoxLink(BinaryReaderEx br) : base(br) { }
            }
        }
    }
}
