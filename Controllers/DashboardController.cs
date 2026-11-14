using DMDFoodBank.Web.Infrastructure;
using DMDFoodBank.Web.Models;
using DMDFoodBank.Web.Services;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

[Authorize]
public class DashboardController(DonationService donations, AccountService accounts) : Controller
{
    public async Task<IActionResult> Index()
    {
        var user = await accounts.GetUserAsync(User.GetUserId()!.Value);
        if (user is null) return RedirectToAction("Login", "Account");

        var month = await donations.MonthTotalAsync();
        var next = Milestones.Next(user.TotalDonated);
        return View(new DashboardViewModel
        {
            User = user,
            MonthTotal = month,
            MonthPercent = DonationService.Percent(month),
            NextMilestone = next,
            MilestonePercent = next is null ? 100 : (int)Math.Clamp(user.TotalDonated / next.Threshold * 100, 0, 100),
            Recent = await donations.ForUserAsync(user.Id, 5)
        });
    }
}
