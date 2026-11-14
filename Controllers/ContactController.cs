using DMDFoodBank.Web.Services;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

public class ContactController(AccountService accounts) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new ContactViewModel());

    [HttpPost]
    public async Task<IActionResult> Index(ContactViewModel m)
    {
        if (!ModelState.IsValid) return View(m);
        await accounts.SendMessageAsync(m);
        TempData["Success"] = "Message sent! We'll reply within 2 business days.";
        return RedirectToAction(nameof(Index));
    }
}
