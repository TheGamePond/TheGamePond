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

        builder.Entity<ProductCategory>(entity =>
        {
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(100).IsRequired();
            entity.Property(category => category.Description).HasMaxLength(500);

            entity.HasData(
                new ProductCategory
                {
                    Id = 1,
                    Name = "Video Games",
                    Slug = "video-games",
                    Description = "Physical games across current and retro platforms.",
                    SortOrder = 10,
                    IsActive = true,
                    CreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero)
                },
                new ProductCategory
                {
                    Id = 2,
                    Name = "Consoles",
                    Slug = "consoles",
                    Description = "New, used, and retro systems.",
                    SortOrder = 20,
                    IsActive = true,
                    CreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero)
                },
                new ProductCategory
                {
                    Id = 3,
                    Name = "Accessories",
                    Slug = "accessories",
                    Description = "Controllers, cables, memory cards, cases, and related gear.",
                    SortOrder = 30,
                    IsActive = true,
                    CreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero)
                },
                new ProductCategory
                {
                    Id = 4,
                    Name = "Trading Cards",
                    Slug = "trading-cards",
                    Description = "TCG singles, sealed product, and accessories.",
                    SortOrder = 40,
                    IsActive = true,
                    CreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero)
                },
                new ProductCategory
                {
                    Id = 5,
                    Name = "Collectibles",
                    Slug = "collectibles",
                    Description = "Figures, anime items, collectibles, and community finds.",
                    SortOrder = 50,
                    IsActive = true,
                    CreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero)
                });
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(product => product.Slug).IsUnique();
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.HasIndex(product => product.Barcode);
            entity.Property(product => product.Name).HasMaxLength(160).IsRequired();
            entity.Property(product => product.Slug).HasMaxLength(180).IsRequired();
            entity.Property(product => product.Sku).HasMaxLength(64).IsRequired();
            entity.Property(product => product.Barcode).HasMaxLength(64);
            entity.Property(product => product.Platform).HasMaxLength(80);
            entity.Property(product => product.Franchise).HasMaxLength(120);
            entity.Property(product => product.CostPrice).HasPrecision(18, 2);
            entity.Property(product => product.SalePrice).HasPrecision(18, 2);

            entity
                .HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<InventoryItem>(entity =>
        {
            entity.HasIndex(item => item.ProductId).IsUnique();
            entity.Property(item => item.LocationCode).HasMaxLength(80);

            entity
                .HasOne(item => item.Product)
                .WithOne(product => product.InventoryItem)
                .HasForeignKey<InventoryItem>(item => item.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ProductImage>(entity =>
        {
            entity.Property(image => image.ImagePath).HasMaxLength(260).IsRequired();
            entity.Property(image => image.AltText).HasMaxLength(160);

            entity
                .HasOne(image => image.Product)
                .WithMany(product => product.Images)
                .HasForeignKey(image => image.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<StockAdjustment>(entity =>
        {
            entity.Property(adjustment => adjustment.Notes).HasMaxLength(500);
            entity.Property(adjustment => adjustment.CreatedByUserId).HasMaxLength(450);

            entity
                .HasOne(adjustment => adjustment.Product)
                .WithMany(product => product.StockAdjustments)
                .HasForeignKey(adjustment => adjustment.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
