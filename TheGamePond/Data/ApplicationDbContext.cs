using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Models;
using TheGamePond.Models.Catalog;
using TheGamePond.Models.Orders;
using TheGamePond.Models.TradeIns;

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

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<PaymentEvent> PaymentEvents => Set<PaymentEvent>();

    public DbSet<OrderStatusHistory> OrderStatusHistory => Set<OrderStatusHistory>();

    public DbSet<TradeInRequest> TradeInRequests => Set<TradeInRequest>();

    public DbSet<TradeInRequestItem> TradeInRequestItems => Set<TradeInRequestItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var seedCreatedAt = new DateTimeOffset(2026, 4, 29, 0, 0, 0, TimeSpan.Zero);

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

        builder.Entity<ProductCategory>(entity =>
        {
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(80).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(120).IsRequired();
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
                    CreatedAt = seedCreatedAt
                },
                new ProductCategory
                {
                    Id = 2,
                    Name = "Consoles",
                    Slug = "consoles",
                    Description = "New, used, and retro systems.",
                    SortOrder = 20,
                    IsActive = true,
                    CreatedAt = seedCreatedAt
                },
                new ProductCategory
                {
                    Id = 3,
                    Name = "Accessories",
                    Slug = "accessories",
                    Description = "Controllers, cables, memory cards, cases, and related gear.",
                    SortOrder = 30,
                    IsActive = true,
                    CreatedAt = seedCreatedAt
                },
                new ProductCategory
                {
                    Id = 4,
                    Name = "Trading Cards",
                    Slug = "trading-cards",
                    Description = "TCG singles, sealed product, and accessories.",
                    SortOrder = 40,
                    IsActive = true,
                    CreatedAt = seedCreatedAt
                },
                new ProductCategory
                {
                    Id = 5,
                    Name = "Collectibles",
                    Slug = "collectibles",
                    Description = "Figures, anime items, collectibles, and community finds.",
                    SortOrder = 50,
                    IsActive = true,
                    CreatedAt = seedCreatedAt
                });
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(product => product.Slug).IsUnique();
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.HasIndex(product => product.Barcode);
            entity.Property(product => product.Name).HasMaxLength(160).IsRequired();
            entity.Property(product => product.Slug).HasMaxLength(180).IsRequired();
            entity.Property(product => product.Description).HasMaxLength(4000);
            entity.Property(product => product.Sku).HasMaxLength(80).IsRequired();
            entity.Property(product => product.Barcode).HasMaxLength(80);
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

        builder.Entity<Order>(entity =>
        {
            entity.HasIndex(order => order.OrderNumber).IsUnique();
            entity.HasIndex(order => order.PaymentSessionId);
            entity.Property(order => order.OrderNumber).HasMaxLength(32).IsRequired();
            entity.Property(order => order.PaymentProvider).HasMaxLength(80);
            entity.Property(order => order.PaymentSessionId).HasMaxLength(200);
            entity.Property(order => order.CustomerName).HasMaxLength(120).IsRequired();
            entity.Property(order => order.CustomerEmail).HasMaxLength(180).IsRequired();
            entity.Property(order => order.CustomerPhone).HasMaxLength(40);
            entity.Property(order => order.ShippingAddressLine1).HasMaxLength(180).IsRequired();
            entity.Property(order => order.ShippingAddressLine2).HasMaxLength(180);
            entity.Property(order => order.ShippingCity).HasMaxLength(90).IsRequired();
            entity.Property(order => order.ShippingState).HasMaxLength(60).IsRequired();
            entity.Property(order => order.ShippingPostalCode).HasMaxLength(20).IsRequired();
            entity.Property(order => order.ShippingCountry).HasMaxLength(80).IsRequired();
            entity.Property(order => order.CustomerNotes).HasMaxLength(1000);
            entity.Property(order => order.TrackingNumber).HasMaxLength(120);
            entity.Property(order => order.StaffNotes).HasMaxLength(1000);
            entity.Property(order => order.Subtotal).HasPrecision(18, 2);
            entity.Property(order => order.ShippingTotal).HasPrecision(18, 2);
            entity.Property(order => order.TaxTotal).HasPrecision(18, 2);
            entity.Property(order => order.Total).HasPrecision(18, 2);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.Property(item => item.ProductName).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Sku).HasMaxLength(64).IsRequired();
            entity.Property(item => item.ProductSlug).HasMaxLength(180);
            entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
            entity.Property(item => item.LineTotal).HasPrecision(18, 2);

            entity
                .HasOne(item => item.Order)
                .WithMany(order => order.Items)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PaymentEvent>(entity =>
        {
            entity.HasIndex(paymentEvent => new { paymentEvent.Provider, paymentEvent.ProviderEventId }).IsUnique();
            entity.Property(paymentEvent => paymentEvent.Provider).HasMaxLength(80).IsRequired();
            entity.Property(paymentEvent => paymentEvent.ProviderEventId).HasMaxLength(200).IsRequired();
            entity.Property(paymentEvent => paymentEvent.ProviderSessionId).HasMaxLength(200);
            entity.Property(paymentEvent => paymentEvent.RawPayload).HasMaxLength(2000);

            entity
                .HasOne(paymentEvent => paymentEvent.Order)
                .WithMany(order => order.PaymentEvents)
                .HasForeignKey(paymentEvent => paymentEvent.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasIndex(history => new { history.OrderId, history.CreatedAt });
            entity.Property(history => history.ChangedByUserId).HasMaxLength(450);
            entity.Property(history => history.Notes).HasMaxLength(1000);

            entity
                .HasOne(history => history.Order)
                .WithMany(order => order.StatusHistory)
                .HasForeignKey(history => history.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TradeInRequest>(entity =>
        {
            entity.HasIndex(request => request.RequestNumber).IsUnique();
            entity.Property(request => request.RequestNumber).HasMaxLength(32).IsRequired();
            entity.Property(request => request.CustomerName).HasMaxLength(120).IsRequired();
            entity.Property(request => request.CustomerEmail).HasMaxLength(180).IsRequired();
            entity.Property(request => request.CustomerPhone).HasMaxLength(40);
            entity.Property(request => request.PreferredContactMethod).HasMaxLength(80);
            entity.Property(request => request.CustomerNotes).HasMaxLength(1000);
            entity.Property(request => request.StaffNotes).HasMaxLength(1000);
            entity.Property(request => request.EstimatedOfferLow).HasPrecision(18, 2);
            entity.Property(request => request.EstimatedOfferHigh).HasPrecision(18, 2);
        });

        builder.Entity<TradeInRequestItem>(entity =>
        {
            entity.Property(item => item.ItemName).HasMaxLength(160).IsRequired();
            entity.Property(item => item.Platform).HasMaxLength(80);
            entity.Property(item => item.Condition).HasMaxLength(80);
            entity.Property(item => item.Notes).HasMaxLength(500);

            entity
                .HasOne(item => item.TradeInRequest)
                .WithMany(request => request.Items)
                .HasForeignKey(item => item.TradeInRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
