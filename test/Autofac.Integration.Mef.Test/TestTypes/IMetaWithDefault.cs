// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.ComponentModel;

namespace Autofac.Integration.Mef.Test.TestTypes
{
    public interface IMetaWithDefault
    {
        [DefaultValue(42)]
        int TheInt { get; }
    }
}
