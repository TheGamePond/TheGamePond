using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.Orders;

namespace TheGamePond.Models.Admin;

public class OrderStatusUpdateViewModel
{
    public string OrderNumber { get; set; } = string.Empty;

    [Display(Name = "Next status")]
    public OrderStatus NextStatus { get; set; }

    [Display(Name = "Tracking number")]
    [StringLength(120)]
    public string? TrackingNumber { get; set; }

    [Display(Name = "Staff notes")]
    [StringLength(1000)]
    public string? StaffNotes { get; set; }
}
