using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Storefront;

public class ShopIndexViewModel
{
    public string? Search { get; set; }

    public string? Category { get; set; }

    public IReadOnlyList<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

    public IReadOnlyList<ShopProductCardViewModel> Products { get; set; } = new List<ShopProductCardViewModel>();
}
