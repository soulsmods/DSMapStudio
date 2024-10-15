using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.Data;
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
///     Static data not tied to a project, but perhaps tied to a loaded game.
/// </summary>
public class ResDirectory
{
    public static ResDirectory CurrentGame { get; set; } = new();

    public Dictionary<string, Project> AuxProjects = new();

    public ParamDefBank ParamDefBank = new();
    public ParamMetaBank ParamMetaBank = new();

}
