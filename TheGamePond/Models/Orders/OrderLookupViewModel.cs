using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Orders;

public class OrderLookupViewModel
{
    [Required]
    [StringLength(32)]
    [Display(Name = "Order number")]
    public string OrderNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(180)]
    [Display(Name = "Email address")]
    public string CustomerEmail { get; set; } = string.Empty;
}
