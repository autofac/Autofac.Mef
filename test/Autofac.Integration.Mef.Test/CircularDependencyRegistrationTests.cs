// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Autofac.Integration.Mef.Test;

public class CircularDependencyRegistrationTests
{
    [Fact]
    public void HandlesLazyMefNonPrerequisiteCircularity1()
    {
        var container = RegisterTypeCatalogContaining(typeof(LazyCircularA), typeof(LazyCircularB));
        var a = container.Resolve<LazyCircularA>();
        Assert.NotNull(a);
        Assert.NotNull(a.B);
        Assert.Same(a, a.B.Value.A.Value);
    }

    [Fact]
    public void HandlesLazyMefNonPrerequisiteCircularity2()
    {
        var container = RegisterTypeCatalogContaining(typeof(LazyCircularA), typeof(LazyCircularB));
        var b = container.Resolve<LazyCircularB>();
        Assert.NotNull(b);
        Assert.NotNull(b.A);
        Assert.Same(b, b.A.Value.B.Value);
    }

    private static IContainer RegisterTypeCatalogContaining(params Type[] types)
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(types);
        builder.RegisterComposablePartCatalog(catalog);
        var container = builder.Build();
        return container;
    }

    [Fact]
    public void HandlesEagerMefNonPrerequisiteCircularity1()
    {
        var container = RegisterTypeCatalogContaining(typeof(EagerCircularA), typeof(EagerCircularB));
        var a = container.Resolve<EagerCircularA>();
        Assert.NotNull(a);
        Assert.NotNull(a.B);
        Assert.Same(a, a.B.A);
        Assert.Same(a.B, a.B.A.B);
    }

    [Fact]
    public void HandlesEagerMefNonPrerequisiteCircularity2()
    {
        var container = RegisterTypeCatalogContaining(typeof(EagerCircularA), typeof(EagerCircularB));
        var b = container.Resolve<EagerCircularB>();
        Assert.NotNull(b);
        Assert.NotNull(b.A);
        Assert.Same(b, b.A.B);
        Assert.Same(b.A, b.A.B.A);
    }

    [Export]
    private class LazyCircularA
    {
        [ImportingConstructor]
        public LazyCircularA(Lazy<LazyCircularB> b)
        {
            B = b;
        }

        public Lazy<LazyCircularB> B { get; private set; }
    }

    [Export]
    private class LazyCircularB
    {
        [Import]
        public Lazy<LazyCircularA> A { get; set; }
    }

    /* Non-lazy circular dependencies in MEF have to be done with
     * properties. Constructor parameters will throw a MEF composition
     * exception. */

    [Export]
    private class EagerCircularA
    {
        [Import]
        public EagerCircularB B { get; private set; }
    }

    [Export]
    private class EagerCircularB
    {
        [Import]
        public EagerCircularA A { get; set; }
    }
}
