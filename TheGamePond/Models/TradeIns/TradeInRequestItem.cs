using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.TradeIns;

public class TradeInRequestItem
{
    public int Id { get; set; }

    public int TradeInRequestId { get; set; }

    public TradeInRequest? TradeInRequest { get; set; }

    [Required]
    [StringLength(160)]
    public string ItemName { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Platform { get; set; }

    [StringLength(80)]
    public string? Condition { get; set; }

    public int Quantity { get; set; } = 1;

    [StringLength(500)]
    public string? Notes { get; set; }
}
