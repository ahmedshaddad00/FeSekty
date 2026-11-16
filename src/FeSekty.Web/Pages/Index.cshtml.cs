using Microsoft.AspNetCore.Mvc;

namespace FeSekty.Web.Pages;

public class IndexModel : FeSektyPageModel
{
    public IActionResult OnGet()
    {
        // Signed-in users go to the admin dashboard; visitors see the public landing page.
        if (CurrentUser.IsAuthenticated)
        {
            return RedirectToPage("/Dashboard");
        }

        return Page();
    }
}
