using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.TradeIns;

public class TradeInRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(32)]
    public string RequestNumber { get; set; } = string.Empty;

    public TradeInRequestStatus Status { get; set; } = TradeInRequestStatus.Submitted;

    [Required]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(180)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Phone]
    [StringLength(40)]
    public string? CustomerPhone { get; set; }

    [StringLength(80)]
    public string PreferredContactMethod { get; set; } = "Email";

    [StringLength(1000)]
    public string? CustomerNotes { get; set; }

    [StringLength(1000)]
    public string? StaffNotes { get; set; }

    public decimal? EstimatedOfferLow { get; set; }

    public decimal? EstimatedOfferHigh { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<TradeInRequestItem> Items { get; set; } = new List<TradeInRequestItem>();
}
