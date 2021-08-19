// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Autofac.Integration.Mef.Util
{
    internal static class TypeExtensions
    {
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
}
