using DMDFoodBank.Web.Infrastructure;
using DMDFoodBank.Web.Models;
using DMDFoodBank.Web.Services;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

[Authorize]
public class ProfileController(AccountService accounts, DonationService donations) : Controller
{
    private async Task<ProfilePageViewModel?> BuildAsync(string tab)
    {
        var user = await accounts.GetUserAsync(User.GetUserId()!.Value);
        if (user is null) return null;
        return new ProfilePageViewModel
        {
            Tab = tab,
            User = user,
            Donations = await donations.ForUserAsync(user.Id),
            Milestones = Milestones.All
                .Select(m => new MilestoneStatus(m, m.IsReached(user.TotalDonated, user.DonationCount))).ToList(),
            Settings = new ProfileSettingsViewModel { FullName = user.FullName, Phone = user.Phone, DonorType = user.DonorType }
        };
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tab = "overview")
    {
        var page = await BuildAsync(tab);
        return page is null ? RedirectToAction("Login", "Account") : View(page);
    }

    [HttpPost]
    public async Task<IActionResult> Settings([Bind(Prefix = "Settings")] ProfileSettingsViewModel m)
    {
        if (!ModelState.IsValid)
        {
            var page = (await BuildAsync("settings"))!;
            page.Settings = m;
            return View("Index", page);
        }
        await accounts.UpdateProfileAsync(User.GetUserId()!.Value, m);
        TempData["Success"] = "Profile updated successfully!";
        return RedirectToAction(nameof(Index), new { tab = "settings" });
    }

    [HttpPost]
    public async Task<IActionResult> Password([Bind(Prefix = "Password")] ChangePasswordViewModel m)
    {
        if (ModelState.IsValid)
        {
            var error = await accounts.ChangePasswordAsync(User.GetUserId()!.Value, m);
            if (error is null)
            {
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index), new { tab = "security" });
            }
            ModelState.AddModelError("Password.CurrentPassword", error);
        }
        var page = (await BuildAsync("security"))!;
        page.Password = new ChangePasswordViewModel();
        return View("Index", page);
    }
}
