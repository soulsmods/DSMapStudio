using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using SoulsFormats;
using ImGuiNET;

namespace StudioCore.Resource
{
    /// <summary>
    /// Manages resources (mainly GPU) such as textures and models, and can be used to unload and reload them at will.
    /// A background thread will manage the unloading and streaming in of assets. This is designed to map closely to
    /// the souls asset system, but in a more abstract way
    /// </summary>
    public class ResourceManager
    {
        public enum ResourceType
        {
            Flver,
            Texture,
        }

        private IResourceHandle InstantiateResource(ResourceType type, string path)
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
            public LoadResourceFromBytesTask(IResourceHandle handle, byte[] bytes, AccessLevel al)
            {
                this.handle = handle;
                this.bytes = bytes;
                this.AccessLevel = al;
            }

            public int GetEstimateTaskSize()
            {
                return 1;
            }

            public void Run()
            {
                handle._LoadResource(bytes, AccessLevel);
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return Task.Run(() =>
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
            public LoadResourceFromFileTask(IResourceHandle handle, string file, AccessLevel al)
            {
                this.handle = handle;
                this.file = file;
                this.AccessLevel = al;
            }

            public int GetEstimateTaskSize()
            {
                return 1;
            }

            public void Run()
            {
                handle._LoadResource(file, AccessLevel);
            }

            public Task RunAsync(IProgress<int> progress)
            {
                return Task.Run(() =>
                {
                    Run();
                    progress.Report(1);
                });
            }
        }

        public class LoadBinderResourcesTask : IResourceTask
        {
            private ResourceManager ResourceMan;
            private string BinderVirtualPath = null;
            private BinderReader Binder = null;
            private bool PopulateResourcesOnly = false;
            private HashSet<int> BinderLoadMask = null;
            private List<Task> LoadingTasks = new List<Task>();
            private List<int> TaskSizes = new List<int>();
            private List<int> TaskProgress = new List<int>();
            private int TotalSize = 0;

            private List<Tuple<IResourceHandle, string, BinderFileHeader>> PendingResources = new List<Tuple<IResourceHandle, string, BinderFileHeader>>();
            private List<BinderFileHeader> PendingTPFs = new List<BinderFileHeader>();

            private readonly object ProgressLock = new object();

            public LoadBinderResourcesTask(ResourceManager rm, string virtpath, bool populateOnly)
            {
                ResourceMan = rm;
                BinderVirtualPath = virtpath;
                PopulateResourcesOnly = populateOnly;
            }

            public void ProcessBinder()
            {
                if (Binder == null)
                {
                    string o;
                    var path = ResourceMan.Locator.VirtualToRealPath(BinderVirtualPath, out o);
                    Binder = InstantiateBinderReaderForFile(path, ResourceMan.Locator.Type);
                }

                for (int i = 0; i < Binder.Files.Count(); i++)
                {
                    var f = Binder.Files[i];
                    if (BinderLoadMask != null && !BinderLoadMask.Contains(i))
                    {
                        continue;
                    }
                    var binderpath = f.Name;
                    var filevirtpath = ResourceMan.Locator.GetBinderVirtualPath(BinderVirtualPath, binderpath);
                    IResourceHandle handle = null;
                    /*if (ResourceMan.ResourceDatabase.ContainsKey(filevirtpath))
                    {
                        handle = ResourceMan.ResourceDatabase[filevirtpath];
                    }*/
                    if (filevirtpath.ToUpper().EndsWith(".TPF"))
                    {

                    }
                    else
                    {
                        if (filevirtpath.ToUpper().EndsWith(".FLVER"))
                        {
                            //handle = new ResourceHandle<FlverResource>();
                            handle = ResourceMan.GetResource<FlverResource>(filevirtpath);
                        }
                        else if (filevirtpath.ToUpper().EndsWith(".NVM"))
                        {
                            handle = ResourceMan.GetResource<NVMNavmeshResource>(filevirtpath);
                        }

                        if (handle != null)
                        {
                            PendingResources.Add(new Tuple<IResourceHandle, string, BinderFileHeader>(handle, filevirtpath, f));
                        }
                    }
                }

                /*foreach (var p in PendingResources)
                {
                    if (!ResourceMan.ResourceDatabase.ContainsKey(p.Item2))
                    {
                        ResourceMan.ResourceDatabase[p.Item2] = p.Item1;
                    }
                }*/
            }

            public void Run()
            {
                ProcessBinder();
                if (!PopulateResourcesOnly)
                {
                    foreach (var p in PendingResources)
                    {
                        var f = Binder.ReadFile(p.Item3);
                        var task = new LoadResourceFromBytesTask(p.Item1, f, AccessLevel.AccessGPUOptimizedOnly);
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
                return Task.Run(() =>
                {
                    ProcessBinder();
                    if (!PopulateResourcesOnly)
                    {
                        bool doasync = (PendingResources.Count() + PendingTPFs.Count()) > 1;
                        int i = 0;
                        foreach (var p in PendingResources)
                        {
                            var f = Binder.ReadFile(p.Item3);
                            var task = new LoadResourceFromBytesTask(p.Item1, f, AccessLevel.AccessGPUOptimizedOnly);
                            var size = task.GetEstimateTaskSize();
                            TotalSize += size;
                            if (doasync)
                            {
                                var progress1 = new Progress<int>();
                                TaskSizes.Add(size);
                                TaskProgress.Add(0);
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
                return Task.Run(() =>
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
                });
            }
        }

        public class ResourceJobBuilder
        {
            private ResourceManager ResourceMan = null;
            private List<IResourceTask> Tasks = new List<IResourceTask>();
            private string Name;
            private HashSet<string> archivesToLoad = new HashSet<string>();

            public ResourceJobBuilder(ResourceManager manager, string name)
            {
                ResourceMan = manager;
                Name = name;
            }

            /// <summary>
            /// Loads an entire archive in this virtual path
            /// </summary>
            /// <param name="virtualPath"></param>
            public void AddLoadArchiveTask(string virtualPath, bool populateOnly)
            {
                if (virtualPath == "null")
                {
                    return;
                }
                if (!archivesToLoad.Contains(virtualPath))
                {
                    archivesToLoad.Add(virtualPath);
                    var task = new LoadBinderResourcesTask(ResourceMan, virtualPath, populateOnly);
                    Tasks.Add(task);
                }
            }

            /// <summary>
            /// Loads a loose virtual file
            /// </summary>
            /// <param name="virtualPath"></param>
            public void AddLoadFileTask(string virtualPath)
            {
                IResourceHandle handle;
                if (virtualPath == "null")
                {
                    return;
                }
                if (virtualPath.EndsWith(".hkx"))
                {
                    handle = ResourceMan.GetResource<HavokCollisionResource>(virtualPath);
                }
                else
                {
                    handle = ResourceMan.GetResource<FlverResource>(virtualPath);
                }
                string bndout;
                var path = ResourceMan.Locator.VirtualToRealPath(virtualPath, out bndout);
                var task = new LoadResourceFromFileTask(handle, path, AccessLevel.AccessGPUOptimizedOnly);
                Tasks.Add(task);
            }

            public Task StartJobAsync()
            {
                // Build the job, register it with the task manager, and start it
                var job = new ResourceJob(Name, Tasks);
                ResourceMan.ActiveJobProgress[job] = 0;
                var progress1 = new Progress<int>();
                progress1.ProgressChanged += (x, e) =>
                {
                    ResourceMan.ActiveJobProgress[job] = e;
                };
                Tasks = null;
                var jobtask = job.RunAsync(progress1);
                var posttask = new Task(async () =>
                {
                    await jobtask;
                    int o;
                    ResourceMan.ActiveJobProgress.TryRemove(job, out o);
                });
                posttask.RunSynchronously();
                return jobtask;
            }
        }

        public AssetLocator Locator;

        private ConcurrentDictionary<string, IResourceHandle> ResourceDatabase = new ConcurrentDictionary<string, IResourceHandle>();
        private ConcurrentDictionary<ResourceJob, int> ActiveJobProgress = new ConcurrentDictionary<ResourceJob, int>();

        private int Pending = 0;
        private int Finished = 0;

        private object AddResourceLock = new object();
        private bool AddingResource = false;

        public ResourceManager()
        {
            ThreadPool.SetMaxThreads(12, 12);
        }

        public static BinderReader InstantiateBinderReaderForFile(string filePath, GameType type)
        {
            if (type == GameType.DemonsSouls || type == GameType.DarkSoulsPTDE || type == GameType.DarkSoulsRemastered)
            {
                return new BND3Reader(filePath);
            }
            else
            {
                return new BND4Reader(filePath);
            }
        }

        public void LoadResource(string resourceVirtual)
        {
            Pending++;
            Task.Run(() =>
            {
                string leftover;
                var path = Locator.VirtualToRealPath(resourceVirtual, out leftover);
                IBinder bnd = BND4.Read(path);
                var flverbytes = bnd.Files.First(x => x.Name.ToUpper().Contains("FLVER")).Bytes;
                IResourceHandle res = new ResourceHandle<FlverResource>(resourceVirtual);
                res._LoadResource(flverbytes, AccessLevel.AccessGPUOptimizedOnly);
                ResourceDatabase[resourceVirtual] = res;
                Finished++;
            });
        }

        public ResourceJobBuilder CreateNewJob(string name)
        {
            return new ResourceJobBuilder(this, name);
        }

        public ResourceHandle<T> GetResource<T>(string resourceName) where T : class, IResource, IDisposable, new()
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

        private static bool TaskWindowOpen = true;
        private static bool ResourceListWindowOpen = true;
        public void OnGuiDrawTasks(float w, float h)
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
                    var size = job.Key.GetEstimateTaskSize();
                    ImGui.Text(job.Key.Name);
                    if (size == 0)
                    {
                        ImGui.ProgressBar(0.0f);
                    }
                    else
                    {
                        //ImGui.ProgressBar((float)Finished / (float)Math.Max(Pending, 1.0));
                        ImGui.ProgressBar((float)job.Value / (float)size);
                    }
                }
                ImGui.End();
            }
            ImGui.SetNextWindowSize(new Vector2(400, 250), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(new Vector2(20, h - 300), ImGuiCond.FirstUseEver);
            if (!ImGui.Begin("Resource List", ref ResourceListWindowOpen))
            {
                ImGui.End();
                return;
            }
            ImGui.Columns(2);
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
                ImGui.PopID();
            }
            ImGui.Columns(1);
            ImGui.Separator();
            ImGui.End();
        }
    }
}
