// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Autofac.Util;

namespace Autofac.Integration.Mef.Test
{
    public class DisposalRegistrationTests
    {
        [Fact]
        public void DefaultLifetimeForMefComponentsIsSingleton()
        {
            var builder = new ContainerBuilder();
            using var catalog = new TypeCatalog(typeof(HasDefaultCreationPolicy));
            builder.RegisterComposablePartCatalog(catalog);
            AssertDisposalTrackerIsSingleton(builder);
        }

        [Fact]
        public void RespectsSharedCreationPolicy()
        {
            var builder = new ContainerBuilder();
            using var catalog = new TypeCatalog(typeof(HasSharedCreationPolicy));
            builder.RegisterComposablePartCatalog(catalog);
            AssertDisposalTrackerIsSingleton(builder);
        }

        [Fact]
        public void AnyCreationPolicyDefaultsToShared()
        {
            var builder = new ContainerBuilder();
            using var catalog = new TypeCatalog(typeof(HasAnyCreationPolicy));
            builder.RegisterComposablePartCatalog(catalog);
            AssertDisposalTrackerIsSingleton(builder);
        }

        private static void AssertDisposalTrackerIsSingleton(ContainerBuilder builder)
        {
            var container = builder.Build();
            var instance1 = container.Resolve<DisposalTracker>();
            var instance2 = container.Resolve<DisposalTracker>();
            Assert.Same(instance1, instance2);
            Assert.False(instance1.IsDisposedPublic);
            container.Dispose();
            Assert.True(instance1.IsDisposedPublic);
        }

        [Fact]
        public void RespectsNonSharedCreationPolicy()
        {
            var builder = new ContainerBuilder();
            using var catalog = new TypeCatalog(typeof(HasNonSharedCreationPolicy));
            builder.RegisterComposablePartCatalog(catalog);
            var container = builder.Build();
            var instance1 = container.Resolve<DisposalTracker>();
            var instance2 = container.Resolve<DisposalTracker>();
            Assert.IsAssignableFrom<HasNonSharedCreationPolicy>(instance1);
            Assert.NotSame(instance1, instance2);
            Assert.False(instance1.IsDisposedPublic);
            Assert.False(instance2.IsDisposedPublic);
            container.Dispose();
            Assert.True(instance1.IsDisposedPublic);
            Assert.True(instance2.IsDisposedPublic);
        }

        private class DisposalTracker : Disposable
        {
            public bool IsDisposedPublic
            {
                get
                {
                    return IsDisposed;
                }
            }
        }

        [Export(typeof(DisposalTracker))]
        private class HasDefaultCreationPolicy : DisposalTracker
        {
        }

        [PartCreationPolicy(CreationPolicy.Any)]
        [Export(typeof(DisposalTracker))]
        private class HasAnyCreationPolicy : DisposalTracker
        {
        }

        [PartCreationPolicy(CreationPolicy.Shared)]
        [Export(typeof(DisposalTracker))]
        private class HasSharedCreationPolicy : DisposalTracker
        {
        }

        [PartCreationPolicy(CreationPolicy.NonShared)]
        [Export(typeof(DisposalTracker))]
        private class HasNonSharedCreationPolicy : DisposalTracker
        {
        }
    }
}
