using System.Threading.Tasks;

namespace FeSekty.Data;

public interface IFeSektyDbSchemaMigrator
{
    Task MigrateAsync();
}
