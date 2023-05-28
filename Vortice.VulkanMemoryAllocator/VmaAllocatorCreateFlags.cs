// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace Vortice.Vulkan;

[Flags]
public enum VmaAllocatorCreateFlags : uint
{
    ExternallySynchronized = 0x00000001,
    KHRDedicatedAllocation = 0x00000002,
    KHRBindMemory2 = 0x00000004,
    ExtMemoryBudget = 0x00000008,
    AMDDeviceCoherentMemory = 0x00000010,
    BufferDeviceAddress = 0x00000020,
    ExtMemoryPriority = 0x00000040,
}
