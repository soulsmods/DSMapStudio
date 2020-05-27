using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Threading;
using System.Numerics;
using System.IO;
using System.Text.RegularExpressions;
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
        private static QueuedTaskScheduler BinderWorkerScheduler = new QueuedTaskScheduler(6, "BinderWorker");
        private static QueuedTaskScheduler ResourceWorkerScheduler = new QueuedTaskScheduler(6, "ResourceWorker");

        private static TaskFactory JobTaskFactory = new TaskFactory(JobScheduler);
        private static TaskFactory BinderTaskFactory = new TaskFactory(BinderWorkerScheduler);
        private static TaskFactory ResourceTaskFactory = new TaskFactory(ResourceWorkerScheduler);

        [Flags]
        public enum ResourceType
        {
            Flver = 1,
            Texture = 2,
            CollisionHKX = 4,
            Navmesh = 8,
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

        public class LoadResourceFromBytesTask : IResourceTask
        {
            private IResourceHandle handle = null;
            private byte[] bytes = null;
            private AccessLevel AccessLevel = AccessLevel.AccessGPUOptimizedOnly;
            private GameType Game;
            public LoadResourceFromBytesTask(IResourceHandle handle, byte[] bytes, AccessLevel al, GameType type)
            {
                this.handle = handle;
                this.bytes = bytes;
                this.AccessLevel = al;
                Game = type;
            }

            public int GetEstimateTaskSize()
            {
                return 1;
            }

            public void Run()
            {
                handle._LoadResource(bytes, AccessLevel, Game);
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return ResourceTaskFactory.StartNew(() =>
                {
                    Run();
                    progress.Report(1);
                });
            }
        }

        public class LoadResourceFromFileTask : IResourceTask
        {
            private IResourceHandle handle = null;
            private string file = null;
            private AccessLevel AccessLevel = AccessLevel.AccessGPUOptimizedOnly;
            private GameType Game;
            public LoadResourceFromFileTask(IResourceHandle handle, string file, AccessLevel al, GameType type)
            {
                this.handle = handle;
                this.file = file;
                this.AccessLevel = al;
                this.Game = type;
            }

            public int GetEstimateTaskSize()
            {
                return 1;
            }

            public void Run()
            {
                handle._LoadResource(file, AccessLevel, Game);
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return ResourceTaskFactory.StartNew(() =>
                {
                    Run();
                    progress.Report(1);
                });
            }
        }

        public class LoadTPFResourcesTask : IResourceTask
        {
            private string _virtpathbase = null;
            private TPF _tpf = null;
            private AccessLevel _accessLevel = AccessLevel.AccessGPUOptimizedOnly;
            private GameType _game;

            private List<Tuple<TextureResourceHande, string, int>> _pendingResources = new List<Tuple<TextureResourceHande, string, int>>();

            public LoadTPFResourcesTask(string virtpathbase, TPF tpf, AccessLevel al, GameType type)
            {
                _virtpathbase = virtpathbase;
                _tpf = tpf;
                _accessLevel = al;
                _game = type;
            }

            public int GetEstimateTaskSize()
            {
                return 1;
            }

            public void Run()
            {
                if (!CFG.Current.EnableTexturing)
                {
                    _pendingResources.Clear();
                    _tpf = null;
                    return;
                }

                for (int i = 0; i < _tpf.Textures.Count; i++)
                {
                    var tex = _tpf.Textures[i];
                    var handle = ResourceManager.GetTextureResource($@"{_virtpathbase}/{tex.Name.ToLower()}");
                    _pendingResources.Add(new Tuple<TextureResourceHande, string, int>(handle, $@"{_virtpathbase}/{tex.Name}", i));
                }

                foreach (var t in _pendingResources)
                {
                    t.Item1._LoadTextureResource(_tpf, t.Item3, _accessLevel, _game);
                }
                _pendingResources.Clear();
                _tpf = null;
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return ResourceTaskFactory.StartNew(() =>
                {
                    Run();
                    progress.Report(1);
                });
            }
        }

        public class LoadBinderResourcesTask : IResourceTask
        {
            private string BinderVirtualPath = null;
            private BinderReader Binder = null;
            private bool PopulateResourcesOnly = false;
            private HashSet<int> BinderLoadMask = null;
            private List<Task> LoadingTasks = new List<Task>();
            private List<int> TaskSizes = new List<int>();
            private List<int> TaskProgress = new List<int>();
            private int TotalSize = 0;
            private HashSet<string> AssetWhitelist = null;
            private ResourceType ResourceMask = ResourceType.All;
            private AccessLevel AccessLevel = AccessLevel.AccessGPUOptimizedOnly;

            private List<Tuple<IResourceHandle, string, BinderFileHeader>> PendingResources = new List<Tuple<IResourceHandle, string, BinderFileHeader>>();
            private List<Tuple<string, BinderFileHeader>> PendingTPFs = new List<Tuple<string, BinderFileHeader>>();

            private readonly object ProgressLock = new object();

            public LoadBinderResourcesTask(string virtpath, AccessLevel accessLevel, bool populateOnly, ResourceType mask, HashSet<string> whitelist)
            {
                BinderVirtualPath = virtpath;
                PopulateResourcesOnly = populateOnly;
                ResourceMask = mask;
                AssetWhitelist = whitelist;
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
                    /*if (ResourceMan.ResourceDatabase.ContainsKey(filevirtpath))
                    {
                        handle = ResourceMan.ResourceDatabase[filevirtpath];
                    }*/

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

                        if (handle != null)
                        {
                            PendingResources.Add(new Tuple<IResourceHandle, string, BinderFileHeader>(handle, filevirtpath, f));
                        }
                    }
                }
            }

            public void Run()
            {
                ProcessBinder();
                if (!PopulateResourcesOnly)
                {
                    foreach (var p in PendingResources)
                    {
                        var f = Binder.ReadFile(p.Item3);
                        var task = new LoadResourceFromBytesTask(p.Item1, f, AccessLevel, ResourceManager.Locator.Type);
                        task.Run();
                    }

                    foreach (var t in PendingTPFs)
                    {
                        var f = TPF.Read(Binder.ReadFile(t.Item2));
                        var task = new LoadTPFResourcesTask(t.Item1, f, AccessLevel, ResourceManager.Locator.Type);
                        task.Run();
                    }
                }
                PendingResources.Clear();
                Binder = null;
            }

            public void UpdateProgress(IProgress<int> progress)
            {
                int totalProgress;
                lock (ProgressLock)
                {
                    totalProgress = TaskProgress.Sum();
                }
                if (TotalSize != 0)
                {
                    progress.Report(totalProgress);
                }
                else
                {
                    progress.Report(0);
                }
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return BinderTaskFactory.StartNew(() =>
                {
                    ProcessBinder();
                    if (!PopulateResourcesOnly)
                    {
                        bool doasync = (PendingResources.Count() + PendingTPFs.Count()) > 1;
                        int i = 0;
                        foreach (var p in PendingResources)
                        {
                            var f = Binder.ReadFile(p.Item3);
                            var task = new LoadResourceFromBytesTask(p.Item1, f, AccessLevel.AccessGPUOptimizedOnly, ResourceManager.Locator.Type);
                            var size = task.GetEstimateTaskSize();
                            TotalSize += size;
                            if (doasync)
                            {
                                var progress1 = new Progress<int>();
                                TaskSizes.Add(size);
                                lock (ProgressLock)
                                {
                                    TaskProgress.Add(0);
                                }
                                int bindi = i;
                                progress1.ProgressChanged += (x, e) =>
                                {
                                    lock (ProgressLock)
                                    {
                                        TaskProgress[bindi] = e;
                                    }
                                    UpdateProgress(progress);
                                };
                                LoadingTasks.Add(task.RunAsync(progress1));
                                i++;
                            }
                            else
                            {
                                task.Run();
                                i++;
                                progress.Report(i);
                            }
                        }

                        foreach (var t in PendingTPFs)
                        {
                            var f = TPF.Read(Binder.ReadFile(t.Item2));
                            var task = new LoadTPFResourcesTask(t.Item1, f, AccessLevel.AccessGPUOptimizedOnly, ResourceManager.Locator.Type);
                            var size = task.GetEstimateTaskSize();
                            TotalSize += size;
                            if (doasync)
                            {
                                var progress1 = new Progress<int>();
                                TaskSizes.Add(size);
                                lock (ProgressLock)
                                {
                                    TaskProgress.Add(0);
                                }
                                int bindi = i;
                                progress1.ProgressChanged += (x, e) =>
                                {
                                    lock (ProgressLock)
                                    {
                                        TaskProgress[bindi] = e;
                                    }
                                    UpdateProgress(progress);
                                };
                                LoadingTasks.Add(task.RunAsync(progress1));
                                i++;
                            }
                            else
                            {
                                task.Run();
                                i++;
                                progress.Report(i);
                            }
                        }
                    }
                    PendingResources.Clear();

                    // Wait for all the tasks to complete
                    while (LoadingTasks.Count() > 0)
                    {
                        int idx = Task.WaitAny(LoadingTasks.ToArray());
                        LoadingTasks.RemoveAt(idx);
                    }
                    Binder = null;
                });
            }

            public int GetEstimateTaskSize()
            {
                if (TotalSize == 0)
                {
                    // Shitty estimate, but usually works as it is used most often when there's a large amount of tiny archives
                    return 1;
                }
                return TotalSize;
            }
        }

        /// <summary>
        /// A named job that runs many tasks and whose progress will appear in the progress window
        /// </summary>
        public class ResourceJob : IResourceTask
        {
            public string Name { get; private set; } = null;
            private List<IResourceTask> TaskList;
            private List<Task> RunningTaskList = new List<Task>();
            private List<int> TaskProgress = new List<int>();
            private int TotalSize = 0;

            private readonly object ProgressLock = new object();
            private readonly object TaskListLock = new object();

            public bool Finished { get; private set; } = false;

            public ResourceJob(string name, List<IResourceTask> tasks)
            {
                Name = name;
                TaskList = tasks;
            }

            public int GetEstimateTaskSize()
            {
                if (TotalSize == 0)
                {
                    int size = 0;
                    lock (TaskListLock)
                    {
                        foreach (var t in TaskList)
                        {
                            size += t.GetEstimateTaskSize();
                        }
                    }
                    TotalSize = size;
                }
                return TotalSize;
            }

            public void Run()
            {
                foreach (var t in TaskList)
                {
                    t.Run();
                }
            }

            public void UpdateProgress(IProgress<int> progress)
            {
                int totalProgress;
                lock (ProgressLock)
                {
                    totalProgress = TaskProgress.Sum();
                }
                TotalSize = 0;
                GetEstimateTaskSize();
                if (TotalSize != 0)
                {
                    progress.Report(totalProgress);
                }
                else
                {
                    progress.Report(0);
                }
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return JobTaskFactory.StartNew(() =>
                {
                    TotalSize = 0;
                    int i = 0;
                    foreach (var t in TaskList)
                    {
                        var progress1 = new Progress<int>();
                        lock (ProgressLock)
                        {
                            TaskProgress.Add(0);
                        }
                        int bindi = i;
                        progress1.ProgressChanged += (x, e) =>
                        {
                            lock (ProgressLock)
                            {
                                TaskProgress[bindi] = e;
                            }
                            UpdateProgress(progress);
                            TotalSize = 0; // Hack to force recompuation of total task size
                        };
                        RunningTaskList.Add(t.RunAsync(progress1));
                        TotalSize = t.GetEstimateTaskSize();
                        i++;
                    }

                    while (RunningTaskList.Count() > 0)
                    {
                        int idx = Task.WaitAny(RunningTaskList.ToArray());
                        RunningTaskList.RemoveAt(idx);
                        //TaskList.RemoveAt(idx);
                    }
                    lock (TaskListLock)
                    {
                        TaskList.Clear();
                    }
                    Finished = true;
                });
            }
        }

        public class ResourceJobBuilder
        {
            private List<IResourceTask> Tasks = new List<IResourceTask>();
            private string Name;
            private HashSet<string> archivesToLoad = new HashSet<string>();

            public ResourceJobBuilder(string name)
            {
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
                    var task = new LoadBinderResourcesTask(virtualPath, al, populateOnly, ResourceType.All, assets);
                    Tasks.Add(task);
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
                    var task = new LoadBinderResourcesTask(virtualPath, al, populateOnly, filter, assets);
                    Tasks.Add(task);
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
                    var ttask = new LoadTPFResourcesTask(virt, TPF.Read(path), al, Locator.Type);
                    Tasks.Add(ttask);
                    return;
                }
                else
                {
                    handle = GetResource<FlverResource>(virtualPath);
                }
                var task = new LoadResourceFromFileTask(handle, path, al, Locator.Type);
                Tasks.Add(task);
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
                            var task = new LoadTPFResourcesTask(Path.GetDirectoryName(r.Key).Replace('\\', '/'), TPF.Read(path), AccessLevel.AccessGPUOptimizedOnly, Locator.Type);
                            Tasks.Add(task);
                        }
                    }
                }
            }

            public Task StartJobAsync()
            {
                // Build the job, register it with the task manager, and start it
                var job = new ResourceJob(Name, Tasks);
                ActiveJobProgress[job] = 0;
                var progress1 = new Progress<int>();
                progress1.ProgressChanged += (x, e) =>
                {
                    ActiveJobProgress[job] = e;
                };
                Tasks = null;
                var jobtask = job.RunAsync(progress1);
                var posttask = new Task(async () =>
                {
                    await jobtask;
                    int o;
                    bool removed = false;
                    while (!removed)
                    {
                        removed = ActiveJobProgress.TryRemove(job, out o);
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
                if (r.Value.GetReferenceCounts() == 0)
                {
                    r.Value.Release();
                }
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

        public static void UpdateTasks()
        {
            int count = ActiveJobProgress.Count();
            if (count > 0)
            {
                HashSet<ResourceJob> toRemove = new HashSet<ResourceJob>();
                foreach (var job in ActiveJobProgress)
                {
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
                    Scene.Renderer.GeometryBufferAllocator.FlushStaging(true);
                }
                if (_prevCount > 0)
                {
                    FlverResource.PurgeCaches();
                }
                if (_scheduleUDSFMLoad)
                {
                    var job = CreateNewJob($@"Loading UDSFM textures");
                    job.AddLoadUDSFMTexturesTask();
                    job.StartJobAsync();
                    _scheduleUDSFMLoad = false;
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
                ImGui.SetNextWindowSize(new Vector2(400, 250));
                ImGui.SetNextWindowPos(new Vector2(w - 450, h - 300));
                if (!ImGui.Begin("Resource Loading Tasks", ref TaskWindowOpen, ImGuiWindowFlags.NoDecoration))
                {
                    ImGui.End();
                    return;
                }
                foreach (var job in ActiveJobProgress)
                {
                    if (!job.Key.Finished)
                    {
                        var size = job.Key.GetEstimateTaskSize();
                        ImGui.Text(job.Key.Name);
                        if (size == 0)
                        {
                            ImGui.ProgressBar(0.0f);
                        }
                        else
                        {
                            ImGui.ProgressBar((float)job.Value / (float)size);
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
            BinderWorkerScheduler.Dispose();
            ResourceWorkerScheduler.Dispose();
            JobScheduler = null;
            BinderWorkerScheduler = null;
            ResourceWorkerScheduler = null;
        }
    }
}
