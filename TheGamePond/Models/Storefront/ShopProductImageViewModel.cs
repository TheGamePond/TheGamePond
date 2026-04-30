namespace TheGamePond.Models.Storefront;

public class ShopProductImageViewModel
{
    public string ImagePath { get; set; } = string.Empty;

    public string? AltText { get; set; }

    public bool IsPrimary { get; set; }
}
