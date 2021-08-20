// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Autofac.Integration.Mef.Test.TestTypes;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    public class LazyWithMetadataWhenMetadataIsSuppliedTests
    {
        private const int SuppliedValue = 123;

        private readonly IContainer _container;

        public LazyWithMetadataWhenMetadataIsSuppliedTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterMetadataRegistrationSources();
            builder.RegisterType<object>().WithMetadata("TheInt", SuppliedValue);

            var catalog = new TypeCatalog(typeof(ThrowingService), typeof(ServiceConsumer));
            builder.RegisterComposablePartCatalog(catalog);
            _container = builder.Build();
        }

        public interface INameMetadata
        {
            string Name { get; }
        }

        internal interface IService
        {
        }

        [Fact(Skip = "Issue #1")]
        public void InstanceShouldNotBeCreated()
        {
            // Issue #1: Lazy dependencies shouldn't be instantiated in the Lazy<T, TMetadata> relationship.
            var serviceConsumer = _container.Resolve<ServiceConsumer>();
            var service = serviceConsumer.Services?.FirstOrDefault(x => x.Metadata.Name == "will-throw-on-ctor");
            Assert.NotNull(service);
            Assert.False(service.IsValueCreated);
        }

        [Fact]
        public void ValuesAreProvidedFromMetadata()
        {
            var meta = _container.Resolve<Lazy<object, IMeta>>();
            Assert.Equal(SuppliedValue, meta.Metadata.TheInt);
        }

        [Fact]
        public void ValuesBubbleUpThroughAdapters()
        {
            var meta = _container.Resolve<Lazy<Func<object>, IMeta>>();
            Assert.Equal(SuppliedValue, meta.Metadata.TheInt);
        }

        [Fact]
        public void ValuesProvidedFromMetadataOverrideDefaults()
        {
            var meta = _container.Resolve<Lazy<object, IMetaWithDefault>>();
            Assert.Equal(SuppliedValue, meta.Metadata.TheInt);
        }

        [Export]
        internal class ServiceConsumer
        {
            [ImportMany]
            public IEnumerable<Lazy<IService, INameMetadata>> Services { get; set; }
        }

        [Export(typeof(IService))]
        [ExportMetadata("Name", "will-throw-on-ctor")]
        internal class ThrowingService : IService
        {
            public ThrowingService()
            {
                throw new Exception("This service should never be created");
            }
        }
    }
}
