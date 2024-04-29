using StudioCore.Editor;
using System;
using System.Globalization;
using System.IO;

namespace StudioCore;

/// <summary>
///     Holds asset locator, data banks, etc
/// </summary>
public class Project
{
    public readonly GameType Type;

    public readonly Project ParentProject;

    public readonly ProjectAssetLocator AssetLocator;
    
    /// <summary>
    ///     Creates a project based in a single folder with no parent. This is for Game or DSMS files.
    /// </summary>
    public Project(ProjectSettings settings)
    {
        Type = settings.GameType;
        AssetLocator = new(this, settings.GameRoot);
        ParentProject = null;
    }
    /// <summary>
    ///     Creates a project based in a folder with no explicit parent project, with a new ParentProject for the game directory. This is for a mod.
    /// </summary>
    public Project(ProjectSettings settings, string moddir)
    {
        Type = settings.GameType;
        AssetLocator = new(this, moddir);
        ParentProject = new Project(settings);
    }
    /// <summary>
    ///     Creates a project based in a folder with an explicit parent project. This is for an addon or fork of a mod.
    /// </summary>
    public Project(string moddir, Project parent)
    {
        Type = parent.Type;
        AssetLocator = new(this, moddir);
        ParentProject = parent;
    }

    /// <summary>
    ///     Creates a project based on an existing one for recovery purposes
    /// </summary>
    public Project(Project parent)
    {
        var time = DateTime.Now.ToString("dd-MM-yyyy-(hh-mm-ss)", CultureInfo.InvariantCulture);
        Type = parent.Type;
        AssetLocator = new(this, parent.AssetLocator.RootDirectory + $@"\recovery\{time}");
        ParentProject = parent.ParentProject;
        if (!Directory.Exists(AssetLocator.RootDirectory))
        {
            Directory.CreateDirectory(AssetLocator.RootDirectory);
        }
    }

    public Project CreateRecoveryProject()
    {
        try
        {
            return new Project(this);
        }
        catch (Exception e)
        {
            return this;
        }
    }
}
