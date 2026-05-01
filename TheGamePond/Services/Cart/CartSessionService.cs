using System.Text.Json;
using TheGamePond.Models.Cart;

namespace TheGamePond.Services.Cart;

public class CartSessionService : ICartSessionService
{
    private const string CartSessionKey = "TheGamePond.Cart";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartSessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<CartSessionItem> GetItems()
    {
        var json = _httpContextAccessor.HttpContext?.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<CartSessionItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CartSessionItem>>(json) ?? new List<CartSessionItem>();
        }
        catch (JsonException)
        {
            return new List<CartSessionItem>();
        }
    }

    public int GetItemCount()
    {
        return GetItems().Sum(item => item.Quantity);
    }

    public void SaveItems(List<CartSessionItem> items)
    {
        var session = _httpContextAccessor.HttpContext?.Session;

        if (session is null)
        {
            return;
        }

        if (items.Count == 0)
        {
            session.Remove(CartSessionKey);
            return;
        }

        session.SetString(CartSessionKey, JsonSerializer.Serialize(items));
    }
}
