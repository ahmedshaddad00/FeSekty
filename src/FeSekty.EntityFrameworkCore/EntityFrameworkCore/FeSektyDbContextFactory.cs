using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FeSekty.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class FeSektyDbContextFactory : IDesignTimeDbContextFactory<FeSektyDbContext>
{
    public FeSektyDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        FeSektyEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<FeSektyDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new FeSektyDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FeSekty.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
