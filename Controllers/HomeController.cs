using DMDFoodBank.Web.Infrastructure;
using DMDFoodBank.Web.Services;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

public class HomeController(DonationService donations, AccountService accounts) : Controller
{
    public async Task<IActionResult> Index()
    {
        var month = await donations.MonthTotalAsync();
        var userId = User.GetUserId();
        return View(new HomeViewModel
        {
            MonthTotal = month,
            MonthPercent = DonationService.Percent(month),
            User = userId is null ? null : await accounts.GetUserAsync(userId.Value)
        });
    }

    public IActionResult About() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
