using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Catalog;

public class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    [StringLength(260)]
    public string ImagePath { get; set; } = string.Empty;

    [StringLength(160)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
