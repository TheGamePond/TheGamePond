using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Models;

namespace TheGamePond.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.DisplayName).HasMaxLength(100);
        });

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = AppRoles.OwnerId,
                Name = AppRoles.Owner,
                NormalizedName = AppRoles.Owner.ToUpperInvariant(),
                ConcurrencyStamp = AppRoles.OwnerId
            },
            new IdentityRole
            {
                Id = AppRoles.AdminId,
                Name = AppRoles.Admin,
                NormalizedName = AppRoles.Admin.ToUpperInvariant(),
                ConcurrencyStamp = AppRoles.AdminId
            },
            new IdentityRole
            {
                Id = AppRoles.StaffId,
                Name = AppRoles.Staff,
                NormalizedName = AppRoles.Staff.ToUpperInvariant(),
                ConcurrencyStamp = AppRoles.StaffId
            });
    }
}
