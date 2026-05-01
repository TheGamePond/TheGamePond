namespace TheGamePond.Models.TradeIns;

public class TradeInConfirmationViewModel
{
    public string RequestNumber { get; set; } = string.Empty;

    public TradeInRequestStatus Status { get; set; }
}
