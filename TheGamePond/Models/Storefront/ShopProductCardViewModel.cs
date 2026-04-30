using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Storefront;

public class ShopProductCardViewModel
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string? Platform { get; set; }

    public string? Franchise { get; set; }

    public string? CategoryName { get; set; }

    public ProductCondition Condition { get; set; }

    public decimal SalePrice { get; set; }

    public int QuantityOnHand { get; set; }

    public string? PrimaryImagePath { get; set; }

    public string? PrimaryImageAltText { get; set; }

    public bool IsInStock => QuantityOnHand > 0;
}
