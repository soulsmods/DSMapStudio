// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Runtime.InteropServices;

namespace bottlenoselabs.C2CS.Runtime;

/// <summary>
///     Utility methods for interoperability with C style strings in C#.
/// </summary>
public static unsafe class CStrings
{
    /// <summary>
    ///     Converts an array of strings to an array of C strings of type `char` (multi-dimensional array of one
    ///     dimensional byte arrays each terminated by a <c>0x0</c>) by allocating and copying if not already cached.
    /// </summary>
    /// <remarks>
    ///     <para>Calls <see cref="CString" />.</para>
    /// </remarks>
    /// <param name="values">The strings.</param>
    /// <returns>An array pointer of C string pointers. You are responsible for freeing the returned pointer.</returns>
    public static CString* CStringArray(ReadOnlySpan<string> values)
    {
        var pointerSize = IntPtr.Size;
        var result = (CString*)Marshal.AllocHGlobal(pointerSize * values.Length);
        for (var i = 0; i < values.Length; ++i)
        {
            var @string = values[i];
            var cString = CString.FromString(@string);
            result[i] = cString;
        }

        return result;
    }

    /// <summary>
    ///     Converts an array of strings to an array of C strings of type `wchar_t` (multi-dimensional array of one
    ///     dimensional ushort arrays each terminated by a <c>0x0</c>) by allocating and copying if not already cached.
    /// </summary>
    /// <remarks>
    ///     <para>Calls <see cref="CString" />.</para>
    /// </remarks>
    /// <param name="values">The strings.</param>
    /// <returns>An array pointer of C string pointers. You are responsible for freeing the returned pointer.</returns>
    public static CStringWide* CStringWideArray(ReadOnlySpan<string> values)
    {
        var pointerSize = IntPtr.Size;
        var result = (CStringWide*)Marshal.AllocHGlobal(pointerSize * values.Length);
        for (var i = 0; i < values.Length; ++i)
        {
            var @string = values[i];
            var cString = CStringWide.FromString(@string);
            result[i] = cString;
        }

        return result;
    }
}
