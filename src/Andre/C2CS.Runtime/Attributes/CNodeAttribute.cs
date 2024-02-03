// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;

namespace bottlenoselabs.C2CS.Runtime;

/// <summary>
///     Specifies a C node with a specific kind for a C# type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Enum | AttributeTargets.Field)]
public sealed class CNodeAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets the C node kind.
    /// </summary>
    public string Kind { get; set; } = string.Empty;
}
