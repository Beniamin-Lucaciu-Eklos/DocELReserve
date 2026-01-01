using Microsoft.AspNetCore.Mvc;

namespace VilaManagement.Web.Controllers
{
    public class BootstrapSheetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
