using System;
using System.Collections.Generic;
using System.Text;
using Veldrid;
using Veldrid.Utilities;

namespace StudioCore.Scene
{
    /// <summary>
    /// Contains if the component is visible/valid. Valid means that there's an actual
    /// renderable in this slot, while visible means it should be rendered
    /// </summary>
    public struct VisibleValidComponent
    {
        public bool _valid;
        public bool _visible;
    }

    /// <summary>
    /// All the components needed by the indirect draw encoder to render a mesh.
    /// Batch them all together for locality
    /// </summary>
    public struct MeshDrawParametersComponent
    {
        public Renderer.IndirectDrawIndexedArgumentsPacked _indirectArgs;
        public ResourceSet _objectResourceSet;
        public IndexFormat _indexFormat;
        public int _bufferIndex;
    }

    public struct SceneVisibilityComponent
    {
        public RenderFilter _renderFilter;
        public DrawGroup _drawGroup;
    }

    /// <summary>
    /// Data oriented structure that contains renderables. This is basically a structure
    /// of arrays intended of containing all the renderables for a certain mesh. Management
    /// of how this is populated is left to a higher level system
    /// </summary>
    public class Renderables
    {
        public const int RENDERABLES_SEGMENT_SIZE = 10000;
        private int _topIndex = 0;

        public int RenderableSystemIndex { get; protected set; }

        /// <summary>
        /// Component for if the renderable is visible or active
        /// </summary>
        public SegmentedArrayList<VisibleValidComponent> cVisible = new SegmentedArrayList<VisibleValidComponent>(RENDERABLES_SEGMENT_SIZE);
        public SegmentedArrayList<SceneVisibilityComponent> cSceneVis = new SegmentedArrayList<SceneVisibilityComponent>(RENDERABLES_SEGMENT_SIZE);
        public SegmentedArrayList<RenderKey> cRenderKeys = new SegmentedArrayList<RenderKey>(RENDERABLES_SEGMENT_SIZE);
        protected int GetNextInvalidIndex()
        {
            return cVisible.IndexOf((vvc)=>!vvc._valid, true);
        }

        protected int AllocateValidAndVisibleRenderable()
        {
            int next = GetNextInvalidIndex();
            cVisible[next]._valid = true;
            cVisible[next]._visible = true;
            cRenderKeys[next] = new RenderKey(0);
            cSceneVis[next]._renderFilter = RenderFilter.All;
            cSceneVis[next]._drawGroup = new DrawGroup();
            return next;
        }

        public void RemoveRenderable(int renderable)
        {
            cVisible[renderable]._valid = false;
        }
    }

    /// <summary>
    /// Structure for mesh renderables and all the information needed to render a single static mesh
    /// </summary>
    public class MeshRenderables : Renderables
    {
        public SegmentedArrayList<BoundingBox> cBounds = new SegmentedArrayList<BoundingBox>(RENDERABLES_SEGMENT_SIZE);
        public SegmentedArrayList<MeshDrawParametersComponent> cDrawParameters = new SegmentedArrayList<MeshDrawParametersComponent>(RENDERABLES_SEGMENT_SIZE);
        public SegmentedArrayList<bool> cCulled = new SegmentedArrayList<bool>(RENDERABLES_SEGMENT_SIZE);
        public SegmentedArrayList<Pipeline> cPipelines = new SegmentedArrayList<Pipeline>(RENDERABLES_SEGMENT_SIZE);

        public SegmentedArrayList<Pipeline> cSelectionPipelines = new SegmentedArrayList<Pipeline>(RENDERABLES_SEGMENT_SIZE);
        public SegmentedArrayList<WeakReference<ISelectable>> cSelectables = new SegmentedArrayList<WeakReference<ISelectable>>(RENDERABLES_SEGMENT_SIZE);

        public MeshRenderables(int id)
        {
            RenderableSystemIndex = id;
        }

        public int CreateMesh(ref BoundingBox bounds, ref MeshDrawParametersComponent drawArgs)
        {
            int idx = AllocateValidAndVisibleRenderable();
            cBounds[idx] = bounds;
            cDrawParameters[idx] = drawArgs;
            return idx;
        }

        public void CullRenderables(BoundingFrustum frustum)
        {
            for (int i = 0; i < cVisible.Size; i++)
            {
                if (!cVisible[i]._valid)
                {
                    continue;
                }
                var intersect = frustum.Contains(ref cBounds[i]);
                if (intersect == ContainmentType.Contains || intersect == ContainmentType.Intersects)
                {
                    cCulled[i] = !cVisible[i]._valid || !cVisible[i]._visible;
                }
                else
                {
                    cCulled[i] = true;
                }
            }
        }

        public void ProcessSceneVisibility(RenderFilter filter, DrawGroup dispGroup)
        {
            bool alwaysVis = dispGroup != null ? dispGroup.AlwaysVisible : true;
            for (int i = 0; i < cVisible.Size; i++)
            {
                if (cCulled[i])
                {
                    continue;
                }

                if ((cSceneVis[i]._renderFilter & filter) == 0)
                {
                    cCulled[i] = true;
                    continue;
                }

                if (!alwaysVis && cSceneVis[i]._drawGroup != null && !cSceneVis[i]._drawGroup.IsInDisplayGroup(dispGroup))
                {
                    cCulled[i] = true;
                }
            }
        }

        public void SubmitRenderables(Renderer.RenderQueue queue)
        {
            for (int i = 0; i < cVisible.Size; i++)
            {
                if (cVisible[i]._valid && !cCulled[i])
                {
                    queue.Add(i, cRenderKeys[i]);
                }
            }
        }
    }
}
