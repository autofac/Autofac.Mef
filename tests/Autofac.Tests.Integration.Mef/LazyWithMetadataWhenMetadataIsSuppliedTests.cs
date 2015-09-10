using System;
using System.ComponentModel.Composition;
using Autofac.Core;
using Autofac.Integration.Mef;
using Xunit;

namespace Autofac.Tests.Integration.Mef
{
    public class LazyWithMetadataWhenMetadataIsSuppliedTests
    {
        const int SuppliedValue = 123;

        IContainer _container;

        public LazyWithMetadataWhenMetadataIsSuppliedTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterType<object>().WithMetadata("TheInt", SuppliedValue);
            _container = builder.Build();
        }

        [Fact]
        public void ValuesAreProvidedFromMetadata()
        {
            var meta = _container.Resolve<Lazy<object, IMeta>>();
            Assert.Equal(SuppliedValue, meta.Metadata.TheInt);
        }

        [Fact]
        public void ValuesProvidedFromMetadataOverrideDefaults()
        {
            var meta = _container.Resolve<Lazy<object, IMetaWithDefault>>();
            Assert.Equal(SuppliedValue, meta.Metadata.TheInt);
        }

        [Fact]
        public void ValuesBubbleUpThroughAdapters()
        {
            var meta = _container.Resolve<Lazy<Func<object>, IMeta>>();
            Assert.Equal(SuppliedValue, meta.Metadata.TheInt);
        }
    }
}