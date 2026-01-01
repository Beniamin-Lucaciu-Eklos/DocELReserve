using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;
using VilaManagement.Web.UIHelpers;
using VilaManagement.Web.ViewModels;

namespace VilaManagement.Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: new string[] { nameof(Vila) }).ToList();
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityViewModel vmAmenity = new AmenityViewModel
            {
                VilaList = GetVitaList()
            };
            return View(vmAmenity);
        }

        private List<SelectListItem> GetVitaList()
        {
            return _unitOfWork.Villa.GetAll()
                            .Select(vila => new SelectListItem
                            {
                                Text = vila.Name,
                                Value = vila.Id.ToString()
                            }).ToList();
        }

        [HttpPost]
        public IActionResult Create(AmenityViewModel vmAmenity)
        {
            ModelState.Remove($"{nameof(Amenity)}.{nameof(VilaNumber.Vila)}");//removing key from validation

            vmAmenity.VilaList ??= GetVitaList();

            bool isAmenityNameExists = _unitOfWork.Amenity.Any(vn => vn.Name == vmAmenity.Amenity.Name);
            if (ModelState.IsValid && !isAmenityNameExists)
            {
                _unitOfWork.Amenity.Add(vmAmenity.Amenity);
                _unitOfWork.SaveChanges();
                this.ShowSuccess("Amenity created successfully.");
                return RedirectToAction(nameof(Index));
            }

            if (isAmenityNameExists)
            {
                this.ShowError("Vila number already exists. Please choose a different number.");
            }
            return View(vmAmenity);
        }

        public IActionResult Edit(int amenityId)
        {
            AmenityViewModel vmAmenity = new AmenityViewModel
            {
                VilaList = GetVitaList(),
                Amenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityId)
            };
            if (vmAmenity.Amenity is null)
            {
                this.ShowError("Amenity not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View(vmAmenity);
        }

        [HttpPost]
        public IActionResult Edit(AmenityViewModel vmAmenity)
        {
            ModelState.Remove($"{nameof(Amenity)}.{nameof(VilaNumber.Vila)}");//removing key from validation

            vmAmenity.VilaList ??= GetVitaList();
            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Update(vmAmenity.Amenity);
                _unitOfWork.SaveChanges();
                this.ShowSuccess("Amenity updated successfully.");
                return RedirectToAction(nameof(Index));
            }
            return View(vmAmenity);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityViewModel vmAmenity = new AmenityViewModel
            {
                VilaList = GetVitaList(),
                Amenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == amenityId)
            };
            return View(vmAmenity);
        }

        [HttpPost]
        public IActionResult Delete(AmenityViewModel vmAmenity)
        {
            var dbAmenity = _unitOfWork.Amenity.Get(amenity => amenity.Id == vmAmenity.Amenity.Id);
            if (dbAmenity is null)
            {
                this.ShowError("Amenity not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            _unitOfWork.Amenity.Remove(dbAmenity);
            _unitOfWork.SaveChanges();

            this.ShowSuccess($"Amenity {dbAmenity.Name} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
