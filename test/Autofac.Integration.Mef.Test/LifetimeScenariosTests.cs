// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Xunit;

namespace Autofac.Integration.Mef.Test
{
    /// <summary>
    /// See Autofac Issue 128 (courtesy of palpatine@kopernet.org).
    /// </summary>
    public class LifetimeScenariosTests
    {
        public class RegisteredInAutofac2
        {
            public ExportedToMefAndImportingFromAutofac ImportedFormMef { get; set; }

            public RegisteredInAutofac2(ExportedToMefAndImportingFromAutofac importedFormMef)
            {
                ImportedFormMef = importedFormMef;
            }
        }

        [Export]
        [PartCreationPolicy(CreationPolicy.NonShared)]
        public class ExportedToMefAndImportingFromAutofac
        {
            [Import]
            public RegisteredInAutofacAndExported ImportedFormAutofac { get; set; }
        }

        public class RegisteredInAutofacAndExported
        {
        }

        [Fact]
        public void ClassRegisteredInAutofacAsFactoryScopedIsResolvedByMefAsFactoryScoped()
        {
            var containerBuilder = new ContainerBuilder();

            var newAssemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            containerBuilder.RegisterComposablePartCatalog(newAssemblyCatalog);
            containerBuilder.RegisterType<RegisteredInAutofac2>();
            containerBuilder.RegisterType<RegisteredInAutofacAndExported>()
                .Exported(e => e.As<RegisteredInAutofacAndExported>());

            var autofacContainer = containerBuilder.Build();

            var elementFromAutofac1 = autofacContainer.Resolve<RegisteredInAutofac2>();
            var elementFromAutofac2 = autofacContainer.Resolve<RegisteredInAutofac2>();

            Assert.NotSame(elementFromAutofac1, elementFromAutofac2);
            Assert.NotSame(elementFromAutofac1.ImportedFormMef, elementFromAutofac2.ImportedFormMef);
            Assert.NotSame(elementFromAutofac1.ImportedFormMef.ImportedFormAutofac, elementFromAutofac2.ImportedFormMef.ImportedFormAutofac);
        }
    }
}
