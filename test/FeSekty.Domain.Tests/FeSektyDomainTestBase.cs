using Volo.Abp.Modularity;

namespace FeSekty;

/* Inherit from this class for your domain layer tests. */
public abstract class FeSektyDomainTestBase<TStartupModule> : FeSektyTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
