using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Application.IO;
using VilaManagement.Application.Services;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;
using VilaManagement.Web.UIHelpers;

namespace VilaManagement.Web.Controllers
{
    [Authorize]
    public class VilasController : Controller
    {
        private readonly IVilaService _vilaService;

        public VilasController(IVilaService vilaService)
        {
            _vilaService = vilaService;
        }

        public IActionResult Index()
        {
            var vilas = _vilaService.GetAll();
            return View(vilas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Vila vila)
        {
            ValidateVilaModel(vila);

            if (!ModelState.IsValid)
            {
                this.ShowError("Error while creating Vila. Please try again.");
                return View(vila);
            }

            ProcessVilaImage(vila);

            _vilaService.Add(vila);

            this.ShowSuccess("Vila created successfully.");
            return RedirectToAction(nameof(Index));
        }

        private void ValidateVilaModel(Vila vila)
        {
            if (vila.Name == vila.Description)
            {
                ModelState.AddModelError(
                    nameof(Vila.Name),
                    "The description cannot exactly match the Name.");
            }

            ModelState.Remove(nameof(Vila.ImageUrl));
        }

        public IActionResult Edit(int id)
        {
            var vila = _vilaService.Get(id);
            if (vila is null)
            {
                return RedirectToErrorPage("Vila not found.");
            }

            return View(vila);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Vila vila)
        {
            if (!ModelState.IsValid || vila.Id <= 0)
            {
                this.ShowError("Error while updating Vila. Please try again.");
                return View(vila);
            }

            if (vila.Image is not null)
            {
                _vilaService.DeleteImage(vila);
                ProcessVilaImage(vila);
            }

            _vilaService.Update(vila);

            this.ShowSuccess("Vila updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        private void ProcessVilaImage(Vila vila)
        {
            _vilaService.SaveImageUrl(vila);
            _vilaService.UploadImage(vila);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var vila = _vilaService.Get(id);
            if (vila is null)
            {
                return RedirectToErrorPage("Vila not found.");
            }

            _vilaService.DeleteImage(vila);
            _vilaService.Remove(vila);

            this.ShowSuccess($"Vila '{vila.Name}' deleted successfully.");
            return RedirectToAction(nameof(Index));
        }

        private IActionResult RedirectToErrorPage(string errorMessage)
        {
            this.ShowError(errorMessage);
            return RedirectToAction(nameof(HomeController.Error), "Home");
        }
    }
}
