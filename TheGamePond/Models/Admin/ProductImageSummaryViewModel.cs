namespace TheGamePond.Models.Admin;

public class ProductImageSummaryViewModel
{
    public string ImagePath { get; set; } = string.Empty;

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }
}
