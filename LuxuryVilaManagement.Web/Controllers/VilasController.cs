using Microsoft.AspNetCore.Mvc;
using LuxuryVilaManagement.Application.Common.Interfaces;
using LuxuryVilaManagement.Application.IO;
using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Infrastructure.Data;
using LuxuryVilaManagement.Web.UIHelpers;

namespace LuxuryVilaManagement.Web.Controllers
{
    public class VilasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFilePathService _svcfilePath;

        public VilasController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment,
            IFilePathService svcfilePath)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _svcfilePath = svcfilePath;
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
                var vilaImagePath = _svcfilePath.GetFullPath(vila.Image, _webHostEnvironment.WebRootPath, FileStorageConstants.VilaImagesFolderPath);
                _svcfilePath.Upload(vila.Image, vilaImagePath);

                var fileName = _svcfilePath.GetFileNameFromFullPath(vilaImagePath);
                vila.ImageUrl = _svcfilePath.CreateRootPath(FileStorageConstants.VilaImagesFolderPath, fileName);
            }
            else
                vila.ImageUrl = FileStorageConstants.NoImageVilaPlaceHolder;

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
                    var vilaImagePath = _svcfilePath.GetFullPath(vila.Image, _webHostEnvironment.WebRootPath, FileStorageConstants.VilaImagesFolderPath);
                    _svcfilePath.Upload(vila.Image, vilaImagePath);

                    var fileName = _svcfilePath.GetFileNameFromFullPath(vilaImagePath);
                    vila.ImageUrl = _svcfilePath.CreateRootPath(FileStorageConstants.VilaImagesFolderPath, fileName);
                }
                else
                    vila.ImageUrl = FileStorageConstants.NoImageVilaPlaceHolder;

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
                _svcfilePath.Delete(oldImage);
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
