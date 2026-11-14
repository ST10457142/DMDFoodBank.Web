using System.Globalization;
using System.Security.Claims;

namespace DMDFoodBank.Web.Infrastructure;

public static class Money
{
    public static string Format(double amount) =>
        "R " + amount.ToString("N2", CultureInfo.InvariantCulture);
}

public static class ClaimsExtensions
{
    public static int? GetUserId(this ClaimsPrincipal p) =>
        int.TryParse(p.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
