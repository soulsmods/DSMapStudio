// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace Vortice.Vulkan;

/// <summary>
/// Intended usage of the allocated memory.
/// </summary>
public enum VmaMemoryUsage
{
    Unknown = 0,
    [Obsolete]
    GpuOnly = 1,
    [Obsolete]
    CpuOnly = 2,
    [Obsolete]
    CpuToGpuy = 3,
    [Obsolete]
    GpuToGpu = 4,
    [Obsolete]
    CpuCopy = 5,
    GpuLazilyAllocated = 6,
    Auto = 7,
    AutoPreferDevice = 8,
    AutoPreferHost = 9,
}
