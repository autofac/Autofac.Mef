// This software is part of the Autofac IoC container
// Copyright (c) 2007 - 2008 Autofac Contributors
// https://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            foreach (var m in metadata)
            {
                WithMetadata(m.Key, m.Value);
            }

            return this;
        }
    }
}
