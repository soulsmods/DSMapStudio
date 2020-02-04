## About DS Map Studio:
DS Map Studio is a standalone map editor for all the souls games. It is intended to be the successor to DSTools, but be much more lightweight with a heavy emphasis on editor performance. It is currently in alpha testing with builds being uploaded to releases periodically. It is functional in many cases for games and supports saving maps for all the games but DeS and Sekiro, but the editor still lacks the stability and polish expected from a full release, so keep that in mind when attempting to use the editor.

## FAQ
### Q: Why did you abandon DSTools?
A: DSTools worked well for the creation of many mods, and is still actively used today. However, the bindings of Unity data structures to Souls ones grew very messy and buggy, and led to a very unintuitive user experience (i.e. most users can't intuitively know what Unity operations are actually supported by DSTools for export). Unity also doesn't provide sufficiently low level APIs for many of its useful subsystems like its lightmapper and navmesh generator, so making these subsystems work for Dark Souls range from painful to impossible.

By far the biggest issue though is how heavyweight Unity is and how bad performance is when importing assets. All the Dark Souls assets have to be imported into Unity which takes a large amount of space and imports themselves can take 10s of minutes for a map. All these lead me to decide to make an editor from scratch that is A) heavily focused on the Souls games and have the user interface designed for editing them and B) has super fast load times by loading the game assets directly with no intermediate conversions needing to be stored. Map Studio still lacks some of the more advanced features supported by DSTools + Unity, but currently the core experience is much nicer to use with loading times for maps being measured in seconds rather than minutes.

### Q: Will true custom maps be possible?
A: That's the goal, but asset pipeline work is still needed to get there. I'm currently working on bringing up a navigation mesh generation system for DS1 (and hopefully DS2) based on Recast, which will make full custom maps possible in theory. A simple collision mesh importing system will follow.

## System Requirements:
* Windows 7/8/8.1/10 (64-bit only)
* [Microsoft .Net Core 3.1 **Desktop** Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* [Visual C++ Redistributable x64 - INSTALL THIS IF THE PROGRAM CRASHES ON STARTUP](https://aka.ms/vs/16/release/vc_redist.x64.exe)
* **A DirectX 11 Compatible Graphics Device**, even if you're just modding DS1: PTDE

## Special Thanks
* TKGP - Made Soulsformats
* [Pav](https://github.com/JohrnaJohrna)
* [Meowmaritus](https://github.com/meowmaritus) - Made DSAnimStudio, which DSMapStudio is loosely based on
* [PredatorCZ](https://github.com/PredatorCZ) - Reverse engineered Spline-Compressed Animation entirely.
* [Horkrux](https://github.com/horkrux) - Reverse engineered the header and swizzling used on non-PC platform textures.
* [Vawser](https://github.com/vawser) - DS2/3 Documentation

## Libraries Utilized
* Soulsformats
* [Newtonsoft Json.NET](https://www.newtonsoft.com/json)
* Veldrid for rendering
* ImGui.NET for UI
* A small portion of [HavokLib](https://github.com/PredatorCZ/HavokLib), specifically the spline-compressed animation decompressor, adapted for C#
* Recast for navigation mesh generation
