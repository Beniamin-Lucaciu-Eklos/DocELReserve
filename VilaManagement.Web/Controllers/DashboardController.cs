using Microsoft.AspNetCore.Mvc;

namespace VilaManagement.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
