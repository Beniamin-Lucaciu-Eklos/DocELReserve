using Microsoft.AspNetCore.Mvc;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.UIHelpers;

namespace WhiteLagoon.Web.Controllers
{
    public class VilasController : Controller
    {
        private const string DefaultImagePlaceHolder = "https://placehold.co/400";
        private const string DefaultVilaFolderPath = @"images\Vila";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VilasController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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

            ModelState.Remove(nameof(Vila.ImageUrl));

            if (!ModelState.IsValid)
            {
                this.ShowError("Error while creating Vila. Please try again.");
                return View();
            }
            if (vila.Image is not null)
            {
                var vilaImagePath = vila.Image.GetFullPath(_webHostEnvironment.WebRootPath, DefaultVilaFolderPath);
                vila.Image.CopyTo(vilaImagePath);

                var fileName = vilaImagePath.GetFileNameFromFullPath();
                vila.ImageUrl = FileHelper.CreateRootPath(DefaultVilaFolderPath, fileName);
            }
            else
                vila.ImageUrl = DefaultImagePlaceHolder;

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
                RemoveVilaImageFile(vila);

                if (vila.Image is not null)
                {
                    var vilaImagePath = vila.Image.GetFullPath(_webHostEnvironment.WebRootPath, DefaultVilaFolderPath);
                    vila.Image.CopyTo(vilaImagePath);

                    var fileName = vilaImagePath.GetFileNameFromFullPath();
                    vila.ImageUrl = FileHelper.CreateRootPath(DefaultVilaFolderPath, fileName);
                }
                else
                    vila.ImageUrl = DefaultImagePlaceHolder;

                _unitOfWork.Villa.Update(vila);
                _unitOfWork.SaveChanges();

                this.ShowSuccess("Vila updated successfully.");
                return RedirectToAction(nameof(Index));
            }

            this.ShowError("Error while updating Vila. Please try again.");
            return View(vila);
        }

        private void RemoveVilaImageFile(Vila vila)
        {
            if (vila.ImageUrl is not null)
            {
                var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, vila.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImage))
                    System.IO.File.Delete(oldImage);
            }
        }

        public IActionResult Delete(int id)
        {
            var vilaFromDb = _unitOfWork.Villa.Get(vila => vila.Id == id);
            if (vilaFromDb is null)
            {
                this.ShowError("Vila not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            RemoveVilaImageFile(vilaFromDb);

            _unitOfWork.Villa.Remove(vilaFromDb);
            _unitOfWork.SaveChanges();

            this.ShowSuccess($"Vila {vilaFromDb.Name} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
