using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FeSekty.Mobile;

[Authorize(AuthenticationSchemes = "OpenIddict.Validation.AspNetCore")] // bearer only: unauthenticated calls get 401, not a login redirect
public class ProfileAppService : FeSektyAppService, IProfileAppService
{
    public Task<MobileUserDto> GetMeAsync()
    {
        return Task.FromResult(new MobileUserDto
        {
            Id = CurrentUser.Id!.Value,
            UserName = CurrentUser.UserName!,
            Name = CurrentUser.Name,
            Surname = CurrentUser.SurName,
            Email = CurrentUser.Email,
            Roles = CurrentUser.Roles ?? Array.Empty<string>()
        });
    }
}
