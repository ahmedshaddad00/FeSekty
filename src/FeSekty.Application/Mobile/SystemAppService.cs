using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FeSekty.Mobile;

[AllowAnonymous]
public class SystemAppService : FeSektyAppService, ISystemAppService
{
    public Task<PingDto> GetPingAsync()
    {
        return Task.FromResult(new PingDto { Status = "ok", ServerTimeUtc = DateTime.UtcNow });
    }
}
