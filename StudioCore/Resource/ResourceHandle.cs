using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private string AssetVirtualPath = null;

        private object LoadingLock = new object();
        private object HandlerLock = new object();
        private object AcquireFreeLock = new object();

        private int ReferenceCount = 0;

        public bool IsLoaded { get; private set; } = false;

        public AccessLevel AccessLevel { get; private set; } = AccessLevel.AccessUnloaded;

        private T Resource = null;

        //private List<Action<ResourceHandle<T>>> LoadCompletionHandlers = new List<Action<ResourceHandle<T>>>();
        //private List<Action<ResourceHandle<T>>> UnloadCompletionHandlers = new List<Action<ResourceHandle<T>>>();

        private List<WeakReference<IResourceEventListener>> EventListeners = new List<WeakReference<IResourceEventListener>>();

        private int LockCounter = 0;
        private object ResourceLock = new object();

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
                Resource._Load(data, al, type);
                // Prevent any new completion handlers from being added while executing them all
                // Any subsequent pending handlers will be executed after this is done
                lock (HandlerLock)
                {
                    IsLoaded = true;
                    foreach (var listener in EventListeners)
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
                    Resource._Load(file, al, type);
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
                    foreach (var listener in EventListeners)
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
            lock (HandlerLock)
            {
                if (IsLoaded)
                {
                    listener.OnResourceLoaded(this);
                }
                if (!IsLoaded)
                {
                    listener.OnResourceUnloaded(this);
                }
                EventListeners.Add(new WeakReference<IResourceEventListener>(listener));
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
                        }
                    }
                }
                
                foreach (var listener in EventListeners)
                {
                    IResourceEventListener l;
                    bool succ = listener.TryGetTarget(out l);
                    if (succ)
                    {
                        l.OnResourceUnloaded(this);
                    }
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
                if (ReferenceCount <= 0 && IsLoaded)
                {
                    unload = true;
                }
                if (ReferenceCount <= 0)
                {
                    ReferenceCount = 0;
                }
            }
            if (unload)
            {
                Unload();
            }
        }

        public override string ToString()
        {
            return AssetVirtualPath;
        }
    }
}
