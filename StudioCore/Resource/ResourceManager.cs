﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Threading;
using System.Numerics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using SoulsFormats;
using ImGuiNET;

namespace StudioCore.Resource
{
    /// <summary>
    /// Manages resources (mainly GPU) such as textures and models, and can be used to unload and reload them at will.
    /// A background thread will manage the unloading and streaming in of assets. This is designed to map closely to
    /// the souls asset system, but in a more abstract way
    /// </summary>
    public static class ResourceManager
    {
        private static QueuedTaskScheduler JobScheduler = new QueuedTaskScheduler(4, "JobMaster");
        private static TaskFactory JobTaskFactory = new TaskFactory(JobScheduler);

        [Flags]
        public enum ResourceType
        {
            Flver = 1,
            Texture = 2,
            CollisionHKX = 4,
            Navmesh = 8,
            NavmeshHKX = 16,
            All = 0xFFFFFFF,
        }

        private static IResourceHandle InstantiateResource(ResourceType type, string path)
        {
            switch (type)
            {
                case ResourceType.Flver:
                    return new ResourceHandle<FlverResource>(path);
                //case ResourceType.Texture:
                //    return new ResourceHandle<TextureResource>(path);
            }
            return null;
        }

        public interface IResourceTask
        {
            public void Run();
            public Task RunAsync(IProgress<int> progress);

            /// <summary>
            /// Get an estimate of the size of a task (i.e. how many files to load)
            /// </summary>
            /// <returns></returns>
            public int GetEstimateTaskSize();
        }

        internal struct LoadResourceFromBytesAction
        {
            public IResourceHandle handle = null;
            public byte[] bytes = null;
            public AccessLevel AccessLevel = AccessLevel.AccessGPUOptimizedOnly;
            public GameType Game;
            public LoadResourceFromBytesAction(IResourceHandle handle, byte[] bytes, AccessLevel al, GameType type)
            {
                this.handle = handle;
                this.bytes = bytes;
                this.AccessLevel = al;
                Game = type;
            }
        }

        internal static IResourceHandle LoadResourceFromBytes(LoadResourceFromBytesAction action)
        {
            var ctx = Tracy.TracyCZoneN(1, "LoadResourceFromBytesTask::Run");
            action.handle._LoadResource(action.bytes, action.AccessLevel, action.Game);
            action.bytes = null;
            Tracy.TracyCZoneEnd(ctx);
            return action.handle;
        }

        internal struct LoadResourcesFromFileAction
        {
            public IResourceHandle handle = null;
            public string file = null;
            public AccessLevel AccessLevel = AccessLevel.AccessGPUOptimizedOnly;
            public GameType Game;
            public LoadResourcesFromFileAction(IResourceHandle handle, string file, AccessLevel al, GameType type)
            {
                this.handle = handle;
                this.file = file;
                this.AccessLevel = al;
                this.Game = type;
            }
        }
        
        internal static IResourceHandle LoadResourceFromFile(LoadResourcesFromFileAction action)
        {
            var ctx = Tracy.TracyCZoneN(1, $@"LoadResourceFromFileTask::Run {action.file}");
            action.handle._LoadResource(action.file, action.AccessLevel, action.Game);
            Tracy.TracyCZoneEnd(ctx);
            return action.handle;
        }

        internal struct LoadTPFResourcesAction
        {
            public ResourceJob _job;
            public string _virtpathbase = null;
            public TPF _tpf = null;
            public AccessLevel _accessLevel = AccessLevel.AccessGPUOptimizedOnly;
            public GameType _game;
            public List<Tuple<TextureResourceHande, string, int>> _pendingResources = new List<Tuple<TextureResourceHande, string, int>>();

            public LoadTPFResourcesAction (ResourceJob job, string virtpathbase, TPF tpf, AccessLevel al, GameType type)
            {
                _job = job;
                _virtpathbase = virtpathbase;
                _tpf = tpf;
                _accessLevel = al;
                _game = type;
            }
        }

        private static IResourceHandle[] LoadTPFResources(LoadTPFResourcesAction action)
        {
            var ctx = Tracy.TracyCZoneN(1, $@"LoadTPFResourcesTask::Run {action._virtpathbase}");
            if (!CFG.Current.EnableTexturing)
            {
                action._pendingResources.Clear();
                action._tpf = null;
                Tracy.TracyCZoneEnd(ctx);
                return new IResourceHandle[]{};
            }

            action._job.IncrementEstimateTaskSize(action._tpf.Textures.Count);
            var ret = new IResourceHandle[action._tpf.Textures.Count];
            for (int i = 0; i < action._tpf.Textures.Count; i++)
            {
                var tex = action._tpf.Textures[i];
                var handle = ResourceManager.GetTextureResource($@"{action._virtpathbase}/{tex.Name.ToLower()}");
                action._pendingResources.Add(new Tuple<TextureResourceHande, string, int>(handle, $@"{action._virtpathbase}/{tex.Name}", i));
                ret[i] = handle;
            }

            foreach (var t in action._pendingResources)
            {
                t.Item1._LoadTextureResource(action._tpf, t.Item3, action._accessLevel, action._game);
            }
            action._pendingResources.Clear();
            action._tpf = null;
            Tracy.TracyCZoneEnd(ctx);
            return ret;
        }

        internal class LoadBinderResourcesAction
        {
            public ResourceJob _job;
            public string BinderVirtualPath = null;
            public BinderReader Binder = null;
            public bool PopulateResourcesOnly = false;
            public HashSet<int> BinderLoadMask = null;
            public List<Task> LoadingTasks = new List<Task>();
            public List<int> TaskSizes = new List<int>();
            public List<int> TaskProgress = new List<int>();
            public int TotalSize = 0;
            public HashSet<string> AssetWhitelist = null;
            public ResourceType ResourceMask = ResourceType.All;
            public AccessLevel AccessLevel = AccessLevel.AccessGPUOptimizedOnly;

            public List<Tuple<IResourceHandle, string, BinderFileHeader>> PendingResources = new List<Tuple<IResourceHandle, string, BinderFileHeader>>();
            public List<Tuple<string, BinderFileHeader>> PendingTPFs = new List<Tuple<string, BinderFileHeader>>();

            public readonly object ProgressLock = new object();

            public LoadBinderResourcesAction(ResourceJob job, string virtpath, AccessLevel accessLevel, bool populateOnly, ResourceType mask, HashSet<string> whitelist)
            {
                _job = job;
                BinderVirtualPath = virtpath;
                PopulateResourcesOnly = populateOnly;
                ResourceMask = mask;
                AssetWhitelist = whitelist;
                AccessLevel = accessLevel;
            }

            public void ProcessBinder()
            {
                if (Binder == null)
                {
                    string o;
                    var path = ResourceManager.Locator.VirtualToRealPath(BinderVirtualPath, out o);
                    Binder = InstantiateBinderReaderForFile(path, ResourceManager.Locator.Type);
                    if (Binder == null)
                    {
                        return;
                    }
                }

                for (int i = 0; i < Binder.Files.Count(); i++)
                {
                    var f = Binder.Files[i];
                    if (BinderLoadMask != null && !BinderLoadMask.Contains(i))
                    {
                        continue;
                    }
                    var binderpath = f.Name;
                    var filevirtpath = ResourceManager.Locator.GetBinderVirtualPath(BinderVirtualPath, binderpath);
                    if (AssetWhitelist != null && !AssetWhitelist.Contains(filevirtpath))
                    {
                        continue;
                    }
                    IResourceHandle handle = null;
                    if (filevirtpath.ToUpper().EndsWith(".TPF") || filevirtpath.ToUpper().EndsWith(".TPF.DCX"))
                    {
                        string virt = BinderVirtualPath;
                        if (virt.StartsWith($@"map/tex"))
                        {
                            var regex = new Regex(@"\d{4}$");
                            if (regex.IsMatch(virt))
                            {
                                virt = virt.Substring(0, virt.Length - 5);
                            }
                            else if (virt.EndsWith("tex"))
                            {
                                virt = virt.Substring(0, virt.Length - 4);
                            }
                        }
                        PendingTPFs.Add(new Tuple<string, BinderFileHeader>(virt, f));
                    }
                    else
                    {
                        if (ResourceMask.HasFlag(ResourceType.Flver) &&
                            (filevirtpath.ToUpper().EndsWith(".FLVER") ||
                             filevirtpath.ToUpper().EndsWith(".FLV") ||
                             filevirtpath.ToUpper().EndsWith(".FLV.DCX")))
                        {
                            //handle = new ResourceHandle<FlverResource>();
                            handle = ResourceManager.GetResource<FlverResource>(filevirtpath);
                        }
                        else if (ResourceMask.HasFlag(ResourceType.CollisionHKX) &&
                            (filevirtpath.ToUpper().EndsWith(".HKX") ||
                             filevirtpath.ToUpper().EndsWith(".HKX.DCX")))
                        {
                            handle = ResourceManager.GetResource<HavokCollisionResource>(filevirtpath);
                        }
                        else if (ResourceMask.HasFlag(ResourceType.Navmesh) && filevirtpath.ToUpper().EndsWith(".NVM"))
                        {
                            handle = ResourceManager.GetResource<NVMNavmeshResource>(filevirtpath);
                        }
                        else if (ResourceMask.HasFlag(ResourceType.NavmeshHKX) &&
                            (filevirtpath.ToUpper().EndsWith(".HKX") ||
                             filevirtpath.ToUpper().EndsWith(".HKX.DCX")))
                        {
                            handle = ResourceManager.GetResource<HavokNavmeshResource>(filevirtpath);
                        }

                        if (handle != null)
                        {
                            PendingResources.Add(new Tuple<IResourceHandle, string, BinderFileHeader>(handle, filevirtpath, f));
                        }
                    }
                }
            }
        }

        private static void LoadBinderResources(LoadBinderResourcesAction action)
        {
            action.ProcessBinder();
            if (!action.PopulateResourcesOnly)
            {
                bool doasync = (action.PendingResources.Count() + action.PendingTPFs.Count()) > 1;
                int i = 0;
                foreach (var p in action.PendingResources)
                {
                    var f = action.Binder.ReadFile(p.Item3);
                    action._job.AddLoadByteResources(new LoadResourceFromBytesAction(p.Item1, f, action.AccessLevel,
                        ResourceManager.Locator.Type));
                    action._job.IncrementEstimateTaskSize(1);
                    i++;
                }

                foreach (var t in action.PendingTPFs)
                {
                    var f = TPF.Read(action.Binder.ReadFile(t.Item2));
                    action._job.AddLoadTPFResources(new LoadTPFResourcesAction(action._job, t.Item1, f, action.AccessLevel, ResourceManager.Locator.Type));
                    i++;
                }
            }

            action.PendingResources.Clear();
            action.Binder = null;
        }

        /// <summary>
        /// A named job that runs many tasks and whose progress will appear in the progress window
        /// </summary>
        public class ResourceJob
        {
            public string Name { get; private set; } = null;
            private int _courseSize = 0;
            private int TotalSize = 0;
            public int Progress { get; private set; } = 0;

            private TransformBlock<LoadResourceFromBytesAction, IResourceHandle> _loadByteResources;
            private TransformBlock<LoadResourcesFromFileAction, IResourceHandle> _loadFileResources;
            private TransformManyBlock<LoadTPFResourcesAction, IResourceHandle> _loadTPFResources;
            private ActionBlock<LoadBinderResourcesAction> _loadBinderResources;
            private BufferBlock<IResourceHandle> _processedResources;
            public bool Finished { get; private set; } = false;

            public ResourceJob(string name)
            {
                var options = new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded};
                Name = name;
                _loadByteResources = 
                    new TransformBlock<LoadResourceFromBytesAction, IResourceHandle>(LoadResourceFromBytes, options);
                _loadFileResources =
                    new TransformBlock<LoadResourcesFromFileAction, IResourceHandle>(LoadResourceFromFile, options);
                _loadTPFResources = new TransformManyBlock<LoadTPFResourcesAction, IResourceHandle>(LoadTPFResources, options);
                
                //options.MaxDegreeOfParallelism = 4;
                _loadBinderResources = new ActionBlock<LoadBinderResourcesAction>(LoadBinderResources, options);
                _processedResources = new BufferBlock<IResourceHandle>();
                _loadByteResources.LinkTo(_processedResources);
                _loadFileResources.LinkTo(_processedResources);
                _loadTPFResources.LinkTo(_processedResources);
            }

            internal void IncrementEstimateTaskSize(int size)
            {
                Interlocked.Add(ref TotalSize, size);
            }

            public int GetEstimateTaskSize()
            {
                return Math.Max(TotalSize, _courseSize);
            }

            internal void AddLoadByteResources(LoadResourceFromBytesAction action)
            {
                _loadByteResources.Post(action);
            }
            
            internal void AddLoadFileResources(LoadResourcesFromFileAction action)
            {
                _courseSize++;
                _loadFileResources.Post(action);
            }
            
            internal void AddLoadTPFResources(LoadTPFResourcesAction action)
            {
                _loadTPFResources.Post(action);
            }
            
            internal void AddLoadBinderResources(LoadBinderResourcesAction action)
            {
                _courseSize++;
                _loadBinderResources.Post(action);
            }

            public Task Complete()
            {
                return JobTaskFactory.StartNew(() =>
                {
                    _loadBinderResources.Complete();
                    _loadBinderResources.Completion.Wait();
                    _loadByteResources.Complete();
                    _loadFileResources.Complete();
                    _loadTPFResources.Complete();
                    _loadByteResources.Completion.Wait();
                    _loadFileResources.Completion.Wait();
                    _loadTPFResources.Completion.Wait();
                    Finished = true;
                });
            }

            public void ProcessLoadedResources()
            {
                if (_processedResources.TryReceiveAll(out var processed))
                {
                    Progress += processed.Count;
                }
            }
        }

        public class ResourceJobBuilder
        {
            private ResourceJob _job;
            private string Name;
            private HashSet<string> archivesToLoad = new HashSet<string>();

            public ResourceJobBuilder(string name)
            {
                _job = new ResourceJob(name);
                Name = name;
            }

            /// <summary>
            /// Loads an entire archive in this virtual path
            /// </summary>
            /// <param name="virtualPath"></param>
            public void AddLoadArchiveTask(string virtualPath, AccessLevel al, bool populateOnly, HashSet<string> assets=null)
            {
                if (virtualPath == "null")
                {
                    return;
                }
                if (!archivesToLoad.Contains(virtualPath))
                {
                    archivesToLoad.Add(virtualPath);
                    _job.AddLoadBinderResources(new LoadBinderResourcesAction(_job, virtualPath, al, populateOnly, ResourceType.All, assets));
                }
            }

            public void AddLoadArchiveTask(string virtualPath, AccessLevel al, bool populateOnly, ResourceType filter, HashSet<string> assets = null)
            {
                if (virtualPath == "null")
                {
                    return;
                }
                if (!archivesToLoad.Contains(virtualPath))
                {
                    archivesToLoad.Add(virtualPath);
                    _job.AddLoadBinderResources(new LoadBinderResourcesAction(_job, virtualPath, al, populateOnly, filter, assets));
                }
            }

            /// <summary>
            /// Loads a loose virtual file
            /// </summary>
            /// <param name="virtualPath"></param>
            public void AddLoadFileTask(string virtualPath, AccessLevel al)
            {
                string bndout;
                var path = Locator.VirtualToRealPath(virtualPath, out bndout);

                IResourceHandle handle;
                if (path == null || virtualPath == "null")
                {
                    return;
                }
                if (virtualPath.EndsWith(".hkx"))
                {
                    handle = GetResource<HavokCollisionResource>(virtualPath);
                }
                else if (path.ToUpper().EndsWith(".TPF") || path.ToUpper().EndsWith(".TPF.DCX"))
                {
                    string virt = virtualPath;
                    if (virt.StartsWith($@"map/tex"))
                    {
                        var regex = new Regex(@"\d{4}$");
                        if (regex.IsMatch(virt))
                        {
                            virt = virt.Substring(0, virt.Length - 5);
                        }
                        else if (virt.EndsWith("tex"))
                        {
                            virt = virt.Substring(0, virt.Length - 4);
                        }
                    }
                    _job.AddLoadTPFResources(new LoadTPFResourcesAction(_job, virt, TPF.Read(path), al, Locator.Type));
                    return;
                }
                else
                {
                    handle = GetResource<FlverResource>(virtualPath);
                }
                _job.AddLoadFileResources(new LoadResourcesFromFileAction(handle, path, al, Locator.Type));
            }

            /// <summary>
            /// Attempts to load unloaded resources (with active references) via UDSFM textures
            /// </summary>
            public void AddLoadUDSFMTexturesTask()
            {
                foreach (var r in ResourceDatabase)
                {
                    if (r.Value is TextureResourceHande t && t.AccessLevel == AccessLevel.AccessUnloaded &&
                        t.GetReferenceCounts() > 0)
                    {
                        var texpath = r.Key;
                        string path = null;
                        if (texpath.StartsWith("map/tex"))
                        {
                            path = $@"{Locator.GameRootDirectory}\map\tx\{Path.GetFileName(texpath)}.tpf";
                        }
                        if (path != null && File.Exists(path))
                        {
                            _job.AddLoadTPFResources(new LoadTPFResourcesAction(_job,Path.GetDirectoryName(r.Key).Replace('\\', '/'), TPF.Read(path), AccessLevel.AccessGPUOptimizedOnly, Locator.Type));
                        }
                    }
                }
            }

            /// <summary>
            /// Looks for unloaded textures and queues them up for loading. References to parts and Elden Ring AETs depend on this
            /// </summary>
            public void AddLoadUnloadedTextures()
            {
                var assetTpfs = new HashSet<string>();
                foreach (var r in ResourceDatabase)
                {
                    if (r.Value is TextureResourceHande t && t.AccessLevel == AccessLevel.AccessUnloaded &&
                        t.GetReferenceCounts() > 0)
                    {
                        var texpath = r.Key;
                        string path = null;
                        if (texpath.StartsWith("aet/"))
                        {
                            var splits = texpath.Split('/');
                            var aetid = splits[1];
                            var aetname = splits[2];
                            var fullaetid = aetname.Substring(0, 10);
                            path = $@"{Locator.GameRootDirectory}\asset\aet\{aetid.Substring(0, 6)}\{fullaetid}.tpf.dcx";
                            if (assetTpfs.Contains(fullaetid))
                                continue;
                            assetTpfs.Add(fullaetid);
                        }
                        if (path != null && File.Exists(path))
                        {
                            _job.AddLoadTPFResources(new LoadTPFResourcesAction(_job,
                                Path.GetDirectoryName(r.Key).Replace('\\', '/'), TPF.Read(path),
                                AccessLevel.AccessGPUOptimizedOnly, Locator.Type));
                        }
                    }
                }
            }

            public Task Complete()
            {
                // Build the job, register it with the task manager, and start it
                ActiveJobProgress[_job] = 0;
                var jobtask = _job.Complete();
                var posttask = new Task(async () =>
                {
                    await jobtask;
                    int o;
                    bool removed = false;
                    while (!removed)
                    {
                        removed = ActiveJobProgress.TryRemove(_job, out o);
                    }
                });
                //posttask.Start();
                return jobtask;
            }
        }

        public static AssetLocator Locator;

        private static ConcurrentDictionary<string, IResourceHandle> ResourceDatabase = new ConcurrentDictionary<string, IResourceHandle>();
        private static ConcurrentDictionary<ResourceJob, int> ActiveJobProgress = new ConcurrentDictionary<ResourceJob, int>();

        private static int Pending = 0;
        private static int Finished = 0;
        private static int _prevCount = 0;

        private static object AddResourceLock = new object();
        private static bool AddingResource = false;

        private static bool _scheduleUDSFMLoad = false;
        private static bool _scheduleUnloadedTexturesLoad = false;

        public static BinderReader InstantiateBinderReaderForFile(string filePath, GameType type)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                return null;
            }
            if (type == GameType.DemonsSouls || type == GameType.DarkSoulsPTDE || type == GameType.DarkSoulsRemastered)
            {
                if (filePath.ToUpper().EndsWith("BHD"))
                {
                    return new BXF3Reader(filePath, filePath.Substring(filePath.Length - 3) + "bdt");
                }
                return new BND3Reader(filePath);
            }
            else
            {
                if (filePath.ToUpper().EndsWith("BHD"))
                {
                    return new BXF4Reader(filePath, filePath.Substring(0, filePath.Length - 3) + "bdt");
                }
                return new BND4Reader(filePath);
            }
        }

        public static void LoadResource(string resourceVirtual)
        {
            Pending++;
            Task.Run(() =>
            {
                string leftover;
                var path = Locator.VirtualToRealPath(resourceVirtual, out leftover);
                IBinder bnd = BND4.Read(path);
                var flverbytes = bnd.Files.First(x => x.Name.ToUpper().Contains("FLVER")).Bytes;
                IResourceHandle res = new ResourceHandle<FlverResource>(resourceVirtual);
                res._LoadResource(flverbytes, AccessLevel.AccessGPUOptimizedOnly, Locator.Type);
                ResourceDatabase[resourceVirtual] = res;
                Finished++;
            });
        }

        public static void UnloadUnusedResources()
        {
            foreach (var r in ResourceDatabase)
            {
                //r.Value.CleanupIfUnused();
            }
        }
        
        public static ResourceJobBuilder CreateNewJob(string name)
        {
            return new ResourceJobBuilder(name);
        }

        public static ResourceHandle<T> GetResource<T>(string resourceName) where T : class, IResource, IDisposable, new()
        {
            if (ResourceDatabase.ContainsKey(resourceName))
            {
                return (ResourceHandle<T>)ResourceDatabase[resourceName];
            }
            lock (AddResourceLock)
            {
                var handle = new ResourceHandle<T>(resourceName);
                if (!ResourceDatabase.ContainsKey(resourceName))
                {
                    ResourceDatabase[resourceName] = handle;
                }
            }
            return (ResourceHandle<T>)ResourceDatabase[resourceName];
        }

        public static TextureResourceHande GetTextureResource(string resourceName)
        {
            if (ResourceDatabase.ContainsKey(resourceName))
            {
                return (TextureResourceHande)ResourceDatabase[resourceName];
            }
            lock (AddResourceLock)
            {
                var handle = new TextureResourceHande(resourceName);
                if (!ResourceDatabase.ContainsKey(resourceName))
                {
                    ResourceDatabase[resourceName] = handle;
                }
            }
            return (TextureResourceHande)ResourceDatabase[resourceName];
        }

        public static void ScheduleUDSMFRefresh()
        {
            _scheduleUDSFMLoad = true;
        }

        public static void ScheduleUnloadedTexturesRefresh()
        {
            _scheduleUnloadedTexturesLoad = true;
        }

        public static void UpdateTasks()
        {
            int count = ActiveJobProgress.Count();
            if (count > 0)
            {
                HashSet<ResourceJob> toRemove = new HashSet<ResourceJob>();
                foreach (var job in ActiveJobProgress)
                {
                    job.Key.ProcessLoadedResources();
                    if (job.Key.Finished)
                    {
                        toRemove.Add(job.Key);
                    }
                }
                foreach (var rm in toRemove)
                {
                    int o;
                    ActiveJobProgress.TryRemove(rm, out o);
                }
            }
            else
            {
                if (Scene.Renderer.GeometryBufferAllocator.HasStagingOrPending())
                {
                    var ctx = Tracy.TracyCZoneN(1, "Flush Staging buffer");
                    Scene.Renderer.GeometryBufferAllocator.FlushStaging(true);
                    Tracy.TracyCZoneEnd(ctx);
                }
                if (_prevCount > 0)
                {
                    FlverResource.PurgeCaches();
                }
                if (_scheduleUDSFMLoad)
                {
                    var job = CreateNewJob($@"Loading UDSFM textures");
                    job.AddLoadUDSFMTexturesTask();
                    job.Complete();
                    _scheduleUDSFMLoad = false;
                }
                if (_scheduleUnloadedTexturesLoad)
                {
                    Task.Run(() =>
                    {
                        var job = CreateNewJob($@"Loading other textures");
                        job.AddLoadUnloadedTextures();
                        job.Complete();
                    });
                    _scheduleUnloadedTexturesLoad = false;
                }
            }
            _prevCount = count;
        }

        private static bool TaskWindowOpen = true;
        private static bool ResourceListWindowOpen = true;
        public static void OnGuiDrawTasks(float w, float h)
        {
            if (ActiveJobProgress.Count() > 0)
            {
                ImGui.SetNextWindowSize(new Vector2(400, 310));
                ImGui.SetNextWindowPos(new Vector2(w - 100, h - 300));
                if (!ImGui.Begin("Resource Loading Tasks", ref TaskWindowOpen, ImGuiWindowFlags.NoDecoration))
                {
                    ImGui.End();
                    return;
                }
                foreach (var job in ActiveJobProgress)
                {
                    if (!job.Key.Finished)
                    {
                        var completed = job.Key.Progress;
                        var size = job.Key.GetEstimateTaskSize();
                        ImGui.Text(job.Key.Name);
                        if (size == 0)
                        {
                            ImGui.ProgressBar(0.0f);
                        }
                        else
                        {
                            ImGui.ProgressBar((float)completed / (float)size, new Vector2(386.0f, 20.0f));
                        }
                    }
                }
                ImGui.End();
            }
        }

        public static void OnGuiDrawResourceList()
        {
            if (!ImGui.Begin("Resource List", ref ResourceListWindowOpen))
            {
                ImGui.End();
                return;
            }
            ImGui.Text("List of Resources Loaded & Unloaded");
            ImGui.Columns(4);
            ImGui.Separator();
            int id = 0;
            foreach (var item in ResourceDatabase)
            {
                ImGui.PushID(id);
                ImGui.AlignTextToFramePadding();
                ImGui.Text(item.Key);
                ImGui.NextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text(item.Value.IsLoaded() ? "Loaded" : "Unloaded");
                ImGui.NextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text(item.Value.AccessLevel.ToString());
                ImGui.NextColumn();
                ImGui.AlignTextToFramePadding();
                ImGui.Text(item.Value.GetReferenceCounts().ToString());
                ImGui.NextColumn();
                ImGui.PopID();
            }
            ImGui.Columns(1);
            ImGui.Separator();
            ImGui.End();
        }

        public static void Shutdown()
        {
            JobScheduler.Dispose();
            JobScheduler = null;
        }
    }
}
