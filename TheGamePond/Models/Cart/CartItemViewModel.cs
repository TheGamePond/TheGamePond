namespace TheGamePond.Models.Cart;

public class CartItemViewModel
{
    public string ProductSlug { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public string? Platform { get; set; }

    public string? ImagePath { get; set; }

    public string? ImageAltText { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public int QuantityOnHand { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}
