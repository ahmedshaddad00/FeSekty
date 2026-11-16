using Volo.Abp.Modularity;

namespace FeSekty;

[DependsOn(
    typeof(FeSektyApplicationModule),
    typeof(FeSektyDomainTestModule)
)]
public class FeSektyApplicationTestModule : AbpModule
{

}
