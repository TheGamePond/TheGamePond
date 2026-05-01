using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Admin;
using TheGamePond.Models.TradeIns;

namespace TheGamePond.Controllers;

[Authorize(Roles = $"{AppRoles.Owner},{AppRoles.Admin},{AppRoles.Staff}")]
[Route("Admin/Trade-Ins")]
public class AdminTradeInsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminTradeInsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(TradeInRequestStatus? status = null)
    {
        var requestsQuery = _context.TradeInRequests
            .AsNoTracking()
            .Include(request => request.Items)
            .AsQueryable();

        if (status.HasValue)
        {
            requestsQuery = requestsQuery.Where(request => request.Status == status.Value);
        }

        ViewData["Status"] = status;

        var requests = await requestsQuery
            .OrderByDescending(request => request.CreatedAt)
            .Take(100)
            .ToListAsync();

        return View(requests);
    }

    [HttpGet("{requestNumber}")]
    public async Task<IActionResult> Details(string requestNumber)
    {
        var request = await _context.TradeInRequests
            .AsNoTracking()
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.RequestNumber == requestNumber);

        if (request is null)
        {
            return NotFound();
        }

        return View(request);
    }

    [HttpPost("{requestNumber}/Status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string requestNumber, TradeInStatusUpdateViewModel model)
    {
        if (!string.Equals(requestNumber, model.RequestNumber, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        var request = await _context.TradeInRequests
            .FirstOrDefaultAsync(item => item.RequestNumber == requestNumber);

        if (request is null)
        {
            return NotFound();
        }

        if (model.EstimatedOfferLow.HasValue &&
            model.EstimatedOfferHigh.HasValue &&
            model.EstimatedOfferLow > model.EstimatedOfferHigh)
        {
            TempData["StatusMessage"] = "Offer low cannot be greater than offer high.";
            return RedirectToAction(nameof(Details), new { requestNumber });
        }

        request.Status = model.Status;
        request.EstimatedOfferLow = model.EstimatedOfferLow;
        request.EstimatedOfferHigh = model.EstimatedOfferHigh;
        request.StaffNotes = NormalizeOptional(model.StaffNotes);
        request.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = $"Trade-in request moved to {model.Status}.";

        return RedirectToAction(nameof(Details), new { requestNumber });
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
