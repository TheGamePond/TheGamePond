using Microsoft.EntityFrameworkCore;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Data;

public static class MockInventorySeedData
{
    private const string MockSkuPrefix = "TGP-MOCK-";
    private const string PlaceholderImage = "/images/brand/the-game-pond-logo.png";

    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var now = DateTimeOffset.UtcNow;

        var categories = await context.ProductCategories
            .Where(category => category.Slug != null)
            .ToDictionaryAsync(category => category.Slug!);
        var seedProducts = BuildProducts();
        var created = 0;
        var updated = 0;

        foreach (var seed in seedProducts)
        {
            if (!categories.TryGetValue(seed.CategorySlug, out var category))
            {
                logger.LogWarning("Mock inventory skipped {Sku}; category {CategorySlug} was not found.", seed.Sku, seed.CategorySlug);
                continue;
            }

            var product = await context.Products
                .Include(item => item.InventoryItem)
                .Include(item => item.Images)
                .FirstOrDefaultAsync(item => item.Sku == seed.Sku);

            if (product is null)
            {
                product = new Product
                {
                    Name = seed.Name,
                    Slug = seed.Slug,
                    Description = seed.Description,
                    Sku = seed.Sku,
                    Barcode = seed.Barcode,
                    Platform = seed.Platform,
                    Franchise = seed.Franchise,
                    Condition = seed.Condition,
                    Status = seed.Status,
                    CostPrice = seed.CostPrice,
                    SalePrice = seed.SalePrice,
                    CategoryId = category.Id,
                    CreatedAt = now,
                    InventoryItem = new InventoryItem
                    {
                        QuantityOnHand = seed.QuantityOnHand,
                        LowStockThreshold = seed.LowStockThreshold,
                        LocationCode = seed.LocationCode,
                        CreatedAt = now
                    },
                    Images =
                    {
                        new ProductImage
                        {
                            ImagePath = PlaceholderImage,
                            AltText = seed.Name,
                            IsPrimary = true,
                            SortOrder = 0,
                            CreatedAt = now
                        }
                    }
                };

                context.Products.Add(product);
                created++;
            }
            else
            {
                product.Name = seed.Name;
                product.Slug = seed.Slug;
                product.Description = seed.Description;
                product.Barcode = seed.Barcode;
                product.Platform = seed.Platform;
                product.Franchise = seed.Franchise;
                product.Condition = seed.Condition;
                product.Status = seed.Status;
                product.CostPrice = seed.CostPrice;
                product.SalePrice = seed.SalePrice;
                product.CategoryId = category.Id;
                product.UpdatedAt = now;

                product.InventoryItem ??= new InventoryItem
                {
                    ProductId = product.Id,
                    CreatedAt = now
                };

                product.InventoryItem.QuantityOnHand = seed.QuantityOnHand;
                product.InventoryItem.LowStockThreshold = seed.LowStockThreshold;
                product.InventoryItem.LocationCode = seed.LocationCode;
                product.InventoryItem.UpdatedAt = now;

                if (!product.Images.Any())
                {
                    product.Images.Add(new ProductImage
                    {
                        ImagePath = PlaceholderImage,
                        AltText = seed.Name,
                        IsPrimary = true,
                        SortOrder = 0,
                        CreatedAt = now
                    });
                }

                updated++;
            }
        }

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Mock inventory seed complete. Created {CreatedCount}, updated {UpdatedCount}.",
            created,
            updated);
    }

    public static async Task DeleteAsync(IServiceProvider services, ILogger logger)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var mockProducts = await context.Products
            .Where(product => product.Sku.StartsWith(MockSkuPrefix))
            .ToListAsync();

        if (mockProducts.Count == 0)
        {
            logger.LogInformation("No mock inventory products found to delete.");
            return;
        }

        context.Products.RemoveRange(mockProducts);
        await context.SaveChangesAsync();

        logger.LogInformation("Mock inventory cleanup complete. Deleted {DeletedCount} mock products.", mockProducts.Count);
    }

    private static IReadOnlyList<MockProductSeed> BuildProducts()
    {
        return new List<MockProductSeed>
        {
            new("Video Games", "video-games", "Super Mario Odyssey", "super-mario-odyssey-switch", "TGP-MOCK-GAME-001", "000100000001", "Nintendo Switch", "Mario", ProductCondition.VeryGood, 24.00m, 39.99m, 8, 2, "VG-SW-01"),
            new("Video Games", "video-games", "The Legend of Zelda: Breath of the Wild", "zelda-breath-of-the-wild-switch", "TGP-MOCK-GAME-002", "000100000002", "Nintendo Switch", "Zelda", ProductCondition.Good, 28.00m, 44.99m, 5, 2, "VG-SW-02"),
            new("Video Games", "video-games", "Pokemon Scarlet", "pokemon-scarlet-switch", "TGP-MOCK-GAME-003", "000100000003", "Nintendo Switch", "Pokemon", ProductCondition.LikeNew, 29.00m, 46.99m, 3, 2, "VG-SW-03"),
            new("Video Games", "video-games", "Halo 3", "halo-3-xbox-360", "TGP-MOCK-GAME-004", "000100000004", "Xbox 360", "Halo", ProductCondition.Good, 4.00m, 11.99m, 12, 3, "VG-XB-01"),
            new("Video Games", "video-games", "Final Fantasy VII Remake", "final-fantasy-vii-remake-ps4", "TGP-MOCK-GAME-005", "000100000005", "PlayStation 4", "Final Fantasy", ProductCondition.VeryGood, 12.00m, 24.99m, 6, 2, "VG-PS-01"),
            new("Consoles", "consoles", "Nintendo Switch OLED Console", "nintendo-switch-oled-console", "TGP-MOCK-CON-001", "000200000001", "Nintendo Switch", "Nintendo", ProductCondition.VeryGood, 190.00m, 269.99m, 2, 1, "CON-SW-01"),
            new("Consoles", "consoles", "PlayStation 2 Slim Console", "playstation-2-slim-console", "TGP-MOCK-CON-002", "000200000002", "PlayStation 2", "Sony", ProductCondition.Good, 55.00m, 94.99m, 1, 1, "CON-PS-01"),
            new("Consoles", "consoles", "Xbox Series S Console", "xbox-series-s-console", "TGP-MOCK-CON-003", "000200000003", "Xbox Series", "Xbox", ProductCondition.LikeNew, 145.00m, 219.99m, 2, 1, "CON-XB-01"),
            new("Accessories", "accessories", "Wireless Switch Controller", "wireless-switch-controller", "TGP-MOCK-ACC-001", "000300000001", "Nintendo Switch", "Controller", ProductCondition.New, 16.00m, 29.99m, 14, 4, "ACC-SW-01"),
            new("Accessories", "accessories", "PS5 DualSense Controller", "ps5-dualsense-controller", "TGP-MOCK-ACC-002", "000300000002", "PlayStation 5", "Controller", ProductCondition.VeryGood, 35.00m, 54.99m, 4, 2, "ACC-PS-01"),
            new("Accessories", "accessories", "Retro HDMI Adapter", "retro-hdmi-adapter", "TGP-MOCK-ACC-003", "000300000003", "Retro", "Cable", ProductCondition.New, 8.00m, 18.99m, 18, 5, "ACC-RT-01"),
            new("Trading Cards", "trading-cards", "Pokemon Booster Pack", "pokemon-booster-pack", "TGP-MOCK-TCG-001", "000400000001", "Pokemon TCG", "Pokemon", ProductCondition.New, 3.00m, 5.49m, 40, 10, "TCG-PK-01"),
            new("Trading Cards", "trading-cards", "Magic Commander Deck", "magic-commander-deck", "TGP-MOCK-TCG-002", "000400000002", "Magic", "MTG", ProductCondition.New, 24.00m, 44.99m, 7, 2, "TCG-MT-01"),
            new("Collectibles", "collectibles", "Anime Figure Mystery Box", "anime-figure-mystery-box", "TGP-MOCK-COL-001", "000500000001", "Collectible", "Anime", ProductCondition.New, 14.00m, 27.99m, 9, 3, "COL-AN-01"),
            new("Collectibles", "collectibles", "Retro Game Enamel Pin", "retro-game-enamel-pin", "TGP-MOCK-COL-002", "000500000002", "Collectible", "Retro", ProductCondition.New, 2.50m, 7.99m, 25, 6, "COL-RT-01")
        };
    }

    private sealed record MockProductSeed(
        string CategoryName,
        string CategorySlug,
        string Name,
        string Slug,
        string Sku,
        string Barcode,
        string Platform,
        string Franchise,
        ProductCondition Condition,
        decimal CostPrice,
        decimal SalePrice,
        int QuantityOnHand,
        int LowStockThreshold,
        string LocationCode)
    {
        public ProductStatus Status => ProductStatus.Active;

        public string Description =>
            $"Mock {CategoryName.ToLowerInvariant()} item for storefront, cart, checkout, inventory, and admin order testing.";
    }
}
