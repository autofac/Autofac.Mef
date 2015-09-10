using System.ComponentModel.Composition;
using Autofac.Core;
using Autofac.Features.Metadata;
using Autofac.Integration.Mef;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class StronglyTypedMetadataWhenNoMatchingMetadataIsSuppliedTests
    {
        IContainer _container;

        public StronglyTypedMetadataWhenNoMatchingMetadataIsSuppliedTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterType<object>();
            _container = builder.Build();
        }

        [Fact]
        public void ResolvingStronglyTypedMetadataWithoutDefaultValueThrowsException()
        {
            var dx = Assert.Throws<DependencyResolutionException>(() => _container.Resolve<Meta<object, IMeta>>());
            Assert.IsType<CompositionContractMismatchException>(dx.InnerException);
        }

        [Fact]
        public void ResolvingStronglyTypedMetadataWithDefaultValueProvidesDefault()
        {
            var m = _container.Resolve<Meta<object, IMetaWithDefault>>();
            Assert.Equal((int)42, (int)m.Metadata.TheInt);
        }
    }
}