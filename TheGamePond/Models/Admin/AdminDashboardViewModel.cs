using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Admin;

public class AdminDashboardViewModel
{
    public int ProductCount { get; set; }

    public int ActiveProductCount { get; set; }

    public int LowStockCount { get; set; }

    public int DraftProductCount { get; set; }

    public int OpenTradeInCount { get; set; }

    public IReadOnlyList<Product> LowStockProducts { get; set; } = new List<Product>();
}
