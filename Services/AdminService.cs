using DMDFoodBank.Web.Data;
using DMDFoodBank.Web.Models;
using DMDFoodBank.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DMDFoodBank.Web.Services;

/// <summary>Model layer: everything the administrator can see or change.</summary>
public class AdminService(AppDbContext db)
{
    public async Task<AdminDashboardViewModel> DashboardAsync() => new()
    {
        TotalRaised = await db.Donations.Where(d => d.DonationType == "money").SumAsync(d => d.Amount),
        MonthRaised = await new DonationService(db).MonthTotalAsync(),
        TotalDonations = await db.Donations.CountAsync(),
        TotalBeneficiaries = await db.Beneficiaries.CountAsync(),
        TopDonors = await db.Users.Where(u => u.Role != Roles.Admin && u.TotalDonated > 0)
            .OrderByDescending(u => u.TotalDonated).Take(5).ToListAsync(),
        RecentDonations = await db.Donations.Include(d => d.User)
            .OrderByDescending(d => d.CreatedAt).Take(8).ToListAsync()
    };

    public Task<int> UnreadMessagesAsync() => db.ContactMessages.CountAsync(m => !m.IsRead);
    public Task<int> PendingBeneficiariesAsync() => db.Beneficiaries.CountAsync(b => b.Status == "pending");
    public Task<int> PendingVolunteersAsync() => db.Volunteers.CountAsync(v => v.Status == "pending");

    public Task<List<User>> DonorsAsync(string? q)
    {
        var query = db.Users.Where(u => u.Role != Roles.Admin);
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(u => u.FullName.Contains(q) || u.Email.Contains(q));
        return query.OrderByDescending(u => u.TotalDonated).ToListAsync();
    }

    public Task<List<User>> UsersAsync() => db.Users.OrderByDescending(u => u.TotalDonated).ToListAsync();
    public Task<List<Donation>> DonationsAsync() =>
        db.Donations.Include(d => d.User).OrderByDescending(d => d.CreatedAt).ToListAsync();
    public Task<List<Beneficiary>> BeneficiariesAsync() =>
        db.Beneficiaries.OrderByDescending(b => b.CreatedAt).ToListAsync();
    public Task<List<Volunteer>> VolunteersAsync() =>
        db.Volunteers.OrderByDescending(v => v.CreatedAt).ToListAsync();
    public Task<List<ContactMessage>> MessagesAsync() =>
        db.ContactMessages.OrderByDescending(m => m.CreatedAt).ToListAsync();

    public async Task<string?> ToggleActiveAsync(int id, int? currentAdminId)
    {
        if (id == currentAdminId) return "You cannot deactivate your own account.";
        var user = await db.Users.FindAsync(id);
        if (user is null) return "User not found.";
        user.IsActive = !user.IsActive;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> SetRoleAsync(int id, string role, int? currentAdminId)
    {
        if (role != Roles.Admin && role != Roles.Donor) return "Unknown role.";
        if (id == currentAdminId) return "You cannot change your own role.";
        var user = await db.Users.FindAsync(id);
        if (user is null) return "User not found.";
        user.Role = role;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task SetBeneficiaryStatusAsync(int id, string status)
    {
        if (status is not ("approved" or "rejected")) return;
        var b = await db.Beneficiaries.FindAsync(id);
        if (b is null) return;
        b.Status = status;
        await db.SaveChangesAsync();
    }

    public async Task SetVolunteerStatusAsync(int id, string status)
    {
        if (status is not ("approved" or "rejected")) return;
        var v = await db.Volunteers.FindAsync(id);
        if (v is null) return;
        v.Status = status;
        await db.SaveChangesAsync();
    }

    public async Task<ContactMessage?> ReadMessageAsync(int id)
    {
        var m = await db.ContactMessages.FindAsync(id);
        if (m is not null && !m.IsRead) { m.IsRead = true; await db.SaveChangesAsync(); }
        return m;
    }

    public async Task DeleteMessageAsync(int id)
    {
        var m = await db.ContactMessages.FindAsync(id);
        if (m is null) return;
        db.ContactMessages.Remove(m);
        await db.SaveChangesAsync();
    }
}
