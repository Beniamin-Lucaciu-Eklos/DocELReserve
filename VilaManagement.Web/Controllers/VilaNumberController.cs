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
    public class VilaNumberController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public VilaNumberController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var vilaNumbers = _unitOfWork.VilaNumber.GetAll(includeProperties: new string[] { nameof(Vila) }).ToList();
            return View(vilaNumbers);
        }

        public IActionResult Create()
        {
            VilaNumberViewModel vmVillaNumber = new VilaNumberViewModel
            {
                VilaList = GetVitaList()
            };
            return View(vmVillaNumber);
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
        public IActionResult Create(VilaNumberViewModel vmVilaNumber)
        {
            ModelState.Remove($"{nameof(VilaNumber)}.{nameof(VilaNumber.Vila)}");//removing key from validation

            vmVilaNumber.VilaList ??= GetVitaList();

            bool isVilaNumberExists = _unitOfWork.VilaNumber.Any(vn => vn.Vila_Number == vmVilaNumber.VilaNumber.Vila_Number);
            if (ModelState.IsValid && !isVilaNumberExists)
            {
                _unitOfWork.VilaNumber.Add(vmVilaNumber.VilaNumber);
                _unitOfWork.SaveChanges();
                this.ShowSuccess("Vila number created successfully.");
                return RedirectToAction(nameof(Index));
            }

            if (isVilaNumberExists)
            {
                this.ShowError("Vila number already exists. Please choose a different number.");
            }
            return View(vmVilaNumber);
        }

        public IActionResult Edit(int vilaNumberId)
        {
            VilaNumberViewModel vmVillaNumber = new VilaNumberViewModel
            {
                VilaList = GetVitaList(),
                VilaNumber = _unitOfWork.VilaNumber.Get(vila => vila.Vila_Number == vilaNumberId)
            };
            if (vmVillaNumber.VilaNumber is null)
            {
                this.ShowError("Vila number not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            return View(vmVillaNumber);
        }

        [HttpPost]
        public IActionResult Edit(VilaNumberViewModel vmVilaNumber)
        {
            ModelState.Remove($"{nameof(VilaNumber)}.{nameof(VilaNumber.Vila)}");//removing key from validation

            vmVilaNumber.VilaList ??= GetVitaList();
            if (ModelState.IsValid)
            {
                _unitOfWork.VilaNumber.Update(vmVilaNumber.VilaNumber);
                _unitOfWork.SaveChanges();
                this.ShowSuccess("Vila number updated successfully.");
                return RedirectToAction(nameof(Index));
            }
            return View(vmVilaNumber);
        }

        public IActionResult Delete(int vilaNumberId)
        {
            VilaNumberViewModel vmVillaNumber = new VilaNumberViewModel
            {
                VilaList = GetVitaList(),
                VilaNumber = _unitOfWork.VilaNumber.Get(vila => vila.Vila_Number == vilaNumberId)!
            };
            return View(vmVillaNumber);
        }

        [HttpPost]
        public IActionResult Delete(VilaNumberViewModel vmVilaNumber)
        {
            var dbVilaNumber = _unitOfWork.VilaNumber.Get(vilaNumber => vilaNumber.Vila_Number == vmVilaNumber.VilaNumber.Vila_Number);
            if (dbVilaNumber is null)
            {
                this.ShowError("Vila number not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            _unitOfWork.VilaNumber.Remove(dbVilaNumber);
            _unitOfWork.SaveChanges();

            this.ShowSuccess($"Vila number {dbVilaNumber.Vila_Number} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
