using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using System.Diagnostics;
using System.Globalization;

namespace StudioCore.Scene
{
    /// <summary>
    /// A scene in the context of rendering. This isn't focused on the heirarchy or properties of an object,
    /// but rather is focused on the organization for efficient rendering
    /// </summary>
    public class RenderScene
    {
        //private HashSet<IDrawable> ObjectsSet = new HashSet<IDrawable>();
        //private HashSet<IDrawable> OctreeSet = new HashSet<IDrawable>();
        //private List<IDrawable> Objects = new List<IDrawable>();
        //private Octree<IDrawable> Octree = new Octree<IDrawable>(new BoundingBox(Vector3.One * -500, Vector3.One * 500), 5);

        //private List<IDrawable> CulledObjects = new List<IDrawable>(500);

        public MeshRenderables OpaqueRenderables { get; private set; } = new MeshRenderables();

        private object SceneUpdateLock = new object();

        public RenderFilter DrawFilter { get; set; } = RenderFilter.All;

        public DrawGroup DisplayGroup { get; set; } = new DrawGroup();

        // Statistics
        public int RenderObjectCount { get; private set; } = 0;
        public float OctreeCullTime { get; private set; } = 0.0f;
        public float CPUDrawTime { get; private set; } = 0.0f;

        public void ToggleDrawFilter(RenderFilter toggle)
        {
            if ((DrawFilter & toggle) > 0)
            {
                DrawFilter &= ~toggle;
            }
            else
            {
                DrawFilter |= toggle;
            }
        }

        private float RCDist = float.PositiveInfinity;
        private int RayCastFilter(Ray ray, IDrawable d, List<RayCastHit<IDrawable>> hits)
        {
            var bb = d.GetBounds();
            //var dist = (ray.Origin - bb.GetCenter()).Length();
            float dist;
            //if (ray.Intersects(bb) && dist < RCDist)
            if ((((d.DrawFilter & DrawFilter) > 0 && d.DrawGroups.IsInDisplayGroup(DisplayGroup) && d.IsVisible) || d.Highlighted)
                && ray.Intersects(bb) && d.RayCast(ray, out dist) && dist < RCDist)
            {
                RCDist = dist;
                hits.Clear();
                hits.Add(new RayCastHit<IDrawable>(d, ray.Origin + ray.Direction * dist, dist));
                return 1;
            }
            return 0;
        }

        public IDrawable CastRay(Ray ray)
        {
            /*var hits = new List<RayCastHit<IDrawable>>();
            RCDist = float.PositiveInfinity;
            lock (SceneUpdateLock)
            {
                Octree.RayCast(ray, hits, RayCastFilter);
            }
            if (hits.Count() > 0)
            {
                return hits[0].Item;
            }*/
            return null;
        }

        public void Render(Renderer.RenderQueue queue, BoundingFrustum frustum, SceneRenderPipeline pipeline)
        {
            /*CulledObjects.Clear();
            lock (SceneUpdateLock)
            {
                var watch = Stopwatch.StartNew();
                Octree.ApplyPendingMoves();
                Octree.GetContainedObjects(frustum, CulledObjects);
                watch.Stop();
                OctreeCullTime = (float)(((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0);
            }
            RenderObjectCount = CulledObjects.Count();
            var watch2 = Stopwatch.StartNew();
            foreach (var obj in CulledObjects)
            {
                if (((obj.DrawFilter & DrawFilter) > 0 && obj.DrawGroups.IsInDisplayGroup(DisplayGroup) && obj.IsVisible)
                    || obj.Highlighted)
                {
                    obj.SubmitRenderObjects(queue);
                }
            }
            watch2.Stop();
            CPUDrawTime = (float)(((double)watch2.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0);*/
            var watch = Stopwatch.StartNew();
            OpaqueRenderables.CullRenderables(frustum);
            watch.Stop();
            OctreeCullTime = (float)(((double)watch.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0);

            var watch2 = Stopwatch.StartNew();
            OpaqueRenderables.SubmitRenderables(queue);
            watch2.Stop();
            CPUDrawTime = (float)(((double)watch2.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0);

            queue.SetDrawParameters(OpaqueRenderables.cDrawParameters);
        }
    }
}
