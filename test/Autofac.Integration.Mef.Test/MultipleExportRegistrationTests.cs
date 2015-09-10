using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Autofac.Core;
using Autofac.Integration.Mef;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class MultipleExportRegistrationTests
    {
        [Fact]
        public void ImportsEmptyCollectionIfDependencyMissing()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(ImportsMany));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var im = container.Resolve<ImportsMany>();
            Assert.NotNull(im.Dependencies);
            Assert.False(im.Dependencies.Any());
        }

        [Fact]
        public void RespectsExplicitInterchangeServices()
        {
            var builder = new ContainerBuilder();
            var catalog = new TypeCatalog(typeof(HasMultipleExports));

            var interchangeService1 = new TypedService(typeof(HasMultipleExportsBase));
            var interchangeService2 = new KeyedService("b", typeof(HasMultipleExports));
            var nonInterchangeService1 = new TypedService(typeof(HasMultipleExports));
            var nonInterchangeService2 = new KeyedService("a", typeof(HasMultipleExports));

            builder.RegisterComposablePartCatalog(catalog,
                interchangeService1,
                interchangeService2);

            var container = builder.Build();

            Assert.True(container.IsRegisteredService(interchangeService1));
            Assert.True(container.IsRegisteredService(interchangeService2));
            Assert.False(container.IsRegisteredService(nonInterchangeService1));
            Assert.False(container.IsRegisteredService(nonInterchangeService2));
        }

        public class HasMultipleExportsBase
        {
        }

        [Export("a"),
        Export("b"),
        Export(typeof(HasMultipleExportsBase)),
        Export(typeof(HasMultipleExports))]
        public class HasMultipleExports : HasMultipleExportsBase
        {
        }

        [Export]
        public class ImportsMany
        {
            [ImportMany]
            public List<string> Dependencies { get; set; }
        }
    }
}
