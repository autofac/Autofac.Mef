using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class GenericExportRegistrationTests
    {
        [Fact]
        public void RegisterComposablePartCatalog_GenericExport()
        {
            var container = RegisterTypeCatalogContaining(typeof(IT1), typeof(ITest<>), typeof(Test));
            var b = container.Resolve<ITest<IT1>>();
            Assert.NotNull(b);
            Assert.IsType<Test>(b);
        }

        [Fact(Skip = "Issue #4")]
        public void RegisterComposablePartCatalog_OpenGeneric()
        {
            // Autofac.Core.Registration.ComponentNotRegisteredException:
            // The requested service 'ContractName=Autofac.Integration.Mef.Test.GenericExportRegistrationTests+OpenGenericExport(Autofac.Integration.Mef.Test.GenericExportRegistrationTests+SimpleType)' has not been registered.
            var container = RegisterTypeCatalogContaining(typeof(OpenGenericExport<>), typeof(SimpleType), typeof(OpenGenericConsumer));
            var b = container.Resolve<OpenGenericConsumer>();
            Assert.NotNull(b);
        }

        private static IContainer RegisterTypeCatalogContaining(params Type[] types)
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(types);
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            return container;
        }

        [InheritedExport]
        public interface ITest<T>
        {
        }

        public interface IT1
        {
        }

        public class Test : ITest<IT1>
        {
        }

        public class TestConsumer
        {
            [Import]
            public ITest<IT1> Property { get; set; }
        }

        [Export(typeof(OpenGenericExport<>))]
        public class OpenGenericExport<T>
        {
            [ImportingConstructor]
            public OpenGenericExport(T t)
            {
            }
        }

        [Export]
        public class SimpleType
        {
        }

        [Export]
        public class OpenGenericConsumer
        {
            [ImportingConstructor]
            public OpenGenericConsumer(OpenGenericExport<SimpleType> o)
            {
            }
        }
    }
}
