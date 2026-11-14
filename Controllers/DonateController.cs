using DMDFoodBank.Web.Infrastructure;
using DMDFoodBank.Web.Services;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

public class DonateController(DonationService donations, AccountService accounts) : Controller
{
    private async Task<DonatePageViewModel> BuildAsync(string tab = "money",
        MoneyDonationViewModel? money = null, FoodDonationViewModel? food = null)
    {
        var page = new DonatePageViewModel
        {
            ActiveTab = tab,
            Money = money ?? new(),
            Food = food ?? new(),
            IsLoggedIn = User.Identity?.IsAuthenticated == true
        };
        var id = User.GetUserId();
        if (id is not null && await accounts.GetUserAsync(id.Value) is { } u)
        {
            page.UserTotal = u.TotalDonated;
            page.UserCount = u.DonationCount;
        }
        return page;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tab = "money") => View(await BuildAsync(tab));

    [HttpPost]
    public async Task<IActionResult> GiveMoney([Bind(Prefix = "Money")] MoneyDonationViewModel m)
    {
        if (ModelState.IsValid)
        {
            var error = await donations.DonateMoneyAsync(User.GetUserId(), m.Amount, m.Frequency);
            if (error is null)
            {
                TempData["Success"] = $"Thank you for donating {Money.Format(m.Amount)}! Your generosity makes a real difference.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("Money.CustomAmount", error);
        }
        return View("Index", await BuildAsync("money", money: m));
    }

    [HttpPost]
    public async Task<IActionResult> GiveFood([Bind(Prefix = "Food")] FoodDonationViewModel m)
    {
        if (ModelState.IsValid)
        {
            var error = await donations.DonateFoodAsync(User.GetUserId(), m.Description, m.Hub);
            if (error is null)
            {
                TempData["Success"] = $"Food donation registered! Please drop off at {m.Hub}.";
                return RedirectToAction(nameof(Index), new { tab = "food" });
            }
            ModelState.AddModelError("Food.Hub", error);
        }
        return View("Index", await BuildAsync("food", food: m));
    }
}
