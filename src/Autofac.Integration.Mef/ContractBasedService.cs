﻿// This software is part of the Autofac IoC container
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
        /// <exception cref="NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
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
