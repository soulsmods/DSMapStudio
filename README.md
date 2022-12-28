## About DS Map Studio:
DS Map Studio is a standalone integrated modding tool for modern FromSoft games, which include Demon's Souls (PS3), the Dark Souls series, Bloodborne, Sekiro, and Elden Ring. It currently includes a map editor, a game param editor, and a text editor for editing in game text.

![msb-editor-screenshot](https://user-images.githubusercontent.com/44953920/209740902-ab75c7fb-e281-4833-aeab-4c2ea41da815.png)

## Basic usage instructions
### Game instructions
* **Dark Souls Prepare to die Edition**: Game must be unpacked with UDSFM before usage with Map Studio (https://www.nexusmods.com/darksouls/mods/1304).
* **Dark Souls Remastered**: Not officially supported yet, but it's possible to work with if you copy the map files to an unpacked PTDE installation, load from there, do the modifications, and then copy back to the remastered installation.
* **Dark Souls 2 SOTFS**: Use UXM (https://www.nexusmods.com/sekiro/mods/26) to unpack the game. Vanilla Dark Souls 2 is not supported.
* **Dark Souls 3 and Sekiro**: Use UXM to extract the game files.
* **Demon's Souls**: I test against the US version, but any valid full game dump of Demon's Souls will probably work out of the box. Make sure to disable the RPCS3 file cache to test changes if using the emulator.
* **Bloodborne**: Any valid full game dump should work out of the box. Note that some dumps will have the base game (1.0) and the patch as separate, so the patch should be merged on top of the base game before use with map studio. You're on your own for installing mods to console at the moment.
* **Elden Ring**: Use UXM Selective Unpack (https://github.com/Nordgaren/UXM-Selective-Unpack) to extract the game files. It's recommended to unpack everything, but at least the `map`, `asset`, `chr`, and `msg` directories are needed for basic editor usage.

### Mod projects
Map studio operates on top of something I call mod projects. These are typically stored in a separate directory from the base game, and all modifies files will be saved there instead of overwriting the base game files. The intended workflow is to install mod engine for your respective game and set the modoverridedirectory in modengine.ini to your mod project directory. This way you don't have to modify base game files (and work on multiple mod projects at a time) and you can easily distribute a mod by zipping up the project directory and uploading it.

## FAQ
### Q: Why did you abandon DSTools?
A: DSTools worked well for the creation of many mods, and is still actively used today. However, the bindings of Unity data structures to Souls ones grew very messy and buggy, and led to a very unintuitive user experience (i.e. most users can't intuitively know what Unity operations are actually supported by DSTools for export). Unity also doesn't provide sufficiently low level APIs for many of its useful subsystems like its lightmapper and navmesh generator, so making these subsystems work for Dark Souls range from painful to impossible.

By far the biggest issue though is how heavyweight Unity is and how bad performance is when importing assets. All the Dark Souls assets have to be imported into Unity which takes a large amount of space and imports themselves can take 10s of minutes for a map. All these lead me to decide to make an editor from scratch that is A) heavily focused on the Souls games and have the user interface designed for editing them and B) has super fast load times by loading the game assets directly with no intermediate conversions needing to be stored. Map Studio still lacks some of the more advanced features supported by DSTools + Unity, but currently the core experience is much nicer to use with loading times for maps being measured in seconds rather than minutes.

### Q: Will true custom maps be possible?
A: That's the goal, but asset pipeline work is still needed to get there. I'm currently working on bringing up a navigation mesh generation system for DS1 (and hopefully DS2) based on Recast, which will make full custom maps possible in theory. A simple collision mesh importing system will follow.

### Q: Why are graphics requirements so steep? Will you ever support DX11 again?
A: Likely not. Rendering the entirety of the maps for DS3, Bloodborne, and Sekiro are quite challenging. In game they have techniques to limit draw calls, but in the editor context sometimes literally every mesh in the map may end up rendered. I thus use some modern Vulkan features to be able to batch and issue 10's of thousands of draw calls per frame, which unfortunately makes my renderer architecture incompatible with DX11.

## System Requirements:
* Windows 7/8/8.1/10/11 (64-bit only)
* Visual C++ Redistributable x64 - INSTALL THIS IF THE PROGRAM CRASHES ON STARTUP (https://aka.ms/vs/16/release/vc_redist.x64.exe)
* For the error message "You must install or update .NET to run this application", use these exact download links. It is not enough to install the default .NET runtime.
  * Microsoft .NET Core 6.0 **Desktop** Runtime (https://aka.ms/dotnet/6.0/windowsdesktop-runtime-win-x64.exe)
  * (if Windows not updated) Microsoft .NET Core 6.0 ASP.NET Core Runtime (https://aka.ms/dotnet/6.0/aspnetcore-runtime-win-x64.exe)
* **A Vulkan Compatible Graphics Device with support for descriptor indexing**, even if you're just modding DS1: PTDE
* Intel GPUs currently don't seem to be working properly. At the moment, you will probably need a somewhat recent (2014+) NVIDIA or AMD GPU
* A 4GB (8GB recommended) graphics card if modding DS3/BB/Sekiro/ER maps due to huge map sizes

## Credits:
* Katalash - primary author
* philiquaz - major contributor to integrated param editor
* george - contributor to integrated text editor and other features

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
* Fork Awesome font for icons
