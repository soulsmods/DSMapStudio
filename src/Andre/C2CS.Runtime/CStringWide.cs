// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Runtime.InteropServices;

namespace bottlenoselabs.C2CS.Runtime;

/// <summary>
///     A pointer value type that represents a wide string; C type `wchar_t*`.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct CStringWide : IEquatable<CStringWide>
{
    public readonly nint Pointer;

    /// <summary>
    ///     Gets a value indicating whether this <see cref="CStringWide" /> is a null pointer.
    /// </summary>
    public bool IsNull => Pointer == 0;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CStringWide" /> struct.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    public CStringWide(byte* value)
    {
        Pointer = (nint)value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CStringWide" /> struct.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    public CStringWide(nint value)
    {
        Pointer = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CStringWide" /> struct.
    /// </summary>
    /// <param name="s">The string value.</param>
    public CStringWide(string s)
    {
        Pointer = FromString(s).Pointer;
    }

    /// <summary>
    ///     Performs an explicit conversion from a <see cref="IntPtr" /> to a <see cref="CStringWide" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CStringWide" />.
    /// </returns>
    public static explicit operator CStringWide(nint value)
    {
        return FromIntPtr(value);
    }

    /// <summary>
    ///     Performs an explicit conversion from a <see cref="IntPtr" /> to a <see cref="CStringWide" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CStringWide" />.
    /// </returns>
    public static CStringWide FromIntPtr(nint value)
    {
        return new CStringWide(value);
    }

    /// <summary>
    ///     Performs an implicit conversion from a byte pointer to a <see cref="CStringWide" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CStringWide" />.
    /// </returns>
    public static implicit operator CStringWide(byte* value)
    {
        return From(value);
    }

    /// <summary>
    ///     Performs an implicit conversion from a byte pointer to a <see cref="CStringWide" />.
    /// </summary>
    /// <param name="value">The pointer value.</param>
    /// <returns>
    ///     The resulting <see cref="CStringWide" />.
    /// </returns>
    public static CStringWide From(byte* value)
    {
        return new CStringWide((nint)value);
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="CStringWide" /> to a <see cref="IntPtr" />.
    /// </summary>
    /// <param name="value">The pointer.</param>
    /// <returns>
    ///     The resulting <see cref="IntPtr" />.
    /// </returns>
    public static implicit operator nint(CStringWide value)
    {
        return value.Pointer;
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="CStringWide" /> to a <see cref="IntPtr" />.
    /// </summary>
    /// <param name="value">The pointer.</param>
    /// <returns>
    ///     The resulting <see cref="IntPtr" />.
    /// </returns>
    public static nint ToIntPtr(CStringWide value)
    {
        return value.Pointer;
    }

    /// <summary>
    ///     Performs an explicit conversion from a <see cref="CStringWide" /> to a <see cref="string" />.
    /// </summary>
    /// <param name="value">The <see cref="CStringWide" />.</param>
    /// <returns>
    ///     The resulting <see cref="string" />.
    /// </returns>
    public static explicit operator string(CStringWide value)
    {
        return ToString(value);
    }

    /// <summary>
    ///     Converts a C style string (unicode) of type `wchar_t` (one dimensional ushort array
    ///     terminated by a <c>0x0</c>) to a UTF-16 <see cref="string" /> by allocating and copying.
    /// </summary>
    /// <param name="value">A pointer to the C string.</param>
    /// <returns>A <see cref="string" /> equivalent of <paramref name="value" />.</returns>
    public static string ToString(CStringWide value)
    {
        if (value.IsNull)
        {
            return string.Empty;
        }

        // calls ASM/C/C++ functions to calculate length and then "FastAllocate" the string with the GC
        // https://mattwarren.org/2016/05/31/Strings-and-the-CLR-a-Special-Relationship/
        var result = Marshal.PtrToStringUni(value.Pointer);

        if (string.IsNullOrEmpty(result))
        {
            return string.Empty;
        }

        return result;
    }

    /// <summary>
    ///     Performs an explicit conversion from a <see cref="string" /> to a <see cref="CStringWide" />.
    /// </summary>
    /// <param name="s">The <see cref="string" />.</param>
    /// <returns>
    ///     The resulting <see cref="CStringWide" />.
    /// </returns>
    public static explicit operator CStringWide(string s)
    {
        return FromString(s);
    }

    /// <summary>
    ///     Converts a C string pointer (one dimensional byte array terminated by a
    ///     <c>0x0</c>) for a specified <see cref="string" /> by allocating and copying if not already cached.
    /// </summary>
    /// <param name="str">The <see cref="string" />.</param>
    /// <returns>A C string pointer.</returns>
    public static CStringWide FromString(string str)
    {
        var pointer = Marshal.StringToHGlobalUni(str);
        return new CStringWide(pointer);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return ToString(this);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is CStringWide value && Equals(value);
    }

    /// <inheritdoc />
    public bool Equals(CStringWide other)
    {
        return Pointer == other.Pointer;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Pointer.GetHashCode();
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CStringWide" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CStringWide" /> to compare.</param>
    /// <param name="right">The second <see cref="CStringWide" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(CStringWide left, CStringWide right)
    {
        return left.Pointer == right.Pointer;
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CBool" /> structures are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="CStringWide" /> to compare.</param>
    /// <param name="right">The second <see cref="CStringWide" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(CStringWide left, CStringWide right)
    {
        return !(left == right);
    }

    /// <summary>
    ///     Returns a value that indicates whether two specified <see cref="CStringWide" /> structures are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CStringWide" /> to compare.</param>
    /// <param name="right">The second <see cref="CStringWide" /> to compare.</param>
    /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
    public static bool Equals(CStringWide left, CStringWide right)
    {
        return left.Pointer == right.Pointer;
    }
}
