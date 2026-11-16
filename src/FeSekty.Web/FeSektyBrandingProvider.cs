using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using FeSekty.Localization;

namespace FeSekty.Web;

[Dependency(ReplaceServices = true)]
public class FeSektyBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FeSektyResource> _localizer;

    public FeSektyBrandingProvider(IStringLocalizer<FeSektyResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];

    public override string? LogoUrl => "~/images/logo/leptonxlite/logo-light-thumbnail.png";

    public override string? LogoIconUrl => "~/images/logo/leptonxlite/logo-dark-thumbnail.png";
}
