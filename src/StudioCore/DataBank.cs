namespace StudioCore;

/// <summary>
///     Class that stores a collection of game data sourced from a single project
/// </summary>
public abstract class DataBank : StudioResource
{
    public Project Project;

    public DataBank(Project project, string nameForUI) : base(project.Settings.GameType, nameForUI)
    {
        Project = project;
    }
    public override string GetTaskName()
    {
        return $@"Resource - Loading {nameForUI} ({Project.Settings.ProjectName})";
    }
    public abstract void Save();
}
