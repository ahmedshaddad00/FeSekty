using Volo.Abp.Modularity;

namespace FeSekty;

[DependsOn(
    typeof(FeSektyDomainModule),
    typeof(FeSektyTestBaseModule)
)]
public class FeSektyDomainTestModule : AbpModule
{

}
