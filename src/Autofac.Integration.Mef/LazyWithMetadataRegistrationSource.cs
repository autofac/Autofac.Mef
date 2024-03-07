// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using System.Reflection;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mef.Util;

namespace Autofac.Integration.Mef;

/// <summary>
/// Support the <see cref="Lazy{T, TMetadata}"/>
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
    private static readonly MethodInfo CreateLazyRegistrationMethod = typeof(LazyWithMetadataRegistrationSource).GetMethod(nameof(CreateLazyRegistration), BindingFlags.Static | BindingFlags.NonPublic)!;

    private delegate IComponentRegistration RegistrationCreator(Service service, ServiceRegistration valueRegistration);

    /// <inheritdoc />
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

    /// <inheritdoc />
    public bool IsAdapterForIndividualComponents
    {
        get
        {
            return false;
        }
    }

    /// <inheritdoc />
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
