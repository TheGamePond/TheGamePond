using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Catalog;

public class ProductImage
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    [StringLength(300)]
    public string FilePath { get; set; } = string.Empty;

    [StringLength(180)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }

    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}
