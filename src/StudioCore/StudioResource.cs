using StudioCore.Editor;
using System.Collections.Generic;

namespace StudioCore;

/// <summary>
///     Class that stores a collection of mapstudio data independent of a given project
/// </summary>
public abstract class StudioResource
{
    public GameType GameType;
    public readonly string nameForUI;

    public StudioResource(GameType gameType, string name)
    {
        GameType = gameType;
        nameForUI = name;
    }

    public bool IsLoaded { get; private set; }
    public bool IsLoading { get; private set; }

    private void Load(Project project)
    {
        if (IsLoaded || IsLoading)
            return;
        // Locking on this isn't ideal if anything else could lock the object. But we don't have Lock yet.
        lock (this)
        {
            IsLoading = true;
            List<TaskManager.LiveTask> tasks = new();
            foreach (StudioResource res in GetDependencies(project))
            {
                if (!res.IsLoaded)
                {
                    TaskManager.LiveTask t = new TaskManager.LiveTask(res.GetTaskName(), TaskManager.RequeueType.None, true, () => {
                        res.Load(project);
                    });
                    t = TaskManager.Run(t);
                    tasks.Add(t);
                }
            }
            foreach (TaskManager.LiveTask t in tasks)
            {
                if (!t.Task.IsCompleted)
                    t.Task.Wait();
            }
            Load();
            IsLoaded = true;
            IsLoading = false;
        }
    }

    public virtual string GetTaskName()
    {
        return $@"Resource - Loading {nameForUI}";
    }
    protected abstract void Load();
    protected abstract IEnumerable<StudioResource> GetDependencies(Project project);

    public static bool AreResourcesLoaded(IEnumerable<StudioResource> res)
    {
        foreach (StudioResource r in res)
        {
            if (!r.IsLoaded)
                return false;
        }
        return true;
    }
    public static void Load(Project project, IEnumerable<StudioResource> resources)
    {
        foreach (StudioResource res in resources)
        {
            if (!res.IsLoaded)
            {
                TaskManager.Run(new TaskManager.LiveTask(res.GetTaskName(), TaskManager.RequeueType.None, true, () => {
                    res.Load(project);
                }));
            }
        }
    }
}
