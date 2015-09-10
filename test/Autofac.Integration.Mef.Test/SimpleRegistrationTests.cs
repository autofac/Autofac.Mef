using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Autofac.Core.Registration;
using Autofac.Integration.Mef;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class SimpleRegistrationTests
    {
        [Fact]
        public void MissingDependencyDetected()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(HasMissingDependency));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve<HasMissingDependency>());
        }

        [Fact]
        public void RetrievesExportedInterfaceFromCatalogPart()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(MefDependency));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var foo = container.Resolve<IDependency>();
            Assert.IsAssignableFrom<MefDependency>(foo);
        }

        [Fact]
        public void SatisfiesImportOnMefComponentFromAutofac()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportsMefDependency));
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
            var catalog = new TypeCatalog(typeof(MefDependency), typeof(ImportsMefDependency));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var bar = container.Resolve<ImportsMefDependency>();
            Assert.NotNull(bar.Dependency);
        }

        [Fact]
        public void ResolvesExportsFromContext()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(MefDependency));
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
            Assert.Equal(1, exports.Count());
        }

        [Fact]
        public void ObjectExportsSupportedByName()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ObjectExportDerivedClass), typeof(ObjectExportImporter));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var importer = container.Resolve<ObjectExportImporter>();
            Assert.NotNull(importer.Item);
        }

        [Fact]
        public void DuplicateConstructorDependency()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(MefDependency), typeof(ImportsMefDependency));
            builder.RegisterType<ImportsDuplicateMefClass>();
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var resolved = container.Resolve<ImportsDuplicateMefClass>();
            Assert.NotNull(resolved.First);
            Assert.NotNull(resolved.Second);
        }

        public interface IDependency
        {
        }

        [Export(typeof(IDependency))]
        public class MefDependency : IDependency
        {
        }

        [Export]
        public class ImportsMefDependency
        {
            [ImportingConstructor]
            public ImportsMefDependency(IDependency dependency)
            {
                Dependency = dependency;
            }

            public IDependency Dependency { get; private set; }
        }

        [Export]
        public class HasMissingDependency
        {
            [Import]
            public string Dependency { get; set; }
        }

        public class ObjectExportBaseClass
        {
        }

        [Export("contract-name", typeof(object))]
        public class ObjectExportDerivedClass : ObjectExportBaseClass
        {
        }

        [Export]
        public class ObjectExportImporter
        {
            [Import("contract-name")]
            public object Item { get; set; }
        }

        public class ImportsDuplicateMefClass
        {
            public ImportsMefDependency First { get; set; }

            public ImportsMefDependency Second { get; set; }

            public ImportsDuplicateMefClass(ImportsMefDependency first, ImportsMefDependency second)
            {
                this.First = first;
                this.Second = second;
            }
        }
    }
}
