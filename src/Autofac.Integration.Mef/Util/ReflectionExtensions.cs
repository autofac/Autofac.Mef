// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Autofac.Integration.Mef.Util
{
    /// <summary>
    /// Extension methods for reflection-related types.
    /// </summary>
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Maps from a property-set-value parameter to the declaring property.
        /// </summary>
        /// <param name="pi">Parameter to the property setter.</param>
        /// <param name="prop">The property info on which the setter is specified.</param>
        /// <returns>True if the parameter is a property setter.</returns>
        public static bool TryGetDeclaringProperty(this ParameterInfo pi, out PropertyInfo prop)
        {
            var mi = pi.Member as MethodInfo;
            if (mi != null && mi.IsSpecialName && mi.Name.StartsWith("set_", StringComparison.Ordinal) && mi.DeclaringType != null)
            {
                prop = mi.DeclaringType.GetProperty(mi.Name.Substring(4));
                return true;
            }

            prop = null;
            return false;
        }

        /// <summary>
        /// Get a PropertyInfo object from an expression of the form
        /// x =&gt; x.P.
        /// </summary>
        /// <typeparam name="TDeclaring">Type declaring the property.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyAccessor">Expression mapping an instance of the
        /// declaring type to the property value.</param>
        /// <returns>Property info.</returns>
        public static PropertyInfo GetProperty<TDeclaring, TProperty>(
            Expression<Func<TDeclaring, TProperty>> propertyAccessor)
        {
            if (propertyAccessor == null)
            {
                throw new ArgumentNullException(nameof(propertyAccessor));
            }

            var mex = propertyAccessor.Body as MemberExpression;
            if (!(mex?.Member is PropertyInfo))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    ReflectionExtensionsResources.ExpressionNotPropertyAccessor,
                    propertyAccessor));
            }

            return (PropertyInfo)mex.Member;
        }
    }
}
