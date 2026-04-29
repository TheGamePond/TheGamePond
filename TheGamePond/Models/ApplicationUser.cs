using Microsoft.AspNetCore.Identity;

namespace TheGamePond.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
