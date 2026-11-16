using FeSekty.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FeSekty.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FeSektyEntityFrameworkCoreModule),
    typeof(FeSektyApplicationContractsModule)
)]
public class FeSektyDbMigratorModule : AbpModule
{
}
