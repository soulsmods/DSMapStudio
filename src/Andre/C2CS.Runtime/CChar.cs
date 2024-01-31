// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace bottlenoselabs.C2CS.Runtime;

/// <summary>
///     A value type with the same memory layout as a <see cref="byte" /> in a managed context and <c>char</c> in
///     an unmanaged context.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct CChar : IEquatable<byte>, IEquatable<CChar>
{
    public readonly byte Value;

    private CChar(byte value)
    {
        Value = Convert.ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="byte" /> to a <see cref="CChar" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CChar" />.</returns>
    public static implicit operator CChar(byte value)
    {
        return FromByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="byte" /> to a <see cref="CChar" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CChar" />.</returns>
    public static CChar FromByte(byte value)
    {
        return new CChar(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CChar" /> to a <see cref="byte" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="byte" />.</returns>
    public static implicit operator byte(CChar value)
    {
        return ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CChar" /> to a <see cref="byte" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="byte" />.</returns>
    public static byte ToByte(CChar value)
    {
        return value.Value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CChar value && Equals(value);
    }

    /// <inheritdoc />
    public bool Equals(byte other)
    {
        return Value == other;
    }

    /// <inheritdoc />
    public bool Equals(CChar other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CChar" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CChar" /> to compare.</param>
    /// <param name="right">The second <see cref="CChar" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(CChar left, CChar right)
    {
        return left.Value == right.Value;
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CChar" /> structures are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="CChar" /> to compare.</param>
    /// <param name="right">The second <see cref="CChar" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CChar left, CChar right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CChar" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CChar" /> to compare.</param>
    /// <param name="right">The second <see cref="CChar" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool Equals(CChar left, CChar right)
    {
        return left.Value == right.Value;
    }
}
