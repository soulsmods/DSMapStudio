using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks.Dataflow;
using SoulsFormats;
using Veldrid;

namespace StudioCore.Resource
{
    public class TextureResource : IResource, IDisposable
    {
        private class TextureLoadPipeline : IResourceLoadPipeline
        {
            public ITargetBlock<LoadByteResourceRequest> LoadByteResourceBlock => throw new NotImplementedException();
            public ITargetBlock<LoadFileResourceRequest> LoadFileResourceRequest => throw new NotImplementedException();

            public ITargetBlock<LoadTPFTextureResourceRequest> LoadTPFTextureResourceRequest =>
                _loadTPFResourcesTransform;

            private ActionBlock<LoadTPFTextureResourceRequest> _loadTPFResourcesTransform;

            private ITargetBlock<IResourceHandle> _loadedResources = null;

            public TextureLoadPipeline(ITargetBlock<IResourceHandle> target)
            {
                _loadedResources = target;
                _loadTPFResourcesTransform = new ActionBlock<LoadTPFTextureResourceRequest>(r =>
                {
                    var res = new TextureResourceHandle(r.virtualPath);
                    bool success = res._LoadTextureResource(r.tpf, r.index, r.AccessLevel, r.GameType);
                    if (success)
                    {
                        _loadedResources.Post(res);
                    }
                });
            }
        }

        public static IResourceLoadPipeline CreatePipeline(ITargetBlock<IResourceHandle> target)
        {
            return new TextureLoadPipeline(target);
        }
        
        public TPF Texture { get; private set; } = null;
        private int TPFIndex = 0;

        public Scene.TexturePool.TextureHandle GPUTexture { get; private set; } = null;

        public TextureResource()
        {
            throw new Exception("Created wrong");
        }

        public TextureResource(TPF tex, int index)
        {
            Texture = tex;
            TPFIndex = index;
        }

        public bool _LoadTexture(AccessLevel al)
        {
            if (Scene.TexturePool.TextureHandle.IsTPFCube(Texture.Textures[TPFIndex], Texture.Platform))
            {
                GPUTexture = Scene.Renderer.GlobalCubeTexturePool.AllocateTextureDescriptor();
            }
            else
            {
                GPUTexture = Scene.Renderer.GlobalTexturePool.AllocateTextureDescriptor();
            }
            if (GPUTexture == null)
            {
                if (FeatureFlags.StrictResourceChecking)
                    throw new Exception("Unable to allocate texture descriptor");
                return false;
            }
            if (Texture.Platform == TPF.TPFPlatform.PC || Texture.Platform == TPF.TPFPlatform.PS3)
            {
                Scene.Renderer.AddLowPriorityBackgroundUploadTask((d, cl) =>
                {
                    if (GPUTexture == null)
                        return;
                    
                    GPUTexture.FillWithTPF(d, cl, Texture.Platform, Texture.Textures[TPFIndex], Texture.Textures[TPFIndex].Name);
                    Texture = null;
                });
            }
            else if (Texture.Platform == TPF.TPFPlatform.PS4)
            {
                Scene.Renderer.AddLowPriorityBackgroundUploadTask((d, cl) =>
                {
                    if (GPUTexture == null)
                        return;
                    
                    GPUTexture.FillWithPS4TPF(d, cl, Texture.Platform, Texture.Textures[TPFIndex], Texture.Textures[TPFIndex].Name);
                    Texture = null;
                });
            }
            return true;
        }

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

                GPUTexture?.Dispose();
                GPUTexture = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~TextureResource()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool IResource._Load(byte[] bytes, AccessLevel al, GameType type)
        {
            return _LoadTexture(al);
        }

        bool IResource._Load(string file, AccessLevel al, GameType type)
        {
            return _LoadTexture(al);
        }
        #endregion
    }
}
