using DMDFoodBank.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DMDFoodBank.Web.Controllers;

public class LocationsController(IConfiguration config) : Controller
{
    public IActionResult Index() =>
        View(new LocationsViewModel { MapboxToken = config["Mapbox:AccessToken"] ?? "" });
}
