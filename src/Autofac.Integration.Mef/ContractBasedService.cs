// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using Autofac.Core;

namespace Autofac.Integration.Mef
{
    /// <summary>
    /// Identifies a service by the MEF contract name it supports.
    /// </summary>
    public class ContractBasedService : Service
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractBasedService"/> class.
        /// </summary>
        /// <param name="contractName">The contract name.</param>
        /// <param name="exportTypeIdentity">Type identity of the objects exported under the contract.</param>
        public ContractBasedService(string contractName, string exportTypeIdentity)
        {
            if (string.IsNullOrEmpty(contractName))
            {
                throw new ArgumentOutOfRangeException(nameof(contractName));
            }

            if (exportTypeIdentity == null)
            {
                // Issue 310: System.ComponentModel.Composition.Hosting.ExportProvider.BuildImportDefinition
                // has a special clause where it will only build the type identity for an import if the type
                // is not System.Object. We need to put that back to handle object export/import.
                exportTypeIdentity = "System.Object";
            }

            ExportTypeIdentity = exportTypeIdentity;

            ContractName = contractName;
        }

        /// <summary>
        /// Gets the type identity of the objects exported under the contract.
        /// </summary>
        public string ExportTypeIdentity { get; }

        /// <summary>
        /// Gets the name of the contract.
        /// </summary>
        /// <value>The name of the contract.</value>
        public string ContractName { get; }

        /// <summary>
        /// Gets a human-readable description of the service.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, ContractBasedServiceResources.DescriptionFormat, ContractName);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="object"/> is equal to the current <see cref="object"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj)
        {
            var that = obj as ContractBasedService;

            if (that == null)
            {
                return false;
            }

            return ContractName == that.ContractName && ExportTypeIdentity == that.ExportTypeIdentity;
        }

        /// <summary>
        /// Serves as a hash function for a particular ExportDefinition.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return ContractName.GetHashCode() ^ ExportTypeIdentity.GetHashCode();
        }
    }
}
