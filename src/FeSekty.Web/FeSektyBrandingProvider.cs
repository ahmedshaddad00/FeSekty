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

    // The LeptonX sidebar is always dark, so the sidebar logo is the white wordmark.
    public override string? LogoUrl => "~/images/brand/wordmark-white.png";

    public override string? LogoIconUrl => "~/images/brand/logo-icon.png";
}
