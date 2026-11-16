using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FeSekty.Data;
using Volo.Abp.DependencyInjection;

namespace FeSekty.EntityFrameworkCore;

public class EntityFrameworkCoreFeSektyDbSchemaMigrator
    : IFeSektyDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreFeSektyDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the FeSektyDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<FeSektyDbContext>()
            .Database
            .MigrateAsync();
    }
}
