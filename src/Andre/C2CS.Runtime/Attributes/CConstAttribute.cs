// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;

namespace bottlenoselabs.C2CS.Runtime;

/// <summary>
///     Specifies a C `const` for a parameter or return value.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class CConstAttribute : Attribute
{
    // marker
}
