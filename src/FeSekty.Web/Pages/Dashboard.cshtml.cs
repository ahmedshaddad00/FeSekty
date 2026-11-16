using Microsoft.AspNetCore.Authorization;

namespace FeSekty.Web.Pages;

[Authorize]
public class DashboardModel : FeSektyPageModel
{
    public void OnGet()
    {
    }
}
