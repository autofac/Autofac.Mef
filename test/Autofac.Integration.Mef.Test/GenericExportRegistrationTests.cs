using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Autofac.Integration.Mef;
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
    }
}
