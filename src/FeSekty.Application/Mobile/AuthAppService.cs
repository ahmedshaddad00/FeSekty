using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Account;
using Volo.Abp.Identity;

namespace FeSekty.Mobile;

[AllowAnonymous]
public class AuthAppService : FeSektyAppService, IAuthAppService
{
    public const string EndUserRoleName = "AppUser";

    private readonly IAccountAppService _accountAppService;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;

    public AuthAppService(
        IAccountAppService accountAppService,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager)
    {
        _accountAppService = accountAppService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<MobileUserDto> RegisterAsync(MobileRegisterInput input)
    {
        // Honors the ABP setting "Abp.Account.IsSelfRegistrationEnabled" and password policy.
        var created = await _accountAppService.RegisterAsync(new RegisterDto
        {
            UserName = input.UserName,
            EmailAddress = input.EmailAddress,
            Password = input.Password,
            AppName = "FeSekty.Mobile"
        });

        var user = await _userManager.GetByIdAsync(created.Id);

        if (!string.IsNullOrWhiteSpace(input.Name) || !string.IsNullOrWhiteSpace(input.Surname))
        {
            user.Name = input.Name;
            user.Surname = input.Surname;
            Check(await _userManager.UpdateAsync(user));
        }

        if (await _roleManager.FindByNameAsync(EndUserRoleName) == null)
        {
            var role = new IdentityRole(GuidGenerator.Create(), EndUserRoleName, CurrentTenant.Id)
            {
                IsPublic = true
            };
            Check(await _roleManager.CreateAsync(role));
        }

        Check(await _userManager.AddToRoleAsync(user, EndUserRoleName));

        return new MobileUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Roles = new[] { EndUserRoleName }
        };
    }

    private static void Check(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new UserFriendlyException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }
    }
}
