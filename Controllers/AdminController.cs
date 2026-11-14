using DMDFoodBank.Web.Infrastructure;
using DMDFoodBank.Web.Models;
using DMDFoodBank.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DMDFoodBank.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController(AdminService admin) : Controller
{
    // Badge counts shown in the admin menu on every admin page
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ViewBag.Unread = await admin.UnreadMessagesAsync();
        ViewBag.PendingBeneficiaries = await admin.PendingBeneficiariesAsync();
        ViewBag.PendingVolunteers = await admin.PendingVolunteersAsync();
        await base.OnActionExecutionAsync(context, next);
    }

    public async Task<IActionResult> Index() => View(await admin.DashboardAsync());

    public async Task<IActionResult> Donors(string? q)
    {
        ViewBag.Query = q;
        return View(await admin.DonorsAsync(q));
    }

    public async Task<IActionResult> Donations() => View(await admin.DonationsAsync());
    public async Task<IActionResult> Beneficiaries() => View(await admin.BeneficiariesAsync());
    public async Task<IActionResult> Volunteers() => View(await admin.VolunteersAsync());
    public async Task<IActionResult> Messages() => View(await admin.MessagesAsync());
    public async Task<IActionResult> Users() => View(await admin.UsersAsync());

    public async Task<IActionResult> Message(int id)
    {
        var m = await admin.ReadMessageAsync(id);
        return m is null ? NotFound() : View(m);
    }

    // ── Actions (all POST + anti-forgery) ────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id, string? returnTo)
    {
        var error = await admin.ToggleActiveAsync(id, User.GetUserId());
        TempData[error is null ? "Success" : "Error"] = error ?? "User status updated.";
        return RedirectToAction(returnTo == "donors" ? nameof(Donors) : nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> SetRole(int id, string role)
    {
        var error = await admin.SetRoleAsync(id, role, User.GetUserId());
        TempData[error is null ? "Success" : "Error"] = error ?? $"Role updated to {role}.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    public async Task<IActionResult> BeneficiaryStatus(int id, string status)
    {
        await admin.SetBeneficiaryStatusAsync(id, status);
        return RedirectToAction(nameof(Beneficiaries));
    }

    [HttpPost]
    public async Task<IActionResult> VolunteerStatus(int id, string status)
    {
        await admin.SetVolunteerStatusAsync(id, status);
        return RedirectToAction(nameof(Volunteers));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        await admin.DeleteMessageAsync(id);
        TempData["Success"] = "Message deleted.";
        return RedirectToAction(nameof(Messages));
    }
}
