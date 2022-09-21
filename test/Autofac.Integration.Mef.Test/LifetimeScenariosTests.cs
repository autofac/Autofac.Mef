// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Autofac.Features.Metadata;
using Autofac.Integration.Mef.Test.TestTypes;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class LifetimeScenariosTests
    {
        [Fact]
        public void ClassRegisteredInAutofacAsFactoryScopedIsResolvedByMefAsFactoryScoped()
        {
            var containerBuilder = new ContainerBuilder();

            using var newAssemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterComposablePartCatalog(newAssemblyCatalog);
            containerBuilder.RegisterType<RegisteredInAutofac>();
            containerBuilder.RegisterType<RegisteredInAutofacAndExported>()
                .Exported(e => e.As<RegisteredInAutofacAndExported>());

            var autofacContainer = containerBuilder.Build();

            var elementFromAutofac1 = autofacContainer.Resolve<RegisteredInAutofac>();
            var elementFromAutofac2 = autofacContainer.Resolve<RegisteredInAutofac>();

            Assert.NotSame(elementFromAutofac1, elementFromAutofac2);
            Assert.NotSame(elementFromAutofac1.ImportedFormMef, elementFromAutofac2.ImportedFormMef);
            Assert.NotSame(elementFromAutofac1.ImportedFormMef.ImportedFormAutofac, elementFromAutofac2.ImportedFormMef.ImportedFormAutofac);
        }

        [Fact]
        public void LazyMetadataRegistrationSourceDoesNotDuplicateDependencies()
        {
            // Issue #23 - configuration action on BeginLifetimeScope causes duplicate resolutions.
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterAssemblyTypes(typeof(LifetimeScenariosTests).Assembly)
                    .AssignableTo<IDependency>()
                    .As<IDependency>()
                    .WithMetadataFrom<IMeta>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var deps = scope.Resolve<IEnumerable<Lazy<IDependency, IMeta>>>();
                Assert.Equal(2, deps.Count());
            }

            using (var scope = container.BeginLifetimeScope(b => { }))
            {
                var deps = scope.Resolve<IEnumerable<Lazy<IDependency, IMeta>>>();
                Assert.Equal(2, deps.Count());
            }
        }

        [Fact]
        public void StronglyTypedMetadataRegistrationSourceDoesNotDuplicateDependencies()
        {
            // Issue #23 - configuration action on BeginLifetimeScope causes duplicate resolutions.
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterAssemblyTypes(typeof(LifetimeScenariosTests).Assembly)
                    .AssignableTo<IDependency>()
                    .As<IDependency>()
                    .WithMetadataFrom<IMeta>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var deps = scope.Resolve<IEnumerable<Meta<IDependency, IMeta>>>();
                Assert.Equal(2, deps.Count());
            }

            using (var scope = container.BeginLifetimeScope(b => { }))
            {
                var deps = scope.Resolve<IEnumerable<Meta<IDependency, IMeta>>>();
                Assert.Equal(2, deps.Count());
            }
        }

        private interface IDependency
        {
        }

        [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
        [Meta(1)]
        private class Dependency1 : IDependency
        {
        }

        [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
        [Meta(2)]
        private class Dependency2 : IDependency
        {
        }

        [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
        private class RegisteredInAutofac
        {
            public ExportedToMefAndImportingFromAutofac ImportedFormMef { get; set; }

            public RegisteredInAutofac(ExportedToMefAndImportingFromAutofac importedFormMef)
            {
                ImportedFormMef = importedFormMef;
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        private class ExportedToMefAndImportingFromAutofac
        {
            [Import]
            public RegisteredInAutofacAndExported ImportedFormAutofac { get; set; }
        }

        [SuppressMessage("CA1812", "CA1812", Justification = "Instantiated by dependency injection.")]
        private class RegisteredInAutofacAndExported
        {
        }
    }
}
