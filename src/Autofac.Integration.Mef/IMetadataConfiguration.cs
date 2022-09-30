// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Integration.Mef;

/// <summary>
/// Implementors are able to provide metadata for a component.
/// </summary>
public interface IMetadataConfiguration
{
    /// <summary>
    /// Gets the metadata properties and values.
    /// </summary>
    IEnumerable<KeyValuePair<string, object>> Properties { get; }
}
