using System.ComponentModel;

namespace Autofac.Integration.Mef.Test
{
    public interface IMeta
    {
        int TheInt { get; }
    }

    public interface IMetaWithDefault
    {
        [DefaultValue(42)]
        int TheInt { get; }
    }
}
