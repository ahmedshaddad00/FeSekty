using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FeSekty.Mobile;

/// <summary>Health/connectivity check for the mobile app (anonymous).</summary>
public interface ISystemAppService : IApplicationService
{
    Task<PingDto> GetPingAsync();
}

/// <summary>Self-registration for end users. Login itself uses POST /connect/token.</summary>
public interface IAuthAppService : IApplicationService
{
    Task<MobileUserDto> RegisterAsync(MobileRegisterInput input);
}

/// <summary>Profile of the currently logged-in end user.</summary>
public interface IProfileAppService : IApplicationService
{
    Task<MobileUserDto> GetMeAsync();
}
