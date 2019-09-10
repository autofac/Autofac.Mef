using System;
using System.ComponentModel.Composition;
using Autofac.Core;
using Autofac.Integration.Mef.Test.TestTypes;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class LazyWithMetadataWhenNoMatchingMetadataIsSuppliedTests
    {
        private IContainer _container;

        public LazyWithMetadataWhenNoMatchingMetadataIsSuppliedTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterType<object>();
            _container = builder.Build();
        }

        [Fact]
        public void ResolvingStronglyTypedMetadataWithoutDefaultValueThrowsException()
        {
            var dx = Assert.Throws<DependencyResolutionException>(() => _container.Resolve<Lazy<object, IMeta>>());

            Assert.IsType<CompositionContractMismatchException>(dx.InnerException);
        }

        [Fact]
        public void ResolvingStronglyTypedMetadataWithDefaultValueProvidesDefault()
        {
            var m = _container.Resolve<Lazy<object, IMetaWithDefault>>();
            Assert.Equal(42, m.Metadata.TheInt);
        }
    }
}