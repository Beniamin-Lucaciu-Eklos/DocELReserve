using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LuxuryVilaManagement.Application.Common.Interfaces;
using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Web.Models;

namespace LuxuryVilaManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var vilas = _unitOfWork.Villa.GetAll(includeProperties: new string[] { nameof(Vila.Amenities) });
            var vm = new ViewModels.HomeViewModel
            {
                Vilas = vilas,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                NumberOfNights = 1
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
