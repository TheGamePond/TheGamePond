using Microsoft.AspNetCore.Mvc;
using TheGamePond.Services.Cart;

namespace TheGamePond.ViewComponents;

public class CartNavViewComponent : ViewComponent
{
    private readonly ICartSessionService _cartSession;

    public CartNavViewComponent(ICartSessionService cartSession)
    {
        _cartSession = cartSession;
    }

    public IViewComponentResult Invoke()
    {
        return View(_cartSession.GetItemCount());
    }
}
