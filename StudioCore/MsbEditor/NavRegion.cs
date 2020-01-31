using System;
using System.Collections.Generic;
using System.Text;
using SoulsFormats;
using Veldrid.Utilities;

namespace StudioCore.MsbEditor
{
    /// <summary>
    /// A (DS1) navigation region which marks a room/area that a navmesh
    /// is active in. Corresponds with an MCP room
    /// </summary>
    public class NavRegion : Scene.ISelectable
    {
        public Scene.IDrawable RenderMesh { get; set; } = null;

        /// <summary>
        /// Index of the map navmesh before resolution
        /// </summary>
        private int _navidx = -1;

        /// <summary>
        /// Navmesh this region is associated with
        /// </summary>
        public MapObject Navmesh { get; set; }

        /// <summary>
        /// Bounding volume of this region
        /// </summary>
        public BoundingBox BoundingBox { get; set; }

        /// <summary>
        /// Indices of neighbors before resolution
        /// </summary>
        private List<int> _nindices = new List<int>();

        /// <summary>
        /// The regions that neighbor and are connected to this region
        /// </summary>
        public List<NavRegion> Neighbors { get; private set; }

        /// <summary>
        /// Construct a region from a deserialized mcp room
        /// </summary>
        public NavRegion(Map enclosingMap, MCP.Room room)
        {
            BoundingBox = new BoundingBox(room.BoundingBoxMin, room.BoundingBoxMax);
            _navidx = room.LocalIndex;
            _nindices.AddRange(room.ConnectedRoomIndices);
        }

        public void OnDeselected()
        {
            throw new NotImplementedException();
        }

        public void OnSelected()
        {
            throw new NotImplementedException();
        }
    }
}
