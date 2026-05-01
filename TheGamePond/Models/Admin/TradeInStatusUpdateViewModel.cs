using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.TradeIns;

namespace TheGamePond.Models.Admin;

public class TradeInStatusUpdateViewModel
{
    [Required]
    public string RequestNumber { get; set; } = string.Empty;

    public TradeInRequestStatus Status { get; set; }

    [Range(0, 100000)]
    [Display(Name = "Offer low")]
    public decimal? EstimatedOfferLow { get; set; }

    [Range(0, 100000)]
    [Display(Name = "Offer high")]
    public decimal? EstimatedOfferHigh { get; set; }

    [StringLength(1000)]
    [Display(Name = "Staff notes")]
    public string? StaffNotes { get; set; }
}
