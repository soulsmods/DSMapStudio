using SoulsFormats;
using StudioCore.Editor;
using StudioCore.ParamEditor;
using StudioCore.TextEditor;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace StudioCore;

/// <summary>
///     Holds asset locator, data banks, etc
/// </summary>
public class Project
{
    public readonly ProjectSettings Settings;

    public readonly Project ParentProject;

    public readonly ProjectAssetLocator AssetLocator;

    public readonly ParamBank ParamBank;

    public readonly FMGBank FMGBank;

    
    public GameType Type => Settings.GameType;
    
    /// <summary>
    ///     Creates a project based in a single folder with no parent. This is for Game or DSMS files.
    /// </summary>
    public Project(ProjectSettings settings)
    {
        Settings = settings;
        AssetLocator = new(this, settings.GameRoot);
        ParentProject = null;

        ParamBank = new(this);
        FMGBank = new(this);
    }
    /// <summary>
    ///     Creates a project based in a folder with no explicit parent project, with a new ParentProject for the game directory. This is for a mod.
    /// </summary>
    public Project(ProjectSettings settings, string moddir)
    {
        Settings = settings;
        AssetLocator = new(this, moddir);
        ParentProject = new Project(settings);

        ParamBank = new(this);
        FMGBank = new(this);
    }
    /// <summary>
    ///     Creates a project based in a folder with an explicit parent project. This is for an addon or fork of a mod.
    /// </summary>
    public Project(string moddir, Project parent, ProjectSettings settings = null)
    {
        if (settings == null)
        {
            Settings = parent.Settings.CopyAndAssumeFromModDir(moddir);
        }
        else
        {
            Settings = settings;
        }
        AssetLocator = new(this, moddir);
        ParentProject = parent;

        ParamBank = new(this);
        FMGBank = new(this);
    }

    /// <summary>
    ///     Creates a project based on an existing one for recovery purposes. Shares resources with the original project.
    /// </summary>
    public Project(Project parent)
    {
        var time = DateTime.Now.ToString("dd-MM-yyyy-(hh-mm-ss)", CultureInfo.InvariantCulture);
        Settings = parent.Settings;
        AssetLocator = new(this, parent.AssetLocator.RootDirectory + $@"\recovery\{time}");
        ParentProject = parent.ParentProject;
        if (!Directory.Exists(AssetLocator.RootDirectory))
        {
            Directory.CreateDirectory(AssetLocator.RootDirectory);
        }

        ParamBank = parent.ParamBank;
        FMGBank = parent.FMGBank;
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
