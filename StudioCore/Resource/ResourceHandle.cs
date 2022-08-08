using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoulsFormats;

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
        public string AssetVirtualPath { get; }

        public AccessLevel AccessLevel { get; }

        public bool _LoadResource(byte[] data, AccessLevel al, GameType type);
        public bool _LoadResource(string file, AccessLevel al, GameType type);

        public int GetReferenceCounts();
        public void Acquire();
        public void Release();

        /// <summary>
        /// Tries to lock the resource if it is loaded. While locked the resource can't be
        /// unloaded.
        /// </summary>
        /// <returns>True if the resource is successfully locked</returns>
        public bool TryLock();

        public void Unlock();

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
        public string AssetVirtualPath { get; private set; }

        protected object LoadingLock = new object();
        protected object HandlerLock = new object();
        protected object AcquireFreeLock = new object();

        protected int ReferenceCount = 0;

        public bool IsLoaded { get; protected set; } = false;

        public AccessLevel AccessLevel { get; protected set; } = AccessLevel.AccessUnloaded;

        protected T Resource = null;

        protected List<WeakReference<IResourceEventListener>> EventListeners = new List<WeakReference<IResourceEventListener>>();

        protected int LockCounter = 0;
        protected object ResourceLock = new object();

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

        public bool TryLock()
        {
            if (!IsLoaded)
            {
                return false;
            }
            lock (ResourceLock)
            {
                if (IsLoaded)
                {
                    LockCounter++;
                    return true;
                }
            }
            return false;
        }

        public void Unlock()
        {
            lock (ResourceLock)
            {
                LockCounter--;
            }
        }

        bool IResourceHandle._LoadResource(byte[] data, AccessLevel al, GameType type)
        {
            lock (LoadingLock)
            {
                if (IsLoaded)
                {
                    Unload();
                }
                AccessLevel = AccessLevel.AccessLoading;
                Resource = new T();
                if (!Resource._Load(data, al, type))
                {
                    AccessLevel = AccessLevel.AccessUnloaded;
                    return false;
                }
                // Prevent any new completion handlers from being added while executing them all
                // Any subsequent pending handlers will be executed after this is done
                WeakReference<IResourceEventListener>[] listeners;
                lock (HandlerLock)
                {
                    IsLoaded = true;
                    listeners = EventListeners.ToArray();
                }
                foreach (var listener in listeners)
                {
                    try
                    {
                        IResourceEventListener l;
                        bool succ = listener.TryGetTarget(out l);
                        if (succ)
                        {
                            l.OnResourceLoaded(this);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("blah");
                    }
                }
                AccessLevel = al;
            }
            return true;
        }

        bool IResourceHandle._LoadResource(string file, AccessLevel al, GameType type)
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
                    if (!Resource._Load(file, al, type))
                    {
                        AccessLevel = AccessLevel.AccessUnloaded;
                        return false;
                    }
                }
                catch (System.IO.FileNotFoundException)
                {
                    Resource = null;
                    AccessLevel = AccessLevel.AccessUnloaded;
                    return false;
                }
                // Prevent any new completion handlers from being added while executing them all
                // Any subsequent pending handlers will be executed after this is done
                WeakReference<IResourceEventListener>[] listeners;
                lock (HandlerLock)
                {
                    IsLoaded = true;
                    listeners = EventListeners.ToArray();
                }
                foreach (var listener in listeners)
                {
                    try
                    {
                        IResourceEventListener l;
                        bool succ = listener.TryGetTarget(out l);
                        if (succ)
                        {
                            l.OnResourceLoaded(this);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("blah");
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
        public void AddResourceEventListener(IResourceEventListener listener)
        {
            // Prevent modification of loading status while doing this check
            bool listenLoad = false;
            bool listenUnload = false;
            lock (HandlerLock)
            {
                if (IsLoaded)
                {
                    listenLoad = true;
                }
                if (!IsLoaded)
                {
                    listenUnload = true;
                }
                EventListeners.Add(new WeakReference<IResourceEventListener>(listener));
            }
            if (listenLoad)
            {
                listener.OnResourceLoaded(this);
            }
            if (listenUnload)
            {
                listener.OnResourceUnloaded(this);
            }
        }

        /// <summary>
        /// Unloads the resource from memory by disposing it, assuming there are no other references to the
        /// underlying resource
        /// </summary>
        public void Unload()
        {
            // Make sure any outstanding handlers are added before changing
            WeakReference<IResourceEventListener>[] listeners;
            lock (HandlerLock)
            {
                bool spin = true;
                while (spin)
                {
                    // Wait until the resource isn't locked
                    while (LockCounter > 0) ;
                    lock (ResourceLock)
                    {
                        if (LockCounter <= 0)
                        {
                            spin = false;
                            IsLoaded = false;
                            AccessLevel = AccessLevel.AccessUnloaded;
                        }
                    }
                }

                listeners = EventListeners.ToArray();
            }
            foreach (var listener in listeners)
            {
                IResourceEventListener l;
                bool succ = listener.TryGetTarget(out l);
                if (succ)
                {
                    l.OnResourceUnloaded(this);
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

        public int GetReferenceCounts()
        {
            return ReferenceCount;
        }

        public void Acquire()
        {
            lock (AcquireFreeLock)
            {
                ReferenceCount++;
            }
        }

        public void Release()
        {
            bool unload = false;
            lock (AcquireFreeLock)
            {
                ReferenceCount--;
                if (ReferenceCount == 0 && IsLoaded)
                {
                    unload = true;
                }
                if (ReferenceCount < 0)
                {
                    throw new Exception($@"Resource {AssetVirtualPath} reference count already 0");
                }
            }
            if (unload)
            {
                Unload();
            }
        }

        /// <summary>
        /// Constructs a temporary handle for a resource. This is useful for creating "fake"
        /// resources that aren't serialized and registered with the resource management system
        /// yet such as previews for freshly imported models
        /// </summary>
        /// <param name="res">The resource to create a handle from</param>
        /// <returns></returns>
        public static ResourceHandle<T> TempHandleFromResource(T res)
        {
            var ret = new ResourceHandle<T>("temp");
            ret.AccessLevel = AccessLevel.AccessFull;
            ret.IsLoaded = true;
            ret.Resource = res;
            return ret;
        }

        public override string ToString()
        {
            return AssetVirtualPath;
        }
    }

    public class TextureResourceHande : ResourceHandle<TextureResource>
    {
        public TextureResourceHande(string virtualPath) : base(virtualPath)
        {
        }

        public bool _LoadTextureResource(TPF tex, int index, AccessLevel al, GameType type)
        {
            lock (LoadingLock)
            {
                if (IsLoaded)
                {
                    Unload();
                }
                AccessLevel = AccessLevel.AccessLoading;
                Resource = new TextureResource(tex, index);
                Resource._LoadTexture(al);
                // Prevent any new completion handlers from being added while executing them all
                // Any subsequent pending handlers will be executed after this is done
                WeakReference<IResourceEventListener>[] listeners;
                lock (HandlerLock)
                {
                    IsLoaded = true;
                    listeners = EventListeners.ToArray();
                }
                foreach (var listener in listeners)
                {
                    try
                    {
                        IResourceEventListener l;
                        bool succ = listener.TryGetTarget(out l);
                        if (succ)
                        {
                            l.OnResourceLoaded(this);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("blah");
                    }
                }
                AccessLevel = al;
            }
            return true;
        }
    }
}
