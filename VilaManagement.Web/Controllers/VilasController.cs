using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Application.IO;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;
using VilaManagement.Web.UIHelpers;

namespace VilaManagement.Web.Controllers
{
    [Authorize]
    public class VilasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFilePathService _filePathService;

        public VilasController(
        IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment,
        IFilePathService filePathService)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _filePathService = filePathService;
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

            _unitOfWork.Villa.Add(vila);
            _unitOfWork.SaveChanges();

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
            var vila = GetVilaById(id);
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
                DeleteExistingImage(vila);
                ProcessVilaImage(vila);
            }

            _unitOfWork.Villa.Update(vila);
            _unitOfWork.SaveChanges();

            this.ShowSuccess("Vila updated successfully.");
            return RedirectToAction(nameof(Index));
        }

        private void ProcessVilaImage(Vila vila)
        {
            if (vila.Image is not null)
            {
                var imagePath = _filePathService.GetFullPath(
                    vila.Image,
                    _webHostEnvironment.WebRootPath,
                    FileStorageConstants.VilaImagesFolderPath);

                _filePathService.Upload(vila.Image, imagePath);

                var fileName = _filePathService.GetFileNameFromFullPath(imagePath);
                vila.ImageUrl = _filePathService.CreateRelativePath(
                    FileStorageConstants.VilaImagesFolderPath,
                    fileName);
            }
            else
            {
                vila.ImageUrl = FileStorageConstants.NoImageVilaPlaceHolder;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var vila = GetVilaById(id);
            if (vila is null)
            {
                return RedirectToErrorPage("Vila not found.");
            }

            DeleteExistingImage(vila);

            _unitOfWork.Villa.Remove(vila);
            _unitOfWork.SaveChanges();

            this.ShowSuccess($"Vila '{vila.Name}' deleted successfully.");
            return RedirectToAction(nameof(Index));
        }

        private Vila GetVilaById(int id)
        {
            return _unitOfWork.Villa.Get(v => v.Id == id);
        }

        private void DeleteExistingImage(Vila vila)
        {
            if (string.IsNullOrEmpty(vila.ImageUrl)
                || vila.ImageUrl == FileStorageConstants.NoImageVilaPlaceHolder)
            {
                return;
            }

            var normalizedPath = vila.ImageUrl
                .Replace("~/", string.Empty)
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, normalizedPath);
            _filePathService.Delete(fullPath);
        }

        private IActionResult RedirectToErrorPage(string errorMessage)
        {
            this.ShowError(errorMessage);
            return RedirectToAction(nameof(HomeController.Error), "Home");
        }
    }
}
