using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.Scene
{
    public abstract class RenderObject
    {
        public abstract void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void Render(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void DestroyDeviceObjects();

        public abstract Pipeline GetPipeline();

        public void Dispose()
        {
            DestroyDeviceObjects();
        }
    }
}
