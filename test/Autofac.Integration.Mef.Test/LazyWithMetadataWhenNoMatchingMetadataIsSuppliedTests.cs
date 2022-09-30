// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using Autofac.Core;
using Autofac.Integration.Mef.Test.TestTypes;

namespace Autofac.Integration.Mef.Test;

public class LazyWithMetadataWhenNoMatchingMetadataIsSuppliedTests
{
    private readonly IContainer _container;

    public LazyWithMetadataWhenNoMatchingMetadataIsSuppliedTests()
    {
        var builder = new ContainerBuilder();
        builder.RegisterMetadataRegistrationSources();
        builder.RegisterType<object>();
        _container = builder.Build();
    }

    [Fact]
    public void ResolvingStronglyTypedMetadataWithoutDefaultValueThrowsException()
    {
        var dx = Assert.Throws<DependencyResolutionException>(() => _container.Resolve<Lazy<object, IMeta>>());

        Assert.IsType<CompositionContractMismatchException>(dx.InnerException);
    }

    [Fact]
    public void ResolvingStronglyTypedMetadataWithDefaultValueProvidesDefault()
    {
        var m = _container.Resolve<Lazy<object, IMetaWithDefault>>();
        Assert.Equal(42, m.Metadata.TheInt);
    }
}
