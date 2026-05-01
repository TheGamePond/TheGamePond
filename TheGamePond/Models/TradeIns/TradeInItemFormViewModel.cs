using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.TradeIns;

public class TradeInItemFormViewModel
{
    [Required]
    [StringLength(160)]
    [Display(Name = "Item name")]
    public string ItemName { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Platform { get; set; }

    [StringLength(80)]
    public string? Condition { get; set; }

    [Range(1, 99)]
    public int Quantity { get; set; } = 1;

    [StringLength(500)]
    public string? Notes { get; set; }
}
