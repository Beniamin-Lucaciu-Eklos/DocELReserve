using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Web.Controllers
{
    public class VilasController : Controller
    {
        private readonly ApplicationDBContext _db;

        public VilasController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var vilas = _db.Vilas.ToList();
            return View(vilas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Vila vila)
        {
            if (vila.Name == vila.Description)
                ModelState.AddModelError(nameof(Vila.Name), "The description cannot exactly match the Name.");

            if(!ModelState.IsValid)
            {
                return View();
            }

            _db.Vilas.Add(vila);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
