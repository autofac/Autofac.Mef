﻿// This software is part of the Autofac IoC container
// Copyright (c) 2007 - 2008 Autofac Contributors
// https://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mef.Util;

namespace Autofac.Integration.Mef
{
    /// <summary>
    /// Support the <see cref="System.Lazy{T, TMetadata}"/>
    /// types automatically whenever type T is registered with the container.
    /// </summary>
    /// <remarks>
    /// Metadata values come from the component registration's metadata.
    /// When a dependency of a lazy type is used, the instantiation of the underlying
    /// component will be delayed until the <see cref="Lazy{T}.Value"/> property
    /// is first accessed.
    /// </remarks>
    internal class LazyWithMetadataRegistrationSource : IRegistrationSource
    {
        private static readonly MethodInfo CreateLazyRegistrationMethod = typeof(LazyWithMetadataRegistrationSource).GetMethod("CreateLazyRegistration", BindingFlags.Static | BindingFlags.NonPublic);

        private delegate IComponentRegistration RegistrationCreator(Service service, ServiceRegistration valueRegistration);

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            if (registrationAccessor == null)
            {
                throw new ArgumentNullException(nameof(registrationAccessor));
            }

            if (!(service is IServiceWithType swt) || !swt.ServiceType.IsGenericTypeDefinedBy(typeof(Lazy<,>)))
            {
                return Enumerable.Empty<IComponentRegistration>();
            }

            var valueType = swt.ServiceType.GetGenericArguments()[0];
            var metaType = swt.ServiceType.GetGenericArguments()[1];
            var valueService = swt.ChangeType(valueType);
            var registrationCreator = (RegistrationCreator)Delegate.CreateDelegate(
                typeof(RegistrationCreator),
                CreateLazyRegistrationMethod.MakeGenericMethod(valueType, metaType));

            return registrationAccessor(valueService)
                .Select(v => registrationCreator(service, v));
        }

        public bool IsAdapterForIndividualComponents
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return LazyWithMetadataRegistrationSourceResources.LazyWithMetadataRegistrationSourceDescription;
        }

        /// <summary>
        /// Lazy registration creator called via reflection by the source
        /// to generate a <see cref="Lazy{T, TMetadata}"/> component.
        /// </summary>
        /// <typeparam name="T">The type of service being resolved.</typeparam>
        /// <typeparam name="TMetadata">The type of metadata object associated with the service.</typeparam>
        /// <param name="providedService">The service for which the component registration is being generated.</param>
        /// <param name="valueRegistration">The registration that should provide the component value.</param>
        /// <returns>
        /// An <see cref="IComponentRegistration"/> containing a <see cref="Lazy{T, TMetadata}"/>.
        /// </returns>
        [SuppressMessage("IDE0051", "IDE0051", Justification = "Method is consumed via reflection in static member variable in this class.")]
        private static IComponentRegistration CreateLazyRegistration<T, TMetadata>(Service providedService, ServiceRegistration valueRegistration)
        {
            var rb = RegistrationBuilder.ForDelegate(
                (c, p) =>
                {
                    var context = c.Resolve<IComponentContext>();
                    return new Lazy<T, TMetadata>(
                        () => (T)context.ResolveComponent(new ResolveRequest(providedService, valueRegistration, p)),
                        AttributedModelServices.GetMetadataView<TMetadata>(valueRegistration.Registration.Target.Metadata));
                })
                .As(providedService);

            return rb.CreateRegistration();
        }
    }
}
