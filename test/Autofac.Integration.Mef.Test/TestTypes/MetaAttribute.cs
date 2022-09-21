// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel.Composition;

namespace Autofac.Integration.Mef.Test.TestTypes
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MetaAttribute : Attribute, IMeta
    {
        public MetaAttribute(int theInt) => TheInt = theInt;

        public int TheInt { get; private set; }
    }
}
