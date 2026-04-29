using Microsoft.AspNetCore.Identity;
using TheGamePond.Models;

namespace TheGamePond.Data;

public static class IdentitySeedData
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration,
        ILogger logger)
    {
        var ownerEmail = configuration["SeedAdmin:Email"];
        var ownerPassword = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(ownerEmail) || string.IsNullOrWhiteSpace(ownerPassword))
        {
            logger.LogInformation("SeedAdmin credentials were not configured. The owner user was not created.");
            return;
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create role '{roleName}': {FormatErrors(roleResult)}");
                }
            }
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var normalizedEmail = ownerEmail.Trim();
        var ownerUser = await userManager.FindByEmailAsync(normalizedEmail);

        if (ownerUser is null)
        {
            ownerUser = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                EmailConfirmed = true,
                DisplayName = configuration["SeedAdmin:DisplayName"] ?? "The Game Pond Owner"
            };

            var createResult = await userManager.CreateAsync(ownerUser, ownerPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not create seeded owner user '{normalizedEmail}': {FormatErrors(createResult)}");
            }
        }

        if (!await userManager.IsInRoleAsync(ownerUser, AppRoles.Owner))
        {
            var roleResult = await userManager.AddToRoleAsync(ownerUser, AppRoles.Owner);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Could not assign owner role to '{normalizedEmail}': {FormatErrors(roleResult)}");
            }
        }
    }

    private static string FormatErrors(IdentityResult result)
    {
        return string.Join("; ", result.Errors.Select(error => error.Description));
    }
}
