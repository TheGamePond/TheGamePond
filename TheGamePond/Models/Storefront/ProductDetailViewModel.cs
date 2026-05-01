namespace TheGamePond.Models.Storefront;

public class ProductDetailViewModel
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Platform { get; set; }

    public string? Franchise { get; set; }

    public string? CategoryName { get; set; }

    public string Condition { get; set; } = string.Empty;

    public decimal SalePrice { get; set; }

    public int QuantityOnHand { get; set; }

    public IReadOnlyList<ShopProductImageViewModel> Images { get; set; } = new List<ShopProductImageViewModel>();

    public string? ShopSearch { get; set; }

    public string? ShopCategory { get; set; }

    public string ShopSort { get; set; } = "newest";

    public bool IsInStock => QuantityOnHand > 0;
}
