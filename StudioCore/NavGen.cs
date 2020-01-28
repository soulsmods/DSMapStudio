using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Numerics;

namespace StudioCore
{
    /// <summary>
    /// Binding to Navgen recast based navmesh generation library
    /// </summary>
    class NavGen
    {
        [DllImport("NavGen.dll")]
        public static extern bool SetNavmeshBuildParams(float cs, float ch, float slope, float aheight, float aclimb, float aradius, int minregionarea);

        [DllImport("NavGen.dll")]
        public static extern bool BuildNavmeshForMesh([In] Vector3[] verts, int vcount, [In] int[] indices, int icount);

        [DllImport("NavGen.dll")]
        public static extern int GetMeshVertCount();

        [DllImport("NavGen.dll")]
        public static extern int GetMeshTriCount();

        [DllImport("NavGen.dll")]
        public static extern void GetMeshVerts([In, Out] ushort[] buffer);

        [DllImport("NavGen.dll")]
        public static extern void GetMeshTris([In, Out] ushort[] buffer);

        [DllImport("NavGen.dll")]
        public static extern void GetBoundingBox([In, Out] Vector3[] buffer);
    }
}
