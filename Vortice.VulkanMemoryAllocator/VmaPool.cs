// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Diagnostics;

namespace Vortice.Vulkan;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct VmaPool : IEquatable<VmaPool>
{
    public VmaPool(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static VmaPool Null => new(0);
    public static implicit operator VmaPool(nint handle) => new(handle);
    public static bool operator ==(VmaPool left, VmaPool right) => left.Handle == right.Handle;
    public static bool operator !=(VmaPool left, VmaPool right) => left.Handle != right.Handle;
    public static bool operator ==(VmaPool left, nint right) => left.Handle == right;
    public static bool operator !=(VmaPool left, nint right) => left.Handle != right;
    public bool Equals(VmaPool other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmaPool handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(VmaPool)} [0x{Handle.ToString("X")}]";
}
