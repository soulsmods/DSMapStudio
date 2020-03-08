using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace StudioCore.Scene
{
    public abstract class RenderObject : IDisposable
    {
        public abstract void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        //public abstract void Render(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void Render(Renderer.IndirectDrawEncoder encoder, SceneRenderPipeline sp);
        public abstract void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneRenderPipeline sp);
        public abstract void DestroyDeviceObjects();

        public abstract Pipeline GetPipeline();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                DestroyDeviceObjects();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~RenderObject()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
