using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Web.UIHelpers;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Controllers
{
    public class VilaNumberController : Controller
    {
        private readonly ApplicationDBContext _db;

        public VilaNumberController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var vilaNumbers = _db.VilaNumbers.Include(vilaNumber => vilaNumber.Vila).ToList();
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
            return _db.Vilas
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

            bool isVilaNumberExists = _db.VilaNumbers.Any(vn => vn.Vila_Number == vmVilaNumber.VilaNumber.Vila_Number);
            if (ModelState.IsValid && !isVilaNumberExists)
            {
                _db.VilaNumbers.Add(vmVilaNumber.VilaNumber);
                _db.SaveChanges();
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
                VilaNumber = _db.VilaNumbers.FirstOrDefault(vila => vila.Vila_Number == vilaNumberId)
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
                _db.VilaNumbers.Update(vmVilaNumber.VilaNumber);
                _db.SaveChanges();
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
                VilaNumber = _db.VilaNumbers.FirstOrDefault(vila => vila.Vila_Number == vilaNumberId)!
            };
            return View(vmVillaNumber);
        }

        [HttpPost]
        public IActionResult Delete(VilaNumberViewModel vmVilaNumber)
        {
            var dbVilaNumber = _db.VilaNumbers.FirstOrDefault(vilaNumber =>vilaNumber.Vila_Number == vmVilaNumber.VilaNumber.Vila_Number);
            if (dbVilaNumber is null)
            {
                this.ShowError("Vila number not found.");
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }

            _db.VilaNumbers.Remove(dbVilaNumber);
            _db.SaveChanges();

            this.ShowSuccess($"Vila number {dbVilaNumber.Vila_Number} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}
