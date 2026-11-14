# DMD Food Bank – ASP.NET Core MVC website

Web version of the DMD Food Bank Android app.

## Run it
Requires the .NET SDK (the project targets `net10.0`; for .NET 8 change `TargetFramework` to `net8.0`
and the EF Core package version to `8.0.x`).

```bash
cd DMDFoodBank.Web
dotnet run
```
Open http://localhost:5080. The SQLite database (`dmd_foodbank.db`) is created on first run.

First admin account (change it after signing in, or edit `Admin` in `appsettings.json` before the first run):
`admin@dmdfoodbank.org` / `Admin@1234`

## MVC structure
| Layer | Folder | Contents |
|---|---|---|
| Model | `Models/`, `Data/`, `Services/` | Entities, EF Core `AppDbContext`, `AccountService`, `DonationService`, `AdminService` |
| View | `Views/`, `wwwroot/` | Razor views, layout, CSS |
| Controller | `Controllers/` | Home, Account, Donate, Dashboard, Profile, Locations, Contact, Admin |
| ViewModels | `ViewModels/` | Form and page models with validation rules |

## Security
- Passwords are hashed (ASP.NET Core `PasswordHasher`), never stored as text
- Cookie authentication; the Admin area requires the `admin` role
- Anti-forgery token on every POST; all output is HTML-encoded by Razor
- Same password rules as the app: 8+ characters, an uppercase letter and a number
- The Mapbox token is a public (`pk.`) token in `appsettings.json`
