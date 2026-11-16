using Volo.Abp.Modularity;

namespace FeSekty;

public abstract class FeSektyApplicationTestBase<TStartupModule> : FeSektyTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
