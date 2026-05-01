using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.Cart;

namespace TheGamePond.Models.Checkout;

public class CheckoutViewModel
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
    [Display(Name = "Address line 1")]
    [StringLength(180)]
    public string ShippingAddressLine1 { get; set; } = string.Empty;

    [Display(Name = "Address line 2")]
    [StringLength(180)]
    public string? ShippingAddressLine2 { get; set; }

    [Required]
    [Display(Name = "City")]
    [StringLength(90)]
    public string ShippingCity { get; set; } = string.Empty;

    [Required]
    [Display(Name = "State")]
    [StringLength(60)]
    public string ShippingState { get; set; } = string.Empty;

    [Required]
    [Display(Name = "ZIP code")]
    [StringLength(20)]
    public string ShippingPostalCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Country")]
    [StringLength(80)]
    public string ShippingCountry { get; set; } = "United States";

    [Display(Name = "Order notes")]
    [StringLength(1000)]
    public string? CustomerNotes { get; set; }

    public CartViewModel Cart { get; set; } = new();
}
