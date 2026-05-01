using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Orders;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    [StringLength(160)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(180)]
    public string ProductSlug { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
