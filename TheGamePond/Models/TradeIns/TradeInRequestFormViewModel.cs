using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.TradeIns;

public class TradeInRequestFormViewModel
{
    [Required]
    [Display(Name = "Full name")]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    [StringLength(180)]
    public string CustomerEmail { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone")]
    [StringLength(40)]
    public string? CustomerPhone { get; set; }

    [Required]
    [Display(Name = "Preferred contact")]
    [StringLength(80)]
    public string PreferredContactMethod { get; set; } = "Email";

    [Display(Name = "Anything else we should know?")]
    [StringLength(1000)]
    public string? CustomerNotes { get; set; }

    public List<TradeInItemFormViewModel> Items { get; set; } =
    [
        new TradeInItemFormViewModel(),
        new TradeInItemFormViewModel(),
        new TradeInItemFormViewModel()
    ];
}
