using FeSekty.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FeSekty.Web.Pages;

public abstract class FeSektyPageModel : AbpPageModel
{
    protected FeSektyPageModel()
    {
        LocalizationResourceType = typeof(FeSektyResource);
    }
}
