This repository contains low-level bindings for the [Vulkan](https://www.khronos.org/vulkan/) graphics and compute API.

# Vulkan bindings and samples for .NET Core

There are several projects included in this repository, including some sample projects using the Vulkan API, which have been adapted from the excellent samples created by Sascha Willems: https://github.com/SaschaWillems/Vulkan.

# Building

To build this repository, you need the .NET Core SDK: https://www.microsoft.com/net/core#windowscmd.

```
dotnet restore src\vk.sln
dotnet msbuild src\vk\vk.csproj
dotnet msbuild src\samples\triangle\triangle.csproj
```

The bindings can be built and work on all platforms. Currently, the sample projects only work on Windows.

# Components
### vk.dll

Contains the raw bindings for the Vulkan API. These bindings differ from many other .NET bindings in that they are low-level and unsafe. There is no attempt made to provide a higher-level abstraction on top of Vulkan. This means you must be very careful with usages of the API. On the other hand, it means that you can simply and easily translate from the many C++ examples available on the web.

### vk.generator.dll

Contains parsing and code generation logic for creating the C# bindings for Vulkan.

### vk.rewriter.dll

Contains assembly rewriting logic, using Mono.Cecil, which completes some of the handling for the native calls used by vk.dll.

## Samples

There are several sample projects included under the `src/samples` directory. These have been adapted line-by-line from the projects [here](https://github.com/SaschaWillems/Vulkan).

### [Triangle](https://github.com/mellinoe/vk/blob/master/src/samples/triangle/TriangleExample.cs)

This is the simplest demo, which just renders a colored triangle to the screen.

# Attributions / Licenses

Please note that (some) models and textures use separate licenses. Please comply to these when redistributing or using them in your own projects :

* Cubemap used in cubemap example by Emil Persson(aka Humus)
* Voyager model by NASA
* Hidden treasure scene used in pipeline and debug marker examples by Laurynas Jurgila
