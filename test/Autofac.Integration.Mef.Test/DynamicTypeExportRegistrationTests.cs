using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using Autofac.Core.Registration;
using Autofac.Integration.Mef.Test.TestTypes;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class DynamicTypeExportRegistrationTests
    {
        [Fact]
        public void ImportManyFromAutofacExports()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportManyDependency));
            builder.RegisterComposablePartCatalog(catalog);
            RegisterAutofacDependencyUsingInterface(builder, true);
            var container = builder.Build();
            var resolve = container.Resolve<ImportManyDependency>();
            Assert.Equal(2, resolve.Dependencies.Count());
        }

        [Fact]
        public void ImportWithMetadataFromAutofacExports()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportWithMetadataDependency));
            builder.RegisterComposablePartCatalog(catalog);
            const int metaInt = 10;
            RegisterAutofacDependencyUsingAttribute(builder, "", metaInt);
            var container = builder.Build();
            var resolve = container.Resolve<ImportWithMetadataDependency>();
            Assert.NotNull(resolve);
            Assert.NotNull(resolve.Dependency.Value);
            Assert.Equal(metaInt, resolve.Dependency.Metadata.TheInt);
        }

        [Fact]
        public void RestrictsExportsBasedOnValueType()
        {
            var builder = new ContainerBuilder();
            const string n = "name";
            RegisterAutofacDependencyUsingAttribute(builder, n);
            var container = builder.Build();
            var exports = container.ResolveExports<IAutofacDependency>(n);
            Assert.Empty(exports);
        }

        [Fact]
        public void DuplicateConstructorDependencyImportUsingAttribute()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportsDuplicateAutofacDependency));
            builder.RegisterComposablePartCatalog(catalog);
            RegisterAutofacDependencyUsingAttribute(builder);
            var container = builder.Build();
            var resolved = container.Resolve<ImportsDuplicateAutofacDependency>();
            Assert.NotNull(resolved.First);
            Assert.NotNull(resolved.Second);
        }

        [Fact]
        public void DuplicateConstructorDependencyImportUsingInterface()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportsDuplicateAutofacDependency));
            builder.RegisterComposablePartCatalog(catalog);
            RegisterAutofacDependencyUsingInterface(builder);
            var container = builder.Build();
            var resolved = container.Resolve<ImportsDuplicateAutofacDependency>();
            Assert.NotNull(resolved.First);
            Assert.NotNull(resolved.Second);
        }

        [Fact]
        public void MixedDependencyConstructorDependencyImport()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportsMixedAutofacMefDependency), typeof(MefDependency));
            builder.RegisterComposablePartCatalog(catalog);
            RegisterAutofacDependencyUsingAttribute(builder);
            var container = builder.Build();
            var resolved = container.Resolve<ImportsMixedAutofacMefDependency>();
            Assert.NotNull(resolved.First);
            Assert.NotNull(resolved.Second);
        }

        [Theory]
        public static void RegisterAutofacDependencyUsingAttribute(ContainerBuilder builder, string name = "", int metaInt = 0)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                Attribute.GetCustomAttribute(type, typeof(ExportFromAutofacAttribute)) != null))
            {
                builder.RegisterType(type).Exported(x =>
                {
                    if (metaInt > 0)
                    {
                        x.As(type).WithMetadata("TheInt", metaInt);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(name))
                            x.As(type);
                        else x.AsNamed(type, name);
                    }
                });
            }
        }

        [Theory]
        public static void RegisterAutofacDependencyUsingInterface(
            ContainerBuilder builder,
            bool registerAsInterfaceType = false)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                typeof(IAutofacDependency).IsAssignableFrom(type) && !type.IsInterface))
            {
                builder.RegisterType(type)
                    .Exported(x => x.As(registerAsInterfaceType ? typeof(IAutofacDependency) : type));
            }
        }

        public interface IAutofacDependency
        {
        }

        [ExportFromAutofac]
        public class ExportFromAutofacDependencyA : IAutofacDependency
        {
        }

        [ExportFromAutofac]
        public class ExportFromAutofacDependencyB : IAutofacDependency
        {
        }

        public interface IDependency
        {
        }

        [Export(typeof(IDependency))]
        public class MefDependency : IDependency
        {
        }

        [Export]
        public class ImportManyDependency
        {
            [ImportMany]
            public IEnumerable<IAutofacDependency> Dependencies { get; set; }
        }

        [Export]
        public class ImportWithMetadataDependency
        {
            [Import]
            public Lazy<ExportFromAutofacDependencyB, IMetaWithDefault> Dependency { get; set; }
        }

        [Export]
        public class ImportsDuplicateAutofacDependency
        {
            public ExportFromAutofacDependencyA First { get; }

            public ExportFromAutofacDependencyB Second { get; }

            [ImportingConstructor]
            public ImportsDuplicateAutofacDependency(ExportFromAutofacDependencyA first, ExportFromAutofacDependencyB second)
            {
                First = first;
                Second = second;
            }
        }

        [Export]
        public class ImportsMixedAutofacMefDependency
        {
            public ExportFromAutofacDependencyA First { get; }

            public IDependency Second { get; }

            [ImportingConstructor]
            public ImportsMixedAutofacMefDependency(ExportFromAutofacDependencyA first, IDependency second)
            {
                First = first;
                Second = second;
            }
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
        public sealed class ExportFromAutofacAttribute : Attribute
        {
        }
    }
}
