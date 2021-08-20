// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.ComponentModel.Composition;

namespace Autofac.Integration.Mef.Test.TestTypes
{
    [MetadataAttribute]
    public class MetaAttribute : Attribute, IMeta
    {
        public MetaAttribute(int value) => TheInt = value;

        public int TheInt { get; private set; }
    }
}
