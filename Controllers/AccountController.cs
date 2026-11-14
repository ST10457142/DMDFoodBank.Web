using System.Security.Claims;
using DMDFoodBank.Web.Models;
using DMDFoodBank.Web.Services;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

public class AccountController(AccountService accounts) : Controller
{
    // ── Sign in ──────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home")
                                               : View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel m)
    {
        if (!ModelState.IsValid) return View(m);

        var (user, error) = await accounts.LoginAsync(m.Email, m.Password);
        if (user is null)
        {
            ModelState.AddModelError("", error!);
            return View(m);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = m.RememberMe });

        TempData["Success"] = $"Welcome back, {user.FullName}!";
        if (user.Role == Roles.Admin) return RedirectToAction("Index", "Admin");
        if (!string.IsNullOrEmpty(m.ReturnUrl) && Url.IsLocalUrl(m.ReturnUrl)) return LocalRedirect(m.ReturnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() => View();

    // ── Register (donor / volunteer / beneficiary) ───────────────────────────

    [HttpGet]
    public IActionResult Register(string tab = "donor") => View(new RegisterPageViewModel { ActiveTab = tab });

    [HttpPost]
    public async Task<IActionResult> RegisterDonor([Bind(Prefix = "Donor")] DonorRegisterViewModel m)
    {
        if (ModelState.IsValid)
        {
            var error = await accounts.RegisterDonorAsync(m);
            if (error is null)
            {
                TempData["Success"] = "Account created! Please sign in.";
                return RedirectToAction(nameof(Login));
            }
            ModelState.AddModelError("Donor.Email", error);
        }
        return View("Register", new RegisterPageViewModel { ActiveTab = "donor", Donor = m });
    }

    [HttpPost]
    public async Task<IActionResult> RegisterVolunteer([Bind(Prefix = "Volunteer")] VolunteerRegisterViewModel m)
    {
        if (!ModelState.IsValid)
            return View("Register", new RegisterPageViewModel { ActiveTab = "volunteer", Volunteer = m });

        await accounts.RegisterVolunteerAsync(m);
        TempData["Success"] = "Volunteer application received! We will be in touch.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> RegisterBeneficiary([Bind(Prefix = "Beneficiary")] BeneficiaryRegisterViewModel m)
    {
        if (!ModelState.IsValid)
            return View("Register", new RegisterPageViewModel { ActiveTab = "beneficiary", Beneficiary = m });

        await accounts.RegisterBeneficiaryAsync(m);
        TempData["Success"] = "Application received! We'll contact you within 48 hours.";
        return RedirectToAction("Index", "Home");
    }
}
