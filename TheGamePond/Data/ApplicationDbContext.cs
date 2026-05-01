using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Models;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var seedCreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.DisplayName).HasMaxLength(100);
            });

        builder.Entity<ProductCategory>(entity =>
        {
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(120);
            entity.Property(category => category.Description).HasMaxLength(500);
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.HasIndex(product => product.Barcode);
            entity.HasIndex(product => new { product.ProductCategoryId, product.IsActive });
            entity.Property(product => product.Name).HasMaxLength(160).IsRequired();
            entity.Property(product => product.Sku).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Barcode).HasMaxLength(80);
            entity.Property(product => product.Platform).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Condition).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(1200);
            entity.Property(product => product.CostPrice).HasColumnType("numeric(10,2)");
            entity.Property(product => product.SalePrice).HasColumnType("numeric(10,2)");
            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.Property(image => image.FilePath).HasMaxLength(300).IsRequired();
            entity.Property(image => image.AltText).HasMaxLength(180);
            entity.HasOne(image => image.Product)
                .WithMany(product => product.Images)
                .HasForeignKey(image => image.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<InventoryItem>(entity =>
        {
            entity.HasIndex(item => item.ProductId).IsUnique();
            entity.HasOne(item => item.Product)
                .WithOne(product => product.InventoryItem)
                .HasForeignKey<InventoryItem>(item => item.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StockAdjustment>(entity =>
        {
            entity.HasIndex(adjustment => new { adjustment.ProductId, adjustment.CreatedAt });
            entity.Property(adjustment => adjustment.Reason).HasMaxLength(120).IsRequired();
            entity.Property(adjustment => adjustment.AdjustedByUserId).HasMaxLength(450);
            entity.HasOne(adjustment => adjustment.Product)
                .WithMany(product => product.StockAdjustments)
                .HasForeignKey(adjustment => adjustment.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductCategory>().HasData(
            new ProductCategory
            {
                Id = 1,
                Name = "Trading Cards",
                Slug = "trading-cards",
                Description = "TCG singles, sealed products, and collectible cards.",
                CreatedAt = seedCreatedAt
            },
            new ProductCategory
            {
                Id = 2,
                Name = "Video Games",
                Slug = "video-games",
                Description = "Retro, modern, and used video games.",
                CreatedAt = seedCreatedAt
            },
            new ProductCategory
            {
                Id = 3,
                Name = "Collectibles",
                Slug = "collectibles",
                Description = "Anime, figures, accessories, and pond-worthy finds.",
                CreatedAt = seedCreatedAt
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
