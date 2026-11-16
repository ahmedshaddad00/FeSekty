using FeSekty.Localization;
using Volo.Abp.Application.Services;

namespace FeSekty;

/* Inherit your application services from this class.
 */
public abstract class FeSektyAppService : ApplicationService
{
    protected FeSektyAppService()
    {
        LocalizationResource = typeof(FeSektyResource);
    }
}
