using System.ComponentModel.DataAnnotations;
using DMDFoodBank.Web.Models;

namespace DMDFoodBank.Web.ViewModels;

public static class Rules
{
    public const string PasswordPattern = @"^(?=.*[A-Z])(?=.*\d).{8,}$";
    public const string PasswordMessage = "Password must be at least 8 characters and contain an uppercase letter and a number.";
}

// ── Account ──────────────────────────────────────────────────────────────────

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required."), EmailAddress, Display(Name = "Email address")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required."), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Keep me signed in")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public class DonorRegisterViewModel
{
    [Required(ErrorMessage = "Full name is required."), StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Email is required."), EmailAddress(ErrorMessage = "Valid email address required."), Display(Name = "Email address")]
    public string Email { get; set; } = "";

    [Required, Display(Name = "Donor type")]
    public string DonorType { get; set; } = "Individual";

    [Required(ErrorMessage = "Password is required."), DataType(DataType.Password)]
    [RegularExpression(Rules.PasswordPattern, ErrorMessage = Rules.PasswordMessage)]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";
}

public class VolunteerRegisterViewModel
{
    [Required(ErrorMessage = "Full name is required."), StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Email is required."), EmailAddress, Display(Name = "Email address")]
    public string Email { get; set; } = "";

    [Required, Display(Name = "Skills / Interests")]
    public string Skills { get; set; } = Options.Skills[0];

    [Required]
    public string Availability { get; set; } = Options.Availability[0];
}

public class BeneficiaryRegisterViewModel
{
    [Required(ErrorMessage = "Full name is required."), StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required."), Phone, Display(Name = "Phone number")]
    public string Phone { get; set; } = "";

    [Required, Display(Name = "Household size")]
    public string HouseholdSize { get; set; } = Options.Households[0];

    [Required, Display(Name = "Nearest hub")]
    public string NearestHub { get; set; } = Options.BeneficiaryHubs[0];
}

public class RegisterPageViewModel
{
    public string ActiveTab { get; set; } = "donor";
    public DonorRegisterViewModel Donor { get; set; } = new();
    public VolunteerRegisterViewModel Volunteer { get; set; } = new();
    public BeneficiaryRegisterViewModel Beneficiary { get; set; } = new();
}

// ── Donate ───────────────────────────────────────────────────────────────────

public class MoneyDonationViewModel
{
    public string PresetAmount { get; set; } = "50";   // 50|100|250|500|1000|custom

    [Range(0, 10_000_000, ErrorMessage = "Enter a valid amount.")]
    [Display(Name = "Custom amount (ZAR)")]
    public double? CustomAmount { get; set; }

    public string Frequency { get; set; } = "once";    // once | monthly

    public double Amount => PresetAmount == "custom"
        ? CustomAmount ?? 0
        : double.TryParse(PresetAmount, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var a) ? a : 0;
}

public class FoodDonationViewModel
{
    [Required(ErrorMessage = "Please describe the food items."), StringLength(500)]
    [Display(Name = "What are you donating?")]
    public string Description { get; set; } = "";

    [Required(ErrorMessage = "Please select a drop-off hub."), Display(Name = "Drop-off location")]
    public string Hub { get; set; } = "";
}

public class DonatePageViewModel
{
    public string ActiveTab { get; set; } = "money";
    public MoneyDonationViewModel Money { get; set; } = new();
    public FoodDonationViewModel Food { get; set; } = new();
    public bool IsLoggedIn { get; set; }
    public double UserTotal { get; set; }
    public int UserCount { get; set; }
}

// ── Home / Dashboard / Profile ───────────────────────────────────────────────

public class HomeViewModel
{
    public double MonthTotal { get; set; }
    public int MonthPercent { get; set; }
    public User? User { get; set; }
}

public class DashboardViewModel
{
    public User User { get; set; } = null!;
    public double MonthTotal { get; set; }
    public int MonthPercent { get; set; }
    public Milestone? NextMilestone { get; set; }
    public int MilestonePercent { get; set; }
    public List<Donation> Recent { get; set; } = new();
}

public record MilestoneStatus(Milestone Milestone, bool Reached);

public class ProfileSettingsViewModel
{
    [Required(ErrorMessage = "Full name is required."), StringLength(100), Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Phone, Display(Name = "Phone number")]
    public string? Phone { get; set; }

    [Required, Display(Name = "Donor type")]
    public string DonorType { get; set; } = "Individual";
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required."), DataType(DataType.Password), Display(Name = "Current password")]
    public string CurrentPassword { get; set; } = "";

    [Required, DataType(DataType.Password), Display(Name = "New password")]
    [RegularExpression(Rules.PasswordPattern, ErrorMessage = Rules.PasswordMessage)]
    public string NewPassword { get; set; } = "";

    [Required, DataType(DataType.Password), Display(Name = "Confirm password")]
    [Compare(nameof(NewPassword), ErrorMessage = "New passwords do not match.")]
    public string ConfirmPassword { get; set; } = "";
}

public class ProfilePageViewModel
{
    public string Tab { get; set; } = "overview";
    public User User { get; set; } = null!;
    public List<Donation> Donations { get; set; } = new();
    public List<MilestoneStatus> Milestones { get; set; } = new();
    public ProfileSettingsViewModel Settings { get; set; } = new();
    public ChangePasswordViewModel Password { get; set; } = new();
}

// ── Contact / Locations ──────────────────────────────────────────────────────

public class ContactViewModel
{
    [Required(ErrorMessage = "Please enter your name."), StringLength(100), Display(Name = "Your name")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email."), EmailAddress, Display(Name = "Your email")]
    public string Email { get; set; } = "";

    [StringLength(150)]
    public string Subject { get; set; } = "";

    [Required(ErrorMessage = "Please write a message."), StringLength(2000)]
    public string Message { get; set; } = "";
}

public class LocationsViewModel
{
    public IReadOnlyList<Hub> Hubs { get; set; } = HubData.All;
    public string MapboxToken { get; set; } = "";
}

// ── Admin ────────────────────────────────────────────────────────────────────

public class AdminDashboardViewModel
{
    public double TotalRaised { get; set; }
    public double MonthRaised { get; set; }
    public int TotalDonations { get; set; }
    public int TotalBeneficiaries { get; set; }
    public List<User> TopDonors { get; set; } = new();
    public List<Donation> RecentDonations { get; set; } = new();
}
