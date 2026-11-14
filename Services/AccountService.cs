using DMDFoodBank.Web.Data;
using DMDFoodBank.Web.Models;
using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DMDFoodBank.Web.Services;

/// <summary>Model layer: accounts, sign-in, registration and profile rules.</summary>
public class AccountService(AppDbContext db, IPasswordHasher<User> hasher)
{
    private static string Normalize(string email) => email.Trim().ToLowerInvariant();

    public Task<User?> GetUserAsync(int id) => db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<(User? User, string? Error)> LoginAsync(string email, string password)
    {
        var e = Normalize(email);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == e && u.IsActive);
        // One generic message, so the form never reveals which emails exist
        const string bad = "Incorrect email or password. Please try again.";
        if (user is null) return (null, bad);

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return (null, bad);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = hasher.HashPassword(user, password);

        user.LastLogin = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return (user, null);
    }

    public async Task<string?> RegisterDonorAsync(DonorRegisterViewModel m)
    {
        var e = Normalize(m.Email);
        if (await db.Users.AnyAsync(u => u.Email == e))
            return "An account with this email already exists.";

        var user = new User { FullName = m.FullName.Trim(), Email = e, DonorType = m.DonorType };
        user.PasswordHash = hasher.HashPassword(user, m.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return null;
    }

    public async Task RegisterVolunteerAsync(VolunteerRegisterViewModel m)
    {
        db.Volunteers.Add(new Volunteer
        {
            FullName = m.FullName.Trim(), Email = Normalize(m.Email),
            Skills = m.Skills, Availability = m.Availability
        });
        await db.SaveChangesAsync();
    }

    public async Task RegisterBeneficiaryAsync(BeneficiaryRegisterViewModel m)
    {
        db.Beneficiaries.Add(new Beneficiary
        {
            FullName = m.FullName.Trim(), Phone = m.Phone.Trim(),
            HouseholdSize = m.HouseholdSize, NearestHub = m.NearestHub
        });
        await db.SaveChangesAsync();
    }

    public async Task UpdateProfileAsync(int userId, ProfileSettingsViewModel m)
    {
        var user = await db.Users.FindAsync(userId);
        if (user is null) return;
        user.FullName = m.FullName.Trim();
        user.Phone = m.Phone?.Trim() ?? "";
        user.DonorType = m.DonorType;
        await db.SaveChangesAsync();
    }

    public async Task<string?> ChangePasswordAsync(int userId, ChangePasswordViewModel m)
    {
        var user = await db.Users.FindAsync(userId);
        if (user is null) return "User not found.";
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, m.CurrentPassword) == PasswordVerificationResult.Failed)
            return "Current password is incorrect.";
        user.PasswordHash = hasher.HashPassword(user, m.NewPassword);
        await db.SaveChangesAsync();
        return null;
    }

    public async Task SendMessageAsync(ContactViewModel m)
    {
        db.ContactMessages.Add(new ContactMessage
        {
            Name = m.Name.Trim(), Email = m.Email.Trim(),
            Subject = m.Subject.Trim(), Message = m.Message.Trim()
        });
        await db.SaveChangesAsync();
    }
}
