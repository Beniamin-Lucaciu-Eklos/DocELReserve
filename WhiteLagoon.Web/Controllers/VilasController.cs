using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.UIHelpers;

namespace WhiteLagoon.Web.Controllers
{
    public class VilasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VilasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var vilas = _unitOfWork.Villa.GetAll();
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
                this.ShowError("Error while creating Vila. Please try again.");
                return View();
            }

            _unitOfWork.Villa.Add(vila);
            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var vilaFromDb = _unitOfWork.Villa.Get(vila => vila.Id == id);
            if (vilaFromDb is null)
            {
                this.ShowError("Vila not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View(vilaFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Vila vila)
        {
            if (ModelState.IsValid && vila.Id > 0)
            {
                _unitOfWork.Villa.Update(vila);
                _unitOfWork.SaveChanges();

                this.ShowSuccess("Vila updated successfully.");
                return RedirectToAction(nameof(Index));
            }

            this.ShowError("Error while updating Vila. Please try again.");
            return View(vila);
        }


        public IActionResult Delete(int id)
        {
            var vilaFromDb = _unitOfWork.Villa.Get(vila => vila.Id == id);
            if (vilaFromDb is null)
            {
                this.ShowError("Vila not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            _unitOfWork.Villa.Remove(vilaFromDb);
            _unitOfWork.SaveChanges();

            this.ShowSuccess($"Vila {vilaFromDb.Name} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
