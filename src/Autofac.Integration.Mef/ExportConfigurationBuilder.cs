// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Autofac.Integration.Mef
{
    /// <summary>
    /// Configures an Export on an Autofac component.
    /// </summary>
    public class ExportConfigurationBuilder
    {
        internal string ContractName { get; private set; }

        internal IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        internal string ExportTypeIdentity { get; private set; }

        /// <summary>
        /// Export the component under typed contract <typeparamref name="TContract"/>.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <returns>Builder for additional configuration.</returns>
        public ExportConfigurationBuilder As<TContract>()
        {
            WithMetadata(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(typeof(TContract)));
            ContractName = AttributedModelServices.GetContractName(typeof(TContract));
            return this;
        }

        /// <summary>
        /// Export the component under typed contract <paramref name="contractType"/>.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <returns>Builder for additional configuration.</returns>
        public ExportConfigurationBuilder As(Type contractType)
        {
            WithMetadata(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(contractType));
            ContractName = AttributedModelServices.GetContractName(contractType);
            return this;
        }

        /// <summary>
        /// Export the component under named contract <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="TExportedValue">Exported value type.</typeparam>
        /// <param name="name">Contract name.</param>
        /// <returns>Builder for additional configuration.</returns>
        public ExportConfigurationBuilder AsNamed<TExportedValue>(string name)
        {
            ContractName = name ?? throw new ArgumentNullException(nameof(name));
            WithMetadata(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(typeof(TExportedValue)));
            return this;
        }

        /// <summary>
        /// Export the component under named contract <paramref name="name"/>.
        /// </summary>
        /// <param name="exportedValueType">Exported value type.</param>
        /// <param name="name">Contract name.</param>
        /// <returns>Builder for additional configuration.</returns>
        public ExportConfigurationBuilder AsNamed(Type exportedValueType, string name)
        {
            ContractName = name ?? throw new ArgumentNullException(nameof(name));
            WithMetadata(CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(exportedValueType));
            return this;
        }

        /// <summary>
        /// Add metadata to the export.
        /// </summary>
        /// <param name="key">Metadata key.</param>
        /// <param name="value">Metadata value.</param>
        /// <returns>Builder for additional configuration.</returns>
        public ExportConfigurationBuilder WithMetadata(string key, object value)
        {
            Metadata.Add(key, value);
            if (key == CompositionConstants.ExportTypeIdentityMetadataName)
            {
                ExportTypeIdentity = (string)value ?? throw new ArgumentNullException(nameof(value));
            }

            return this;
        }

        /// <summary>
        /// Add metadata to the export.
        /// </summary>
        /// <param name="metadata">Metadata.</param>
        /// <returns>Builder for additional configuration.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="metadata" /> is <see langword="null" />.
        /// </exception>
        public ExportConfigurationBuilder WithMetadata(IEnumerable<KeyValuePair<string, object>> metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            foreach (var m in metadata)
            {
                WithMetadata(m.Key, m.Value);
            }

            return this;
        }
    }
}
