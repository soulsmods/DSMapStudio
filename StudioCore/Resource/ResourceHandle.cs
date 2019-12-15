using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioCore.Resource
{
    /// <summary>
    /// Requested access level to a given resource
    /// </summary>
    public enum AccessLevel
    {
        /// <summary>
        /// Asset is not loaded
        /// </summary>
        AccessUnloaded,

        /// <summary>
        /// Asset is loading
        /// </summary>
        AccessLoading,

        /// <summary>
        /// Access to this resource is intended for low level editing only
        /// </summary>
        AccessEditOnly,

        /// <summary>
        /// Access to this resource is intended to be from the GPU in an optimized form
        /// only, and is not intended to be mutated or read from the CPU. Used for textures
        /// and models that aren't being edited primarily.
        /// </summary>
        AccessGPUOptimizedOnly,

        /// <summary>
        /// This resource is intended to be accessed by both the GPU and accessed/modified
        /// by the CPU.
        /// </summary>
        AccessFull,
    }

    public interface IResourceHandle
    {
        public bool _LoadResource(byte[] data, AccessLevel al);
        public bool _LoadResource(string file, AccessLevel al);

        public bool IsLoaded();
    }

    /// <summary>
    /// A handle to a resource, which may or may not be loaded.
    /// </summary>
    /// <typeparam name="T">The resource that is wrapped by this handler</typeparam>
    public class ResourceHandle <T> : IResourceHandle where T : class, IResource, IDisposable, new()
    {
        /// <summary>
        /// Virtual path of the entire asset. Used to implement loading
        /// </summary>
        private string AssetVirtualPath = null;

        private object LoadingLock = new object();
        private object HandlerLock = new object();

        public bool IsLoaded { get; private set; } = false;

        public AccessLevel AccessLevel { get; private set; } = AccessLevel.AccessUnloaded;

        private T Resource = null;

        private List<Action<ResourceHandle<T>>> LoadCompletionHandlers = new List<Action<ResourceHandle<T>>>();
        private List<Action<ResourceHandle<T>>> UnloadCompletionHandlers = new List<Action<ResourceHandle<T>>>();

        public ResourceHandle(string virtualPath)
        {
            AssetVirtualPath = virtualPath;
        }


        public T Get()
        {
            return Resource;
        }

        public void LoadBlocking(AccessLevel al)
        {

        }

        public void LoadAsync(AccessLevel al)
        {

        }

        bool IResourceHandle._LoadResource(byte[] data, AccessLevel al)
        {
            lock (LoadingLock)
            {
                if (IsLoaded)
                {
                    Unload();
                }
                AccessLevel = AccessLevel.AccessLoading;
                Resource = new T();
                Resource._Load(data, al);
                // Prevent any new completion handlers from being added while executing them all
                // Any subsequent pending handlers will be executed after this is done
                lock (HandlerLock)
                {
                    IsLoaded = true;
                    foreach (var handle in LoadCompletionHandlers)
                    {
                        try
                        {
                            handle.Invoke(this);
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine("blah");
                        }
                    }
                }
                AccessLevel = al;
            }
            return true;
        }

        bool IResourceHandle._LoadResource(string file, AccessLevel al)
        {
            lock (LoadingLock)
            {
                if (IsLoaded)
                {
                    Unload();
                }
                AccessLevel = AccessLevel.AccessLoading;
                Resource = new T();
                try
                {
                    Resource._Load(file, al);
                }
                catch (System.IO.FileNotFoundException)
                {
                    Resource = null;
                    AccessLevel = AccessLevel.AccessUnloaded;
                    return false;
                }
                // Prevent any new completion handlers from being added while executing them all
                // Any subsequent pending handlers will be executed after this is done
                lock (HandlerLock)
                {
                    IsLoaded = true;
                    foreach (var handle in LoadCompletionHandlers)
                    {
                        handle.Invoke(this);
                    }
                }
                AccessLevel = al;
                return true;
            }
        }

        /// <summary>
        /// Adds a handler that is called every time this resource is loaded. If the resource
        /// is loaded at the time this handler is added, the handler is called immediately
        /// To prevent deadlock, these handlers should not trigger a load/unload of the resource
        /// </summary>
        /// <param name="handler"></param>
        public void AddResourceLoadedHandler(Action<ResourceHandle<T>> handler)
        {
            // Prevent modification of loading status while doing this check
            lock (HandlerLock)
            {
                if (IsLoaded)
                {
                    handler.Invoke(this);
                }
                LoadCompletionHandlers.Add(handler);
            }
        }

        public void AddResourceUnloadedHandler(Action<ResourceHandle<T>> handler)
        {
            // Prevent modification of loading status while doing this check
            lock (HandlerLock)
            {
                if (!IsLoaded)
                {
                    handler.Invoke(this);
                }
                UnloadCompletionHandlers.Add(handler);
            }
        }

        /// <summary>
        /// Unloads the resource from memory by disposing it, assuming there are no other references to the
        /// underlying resource
        /// </summary>
        public void Unload()
        {
            // Make sure any outstanding handlers are added before changing
            lock (HandlerLock)
            {
                IsLoaded = false;
                foreach (var handler in UnloadCompletionHandlers)
                {
                    handler.Invoke(this);
                }
            }
            var handle = Resource;
            Resource = null;
            handle.Dispose();
        }

        bool IResourceHandle.IsLoaded()
        {
            return IsLoaded;
        }
    }
}
