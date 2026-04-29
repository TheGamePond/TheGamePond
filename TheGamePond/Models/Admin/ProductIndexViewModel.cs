using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Admin;

public class ProductIndexViewModel
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public ProductStatus? Status { get; set; }

    public IReadOnlyList<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

    public IReadOnlyList<Product> Products { get; set; } = new List<Product>();
}
