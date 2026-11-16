using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace FeSekty.Mobile;

public class PingDto
{
    public string Status { get; set; } = "ok";

    public DateTime ServerTimeUtc { get; set; }
}

public class MobileRegisterInput
{
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = default!;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string EmailAddress { get; set; } = default!;

    [Required]
    [StringLength(128)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [StringLength(64)]
    public string? Name { get; set; }

    [StringLength(64)]
    public string? Surname { get; set; }
}

public class MobileUserDto
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = default!;

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Email { get; set; }

    public string[] Roles { get; set; } = Array.Empty<string>();
}
