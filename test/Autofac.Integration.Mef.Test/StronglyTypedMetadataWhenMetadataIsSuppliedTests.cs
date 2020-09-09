using System;
using Autofac.Features.Metadata;
using Autofac.Integration.Mef.Test.TestTypes;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class StronglyTypedMetadataWhenMetadataIsSuppliedTests
    {
        private const int SuppliedValue = 123;

        private readonly IContainer _container;

        public StronglyTypedMetadataWhenMetadataIsSuppliedTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterType<object>().WithMetadata("TheInt", SuppliedValue);
            _container = builder.Build();
        }

        [Fact]
        public void ValuesAreProvidedFromMetadata()
        {
            var meta = _container.Resolve<Meta<object, IMeta>>();
            Assert.Equal((int)SuppliedValue, (int)meta.Metadata.TheInt);
        }

        [Fact]
        public void ValuesProvidedFromMetadataOverrideDefaults()
        {
            var meta = _container.Resolve<Meta<object, IMetaWithDefault>>();
            Assert.Equal((int)SuppliedValue, (int)meta.Metadata.TheInt);
        }

        [Fact]
        public void ValuesBubbleUpThroughAdapters()
        {
            var meta = _container.Resolve<Meta<Func<object>, IMeta>>();
            Assert.Equal((int)SuppliedValue, (int)meta.Metadata.TheInt);
        }
    }
}
