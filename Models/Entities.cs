using System.ComponentModel.DataAnnotations.Schema;
using DMDFoodBank.Web.Infrastructure;

namespace DMDFoodBank.Web.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = Roles.Donor;
    public string DonorType { get; set; } = "Individual";
    public string Phone { get; set; } = "";
    public double TotalDonated { get; set; }
    public int DonationCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Donation> Donations { get; set; } = new();

    [NotMapped]
    public string Initials =>
        string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2).Select(p => char.ToUpperInvariant(p[0])));

    [NotMapped] public string FormattedTotal => Money.Format(TotalDonated);
    [NotMapped] public int MealsFunded => (int)(TotalDonated / 10);
}

public class Donation
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public double Amount { get; set; }
    public string DonationType { get; set; } = "money";   // money | food
    public string? FoodDescription { get; set; }
    public string Frequency { get; set; } = "once";        // once | monthly
    public string Status { get; set; } = "completed";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped] public string FormattedAmount => DonationType == "money" ? Money.Format(Amount) : "In-kind";
    [NotMapped] public string FormattedDate => CreatedAt.ToLocalTime().ToString("dd MMM yyyy");
}

public class ContactMessage
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}

public class Beneficiary
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string HouseholdSize { get; set; } = "";
    public string NearestHub { get; set; } = "";
    public string Status { get; set; } = "pending";   // pending | approved | rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Volunteer
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Skills { get; set; } = "";
    public string Availability { get; set; } = "";
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class Roles
{
    public const string Donor = "donor";
    public const string Admin = "admin";
}
