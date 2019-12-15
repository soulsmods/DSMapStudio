using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Veldrid.Utilities;

namespace StudioCore.Scene
{
    /// <summary>
    /// A drawable is an object that is capable of creating and submiting rendering
    /// work to the renderer
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// World space matrix the drawable is drawn at
        /// </summary>
        public Matrix4x4 WorldMatrix { get; set; }

        /// <summary>
        /// A selectable object that is attached to this scene object and can be selected
        /// with a raycast
        /// </summary>
        public ISelectable Selectable { get; set; }

        /// <summary>
        /// Render mask of this object. Used to hide objects of certain classes from the scene
        /// </summary>
        public RenderFilter DrawFilter { get; set; }

        /// <summary>
        /// The drawable is highlighted. 
        /// </summary>
        public bool Highlighted { get; set; }

        /// <summary>
        /// Enables automatic registration of the object with the scene when certain conditions
        /// are met, such as a required resource is loaded. Not applicable for every drawable
        /// </summary>
        public bool AutoRegister { get; set; }

        /// <summary>
        /// Invoke a manual registration of the object with the renderscene, which adds it
        /// to be rendered
        /// </summary>
        /// <param name="scene">The scene to register with</param>
        public void RegisterWithScene(RenderScene scene);

        /// <summary>
        /// Unregister this object from the scene, effectively removing it
        /// </summary>
        public void UnregisterWithScene();

        public void SubmitRenderObjects(Scene.Renderer.RenderQueue queue);
        public BoundingBox GetBounds();

        public bool RayCast(Ray ray, out float dist);
    }
}
