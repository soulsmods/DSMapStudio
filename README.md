## About DS Map Studio:
DS Map Studio is a standalone map editor for all the souls games. It's currently a work in progress, and there isn't a public release yet. Stay tuned for more information.

## System Requirements:
* Windows 7/8/8.1/10 (64-bit only)
* [Microsoft .Net Core 3.0 Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.0)
* **A DirectX 11 Compatible Graphics Device**, even if you're just modding DS1: PTDE

## Special Thanks
* TKGP - Made Soulsformats
* [Pav](https://github.com/JohrnaJohrna)
* [Meowmaritus](https://github.com/meowmaritus) - Made DSAnimStudio, which DSMapStudio is loosely based on
* [PredatorCZ](https://github.com/PredatorCZ) - Reverse engineered Spline-Compressed Animation entirely.
* [Horkrux](https://github.com/horkrux) - Reverse engineered the header and swizzling used on non-PC platform textures.

## Libraries Utilized
* Soulsformats
* [Newtonsoft Json.NET](https://www.newtonsoft.com/json)
* Veldrid for rendering
* ImGui.NET for UI
* A small portion of [HavokLib](https://github.com/PredatorCZ/HavokLib), specifically the spline-compressed animation decompressor, adapted for C#