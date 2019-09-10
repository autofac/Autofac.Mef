using System.ComponentModel;

namespace Autofac.Integration.Mef.Test.TestTypes
{
    public interface IMetaWithDefault
    {
        [DefaultValue(42)]
        int TheInt { get; }
    }
}
