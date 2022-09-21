// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Autofac.Features.Metadata;
using Autofac.Integration.Mef;
using Autofac.Integration.Mef.Test.TestTypes;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class MetadataRegistrationTests
    {
        [Fact]
        public void RegisterMetadataRegistrationSources_WhenContainerBuilt_AddsStronglyTypedMetaRegistrationSource()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            var container = builder.Build();

            var stronglyTypedMetaCount = container.ComponentRegistry.Sources
                .Count(source => source is StronglyTypedMetadataRegistrationSource);

            Assert.Equal(1, stronglyTypedMetaCount);
        }

        [Fact]
        public void RegisterMetadataRegistrationSources_WhenContainerBuilt_AddsLazyWithMetadataRegistrationSource()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            var container = builder.Build();

            var lazyWithMetadataCount = container.ComponentRegistry.Sources.Count(
            source => source is LazyWithMetadataRegistrationSource);

            Assert.Equal(1, lazyWithMetadataCount);
        }

        [Fact]
        public void WithMetadata_InterfaceBasedMetadata_SupportLazyWithMetadata()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.Register(c => new object()).WithMetadata<IMeta>(m =>
                m.For(value => value.TheInt, 42));
            var container = builder.Build();

            var lazy = container.Resolve<Lazy<object, IMeta>>();

            Assert.Equal(42, lazy.Metadata.TheInt);
            Assert.NotNull(lazy.Value);
        }

        [Fact]
        public void WithMetadata_InterfaceBasedMetadata_SupportMeta()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.Register(c => new object()).WithMetadata<IMeta>(m =>
                m.For(value => value.TheInt, 42));
            var container = builder.Build();

            var meta = container.Resolve<Meta<object, IMeta>>();

            Assert.Equal(42, meta.Metadata.TheInt);
            Assert.NotNull(meta.Value);
        }

        [Fact]
        public void ExcludesExportsWithoutRequiredMetadata()
        {
            var builder = new ContainerBuilder();
            using var catalog = new TypeCatalog(typeof(RequiresMetadataAllowsDefault), typeof(HasNoMetadata));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var rm = container.Resolve<RequiresMetadataAllowsDefault>();
            Assert.Null(rm.Dependency);
        }

        [Fact]
        public void IncludesExportsWithRequiredMetadata()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            using var catalog = new TypeCatalog(typeof(RequiresMetadata), typeof(HasMetadata));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var rm = container.Resolve<RequiresMetadata>();
            Assert.NotNull(rm.Dependency);
        }

        [Fact]
        public void SupportsMetadataOnAutofacExports()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            var metadata = new Dictionary<string, object>
            {
                { "Key", "Value" },
            };
            const string exportedString = "Hello";
            builder.RegisterInstance(exportedString).Exported(e => e.As<string>().WithMetadata(metadata));
            using var catalog = new TypeCatalog(typeof(RequiresMetadata));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var rm = container.Resolve<RequiresMetadata>();
            Assert.NotNull(rm.Dependency);
            Assert.Equal("Hello", rm.Dependency.Value);
        }

        [Fact]
        public void SetsMultipleExportsToZeroOrMoreCardinalityImports()
        {
            var builder = new ContainerBuilder();
            using var catalog = new TypeCatalog(
                typeof(ImportsMany), typeof(HasMetadata), typeof(HasNoMetadata));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var rm = container.Resolve<ImportsMany>();
            Assert.NotNull(rm.Dependencies);
            Assert.Equal(2, rm.Dependencies.Count);
        }

        [SuppressMessage("CA1034", "CA1034", Justification = "Metadata classes must be public for MEF.")]
        public interface IRequiredMetadata
        {
            string Key { get; }
        }

        [Export]
        private class RequiresMetadata
        {
            [Import]
            public Lazy<string, IRequiredMetadata> Dependency { get; set; }
        }

        [Export]
        private class RequiresMetadataAllowsDefault
        {
            [Import(AllowDefault = true)]
            public Lazy<string, IRequiredMetadata> Dependency { get; set; }
        }

        [Export]
        private class ImportsMany
        {
            [ImportMany]
            public Collection<string> Dependencies { get; private set; }
        }

        [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
        private class HasNoMetadata
        {
            [Export]
            public string Service
            {
                get
                {
                    return "Bar";
                }
            }
        }

        [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
        private class HasMetadata
        {
            [Export]
            [ExportMetadata("Key", "Foo")]
            public string Service
            {
                get
                {
                    return "Bar";
                }
            }
        }
    }
}
