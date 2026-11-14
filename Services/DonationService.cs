using DMDFoodBank.Web.Data;
using DMDFoodBank.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DMDFoodBank.Web.Services;

/// <summary>Model layer: donations and the figures shown on Home and Dashboard.</summary>
public class DonationService(AppDbContext db)
{
    public const double MonthlyGoal = 50_000;
    public const double MinimumDonation = 10;

    public async Task<string?> DonateMoneyAsync(int? userId, double amount, string frequency)
    {
        if (amount < MinimumDonation) return "Minimum donation is R 10.00.";
        if (frequency != "once" && frequency != "monthly") frequency = "once";

        db.Donations.Add(new Donation { UserId = userId, Amount = amount, Frequency = frequency });
        if (userId is not null)
        {
            var user = await db.Users.FindAsync(userId.Value);
            if (user is not null) { user.TotalDonated += amount; user.DonationCount++; }
        }
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DonateFoodAsync(int? userId, string description, string hub)
    {
        if (string.IsNullOrWhiteSpace(description)) return "Please describe the food items.";
        if (string.IsNullOrWhiteSpace(hub) || HubData.All.All(h => h.Name != hub))
            return "Please select a drop-off hub.";

        db.Donations.Add(new Donation
        {
            UserId = userId, Amount = 0, DonationType = "food",
            FoodDescription = $"{description.Trim()} | Hub: {hub}"
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<double> MonthTotalAsync()
    {
        var start = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return await db.Donations
            .Where(d => d.DonationType == "money" && d.CreatedAt >= start)
            .SumAsync(d => d.Amount);
    }

    public static int Percent(double total) =>
        (int)Math.Clamp(total / MonthlyGoal * 100, 0, 100);

    public Task<List<Donation>> ForUserAsync(int userId, int? take = null)
    {
        var q = db.Donations.Where(d => d.UserId == userId).OrderByDescending(d => d.CreatedAt).AsQueryable();
        if (take is not null) q = q.Take(take.Value);
        return q.ToListAsync();
    }
}
