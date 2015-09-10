using System;
using System.ComponentModel.Composition;
using Autofac.Core;
using Autofac.Features.Metadata;
using Autofac.Integration.Mef;
using Xunit;

namespace Autofac.Tests.Integration.Mef
{
    public class StronglyTypedMetadataWhenMetadataIsSuppliedTests
    {
        const int SuppliedValue = 123;

        IContainer _container;

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