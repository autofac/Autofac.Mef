# Autofac.Mef

Managed Extensibility Framework (MEF) integration for [Autofac](https://autofac.org).

[![Build status](https://ci.appveyor.com/api/projects/status/404h0j4gj3qyn09a?svg=true)](https://ci.appveyor.com/project/Autofac/autofac-bwvcu)

Please file issues and pull requests for this package [in this repository](https://github.com/autofac/Autofac.Mef/issues) rather than in the Autofac core repo.

- [Documentation](https://autofac.readthedocs.io/en/latest/integration/mef.html)
- [NuGet](https://www.nuget.org/packages/Autofac.Mef)
- [Contributing](https://autofac.readthedocs.io/en/latest/contributors.html)
- [Open in Visual Studio Code](https://open.vscode.dev/autofac/Autofac.Mef)

## Quick Start

The Autofac/MEF integration allows MEF catalogs to be registered with the `ContainerBuilder` using the `RegisterComposablePartCatalog()` extension method. If you register a component using MEF and want to provide Autofac components into that MEF component, use the `Exported()` extension.

```c#
var builder = new ContainerBuilder();
var catalog = new DirectoryCatalog(@"C:\MyExtensions");
builder.RegisterComposablePartCatalog(catalog);
builder.RegisterType<Component>()
       .Exported(x => x.As<IService>().WithMetadata("SomeData", 42));
```

Check out the [Autofac MEF integration documentation](https://autofac.readthedocs.io/en/latest/integration/mef.html) for more information.

## Get Help

**Need help with Autofac?** We have [a documentation site](https://autofac.readthedocs.io/) as well as [API documentation](https://autofac.org/apidoc/). We're ready to answer your questions on [Stack Overflow](https://stackoverflow.com/questions/tagged/autofac) or check out the [discussion forum](https://groups.google.com/forum/#forum/autofac).
