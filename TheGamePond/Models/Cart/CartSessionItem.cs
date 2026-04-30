namespace TheGamePond.Models.Cart;

public class CartSessionItem
{
    public string ProductSlug { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
