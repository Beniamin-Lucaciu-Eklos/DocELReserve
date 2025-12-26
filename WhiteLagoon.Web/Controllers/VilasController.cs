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

        private void ToastSuccess(string message)
        {
            TempData["SuccesMessage"] = message;
        }
        private void ToastError(string message)
        {
            TempData["ErrorMessage"] = message;
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

            if (!ModelState.IsValid)
            {
                ToastError("Error while creating Vila. Please try again.");
                return View();
            }

            _db.Vilas.Add(vila);
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var vilaFromDb = _db.Vilas.FirstOrDefault(vila => vila.Id == id);
            if (vilaFromDb is null)
            {
                ToastError("Vila not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View(vilaFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Vila vila)
        {
            if (ModelState.IsValid && vila.Id > 0)
            {
                _db.Vilas.Update(vila);
                _db.SaveChanges();

                ToastSuccess("Vila updated successfully.");
                return RedirectToAction(nameof(Index));
            }

            ToastError("Error while updating Vila. Please try again.");
            return View(vila);
        }


        public IActionResult Delete(int id)
        {
            var vilaFromDb = _db.Vilas.FirstOrDefault(vila => vila.Id == id);
            if (vilaFromDb is null)
            {
                ToastError("Vila not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            _db.Vilas.Remove(vilaFromDb);
            _db.SaveChanges();

            ToastSuccess($"Vila {vilaFromDb.Name} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
