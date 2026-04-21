using Microsoft.AspNetCore.Mvc;

namespace TaskManagementSystem.Web.Controllers;

public class HomeController : Controller
{
    [Route("Home/Error")]
    public IActionResult Error(int statusCode = 500, string message = "An unexpected error occurred.")
    {
        Response.StatusCode = statusCode;
        ViewData["StatusCode"] = statusCode;
        ViewData["Message"] = message;
        return View("Error");
    }
}
