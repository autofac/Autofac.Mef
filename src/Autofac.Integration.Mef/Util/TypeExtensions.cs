// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Integration.Mef.Util;

/// <summary>
/// Extension methods for working with <see cref="System.Type"/>.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Determines if a closed generic is an implementation of a given open generic.
    /// </summary>
    /// <param name="this">
    /// The closed generic type to check.
    /// </param>
    /// <param name="openGeneric">
    /// The open generic type to compare against the closed generic.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="this"/> is a closed generic version of
    /// <paramref name="openGeneric"/>; otherwise <see langword="false"/>.
    /// </returns>
    internal static bool IsGenericTypeDefinedBy(this Type @this, Type openGeneric)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        if (openGeneric == null)
        {
            throw new ArgumentNullException(nameof(openGeneric));
        }

        return !@this.ContainsGenericParameters && @this.IsGenericType && @this.GetGenericTypeDefinition() == openGeneric;
    }
}
