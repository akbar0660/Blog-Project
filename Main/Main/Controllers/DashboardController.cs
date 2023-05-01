using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers
{
    public class DashboardController : Controller
    {
        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
