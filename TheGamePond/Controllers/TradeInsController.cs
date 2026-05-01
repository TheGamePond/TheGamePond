using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.TradeIns;

namespace TheGamePond.Controllers;

[Route("Trade-In")]
public class TradeInsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TradeInsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Create()
    {
        return View(new TradeInRequestFormViewModel());
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TradeInRequestFormViewModel model)
    {
        var submittedItems = model.Items
            .Where(item => !string.IsNullOrWhiteSpace(item.ItemName))
            .ToList();

        if (submittedItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Add at least one item for trade-in review.");
        }

        for (var index = 0; index < model.Items.Count; index++)
        {
            if (string.IsNullOrWhiteSpace(model.Items[index].ItemName))
            {
                ModelState.Remove($"Items[{index}].ItemName");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var request = new TradeInRequest
        {
            RequestNumber = await CreateRequestNumberAsync(),
            CustomerName = model.CustomerName.Trim(),
            CustomerEmail = model.CustomerEmail.Trim(),
            CustomerPhone = NormalizeOptional(model.CustomerPhone),
            PreferredContactMethod = model.PreferredContactMethod.Trim(),
            CustomerNotes = NormalizeOptional(model.CustomerNotes)
        };

        foreach (var item in submittedItems)
        {
            request.Items.Add(new TradeInRequestItem
            {
                ItemName = item.ItemName.Trim(),
                Platform = NormalizeOptional(item.Platform),
                Condition = NormalizeOptional(item.Condition),
                Quantity = item.Quantity,
                Notes = NormalizeOptional(item.Notes)
            });
        }

        _context.TradeInRequests.Add(request);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Confirmation), new { requestNumber = request.RequestNumber });
    }

    [HttpGet("Confirmation/{requestNumber}")]
    public async Task<IActionResult> Confirmation(string requestNumber)
    {
        var request = await _context.TradeInRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.RequestNumber == requestNumber);

        if (request is null)
        {
            return NotFound();
        }

        return View(new TradeInConfirmationViewModel
        {
            RequestNumber = request.RequestNumber,
            Status = request.Status
        });
    }

    private async Task<string> CreateRequestNumberAsync()
    {
        var datePrefix = DateTimeOffset.UtcNow.ToString("yyyyMMdd");

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var candidate = $"TGI-{datePrefix}-{Random.Shared.Next(1000, 9999)}";
            var exists = await _context.TradeInRequests.AnyAsync(request => request.RequestNumber == candidate);

            if (!exists)
            {
                return candidate;
            }
        }

        return $"TGI-{datePrefix}-{Guid.NewGuid():N}"[..22];
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
