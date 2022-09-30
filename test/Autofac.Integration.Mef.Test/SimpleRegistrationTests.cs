// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Autofac.Core.Registration;

namespace Autofac.Integration.Mef.Test;

public class SimpleRegistrationTests
{
    [Fact]
    public void MissingDependencyDetected()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(HasMissingDependency));
        builder.RegisterComposablePartCatalog(catalog);
        var container = builder.Build();
        Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve<HasMissingDependency>());
    }

    [Fact]
    public void RetrievesExportedInterfaceFromCatalogPart()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(MefDependency));
        builder.RegisterComposablePartCatalog(catalog);
        var container = builder.Build();
        var foo = container.Resolve<IDependency>();
        Assert.IsAssignableFrom<MefDependency>(foo);
    }

    [Fact]
    public void SatisfiesImportOnMefComponentFromAutofac()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(ImportsMefDependency));
        builder.RegisterComposablePartCatalog(catalog);
        builder.RegisterType<MefDependency>().Exported(e => e.As<IDependency>());
        var container = builder.Build();
        var bar = container.Resolve<ImportsMefDependency>();
        Assert.NotNull(bar.Dependency);
    }

    [Fact]
    public void SatisfiesImportOnMefComponentFromMef()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(MefDependency), typeof(ImportsMefDependency));
        builder.RegisterComposablePartCatalog(catalog);
        var container = builder.Build();
        var bar = container.Resolve<ImportsMefDependency>();
        Assert.NotNull(bar.Dependency);
    }

    [Fact]
    public void ResolvesExportsFromContext()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(MefDependency));
        builder.RegisterComposablePartCatalog(catalog);
        builder.RegisterType<MefDependency>().Exported(e => e.As<IDependency>());
        var container = builder.Build();
        var exports = container.ResolveExports<IDependency>();
        Assert.Equal(2, exports.Count());
    }

    [Fact]
    public void RestrictsExportsBasedOnValueType()
    {
        var builder = new ContainerBuilder();
        const string n = "name";
        builder.RegisterType<MefDependency>().Exported(e => e.AsNamed<IDependency>(n));
        builder.RegisterType<MefDependency>().Exported(e => e.AsNamed<MefDependency>(n));
        var container = builder.Build();
        var exports = container.ResolveExports<IDependency>(n);
        Assert.Single(exports);
    }

    [Fact]
    public void ObjectExportsSupportedByName()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(ObjectExportDerivedClass), typeof(ObjectExportImporter));
        builder.RegisterComposablePartCatalog(catalog);
        var container = builder.Build();
        var importer = container.Resolve<ObjectExportImporter>();
        Assert.NotNull(importer.Item);
    }

    [Fact]
    public void DuplicateConstructorDependency()
    {
        var builder = new ContainerBuilder();
        using var catalog = new TypeCatalog(typeof(MefDependency), typeof(ImportsMefDependency));
        builder.RegisterType<ImportsDuplicateMefClass>();
        builder.RegisterComposablePartCatalog(catalog);
        var container = builder.Build();
        var resolved = container.Resolve<ImportsDuplicateMefClass>();
        Assert.NotNull(resolved.First);
        Assert.NotNull(resolved.Second);
    }

    private interface IDependency
    {
    }

    [Export(typeof(IDependency))]
    private class MefDependency : IDependency
    {
    }

    [Export]
    private class ImportsMefDependency
    {
        [ImportingConstructor]
        public ImportsMefDependency(IDependency dependency)
        {
            Dependency = dependency;
        }

        public IDependency Dependency { get; private set; }
    }

    [Export]
    private class HasMissingDependency
    {
        [Import]
        public string Dependency { get; set; }
    }

    private class ObjectExportBaseClass
    {
    }

    [Export("contract-name", typeof(object))]
    private class ObjectExportDerivedClass : ObjectExportBaseClass
    {
    }

    [Export]
    private class ObjectExportImporter
    {
        [Import("contract-name")]
        public object Item { get; set; }
    }

    [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
    private class ImportsDuplicateMefClass
    {
        public ImportsMefDependency First { get; set; }

        public ImportsMefDependency Second { get; set; }

        public ImportsDuplicateMefClass(ImportsMefDependency first, ImportsMefDependency second)
        {
            First = first;
            Second = second;
        }
    }
}
