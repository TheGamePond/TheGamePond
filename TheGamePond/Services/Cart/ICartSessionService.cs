using TheGamePond.Models.Cart;

namespace TheGamePond.Services.Cart;

public interface ICartSessionService
{
    List<CartSessionItem> GetItems();

    int GetItemCount();

    void SaveItems(List<CartSessionItem> items);
}
