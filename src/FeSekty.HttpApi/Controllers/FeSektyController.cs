using FeSekty.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FeSekty.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FeSektyController : AbpControllerBase
{
    protected FeSektyController()
    {
        LocalizationResource = typeof(FeSektyResource);
    }
}
