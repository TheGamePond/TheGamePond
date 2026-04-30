namespace TheGamePond.Models.Cart;

public class CartViewModel
{
    public IReadOnlyList<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

    public decimal Subtotal => Items.Sum(item => item.LineTotal);

    public bool IsEmpty => Items.Count == 0;
}
