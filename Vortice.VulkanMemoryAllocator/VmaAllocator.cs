// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Diagnostics;

namespace Vortice.Vulkan;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaAllocator : IEquatable<VmaAllocator>
{
    public VmaAllocator(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static VmaAllocator Null => new(0);
    public static implicit operator VmaAllocator(nint handle) => new(handle);
    public static bool operator ==(VmaAllocator left, VmaAllocator right) => left.Handle == right.Handle;
    public static bool operator !=(VmaAllocator left, VmaAllocator right) => left.Handle != right.Handle;
    public static bool operator ==(VmaAllocator left, nint right) => left.Handle == right;
    public static bool operator !=(VmaAllocator left, nint right) => left.Handle != right;
    public bool Equals(VmaAllocator other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmaAllocator handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(VmaAllocator)} [0x{Handle.ToString("X")}]";
}
