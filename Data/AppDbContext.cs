using DMDFoodBank.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DMDFoodBank.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<Volunteer> Volunteers => Set<Volunteer>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<Donation>()
            .HasOne(d => d.User).WithMany(u => u.Donations)
            .HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public static class DbSeeder
{
    /// <summary>Creates the database and the first administrator (password is hashed).</summary>
    public static async Task SeedAsync(IServiceProvider sp, IConfiguration config)
    {
        var db = sp.GetRequiredService<AppDbContext>();
        var hasher = sp.GetRequiredService<IPasswordHasher<User>>();
        await db.Database.EnsureCreatedAsync();

        if (await db.Users.AnyAsync(u => u.Role == Roles.Admin)) return;

        var admin = new User
        {
            FullName = "Admin User",
            Email = (config["Admin:Email"] ?? "admin@dmdfoodbank.org").ToLowerInvariant(),
            Role = Roles.Admin,
            Phone = "0000000000"
        };
        admin.PasswordHash = hasher.HashPassword(admin, config["Admin:InitialPassword"] ?? "Admin@1234");
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
