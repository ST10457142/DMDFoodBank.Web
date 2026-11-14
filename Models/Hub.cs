namespace DMDFoodBank.Web.Models;

public enum StockStatus { Full, Low, Critical }

public record Hub(string Name, string Address, string Hours, string Phone,
                  StockStatus Status, double Latitude, double Longitude)
{
    public string StatusLabel => Status switch
    {
        StockStatus.Full => "Fully stocked",
        StockStatus.Low => "Low stock",
        _ => "Critical – needs help"
    };

    public string StatusCss => Status switch
    {
        StockStatus.Full => "status-full",
        StockStatus.Low => "status-low",
        _ => "status-critical"
    };

    public string StatusColor => Status switch
    {
        StockStatus.Full => "#2E7D32",
        StockStatus.Low => "#F9A825",
        _ => "#C62828"
    };
}

public static class HubData
{
    public static readonly IReadOnlyList<Hub> All = new List<Hub>
    {
        new("Soweto Community Hub", "123 Ubuntu St, Soweto, 1804", "Mon–Sat: 08:00–16:00", "011 111 2222", StockStatus.Full, -26.2678, 27.8585),
        new("Alexandra Distribution Centre", "45 Pan Africa Ave, Alexandra, 2090", "Mon–Fri: 09:00–15:00", "011 222 3333", StockStatus.Low, -26.1052, 28.0918),
        new("Tembisa Food Bank", "78 Ivory Park Rd, Tembisa, 1632", "Mon–Sat: 08:00–17:00", "011 333 4444", StockStatus.Full, -25.9964, 28.2268),
        new("Roodepoort Centre", "12 West Rand Dr, Roodepoort, 1725", "Mon–Fri: 08:00–15:00", "011 444 5555", StockStatus.Full, -26.1625, 27.8725),
        new("Diepsloot Hub", "99 Cluster A, Diepsloot, 2189", "Tue & Thu: 09:00–14:00", "011 555 6666", StockStatus.Critical, -25.9345, 28.0114),
        new("Orange Farm Depot", "34 Main Rd, Orange Farm, 1805", "Mon, Wed, Fri: 09:00–13:00", "011 666 7777", StockStatus.Low, -26.4650, 27.8650),
        new("Katlehong Centre", "56 Natalspruit Rd, Katlehong, 1431", "Mon–Sat: 08:00–16:00", "011 777 8888", StockStatus.Full, -26.3350, 28.1500),
    };
}

public record Milestone(string Icon, string Label, string Description, double Threshold, int CountThreshold = 0)
{
    public bool IsReached(double totalDonated, int donationCount) =>
        totalDonated >= Threshold && donationCount >= CountThreshold;
}

public static class Milestones
{
    public static readonly IReadOnlyList<Milestone> All = new List<Milestone>
    {
        new("🌱", "First Donation", "Made your very first donation", 0.01, 1),
        new("🌟", "R 500 Milestone", "Total giving exceeded R 500", 500),
        new("🔥", "Regular Donor", "Made 5 or more donations", 0, 5),
        new("💫", "R 1,000 Club", "Total giving exceeded R 1,000", 1000),
        new("🏅", "R 5,000 Benefactor", "Total giving exceeded R 5,000", 5000),
        new("🏆", "R 10,000 Champion", "Total giving exceeded R 10,000", 10000),
    };

    public static Milestone? Next(double totalDonated) =>
        All.FirstOrDefault(m => totalDonated < m.Threshold && m.Threshold > 0);
}

public static class Options
{
    public static readonly string[] DonorTypes =
        { "Individual", "Corporate / Business", "Government / Parastatal", "Religious / Community Org" };
    public static readonly string[] Skills =
        { "Sorting & Packing", "Food Distribution", "Administration", "Driving / Logistics", "Fundraising", "Social Media" };
    public static readonly string[] Availability = { "Weekdays only", "Weekends only", "Flexible" };
    public static readonly string[] Households = { "1–2 people", "3–4 people", "5–6 people", "7+ people" };
    public static readonly string[] BeneficiaryHubs =
        { "Soweto Community Hub", "Alexandra Distribution Centre", "Tembisa Food Bank", "Roodepoort Centre" };
    public static readonly (double Amount, string Meals)[] Presets =
        { (50, "5 meals"), (100, "10 meals"), (250, "25 meals"), (500, "50 meals"), (1000, "100 meals") };
}
