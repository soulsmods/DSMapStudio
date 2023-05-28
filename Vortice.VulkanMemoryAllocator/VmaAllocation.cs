// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Diagnostics;

namespace Vortice.Vulkan;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaAllocation : IEquatable<VmaAllocation>
{
    public VmaAllocation(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static VmaAllocation Null => new(0);
    public static implicit operator VmaAllocation(nint handle) => new(handle);
    public static bool operator ==(VmaAllocation left, VmaAllocation right) => left.Handle == right.Handle;
    public static bool operator !=(VmaAllocation left, VmaAllocation right) => left.Handle != right.Handle;
    public static bool operator ==(VmaAllocation left, nint right) => left.Handle == right;
    public static bool operator !=(VmaAllocation left, nint right) => left.Handle != right;
    public bool Equals(VmaAllocation other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmaAllocation handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(VmaAllocation)} [0x{Handle.ToString("X")}]";
}
