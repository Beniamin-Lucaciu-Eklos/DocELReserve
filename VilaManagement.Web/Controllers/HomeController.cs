using Azure.Core.Pipeline;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Web.Models;
using VilaManagement.Web.ViewModels;

namespace VilaManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IUnitOfWork unitOfWork,
            IStringLocalizer<HomeController> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
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

        [HttpPost]
        public IActionResult GetVilasByDate(int numberOfNights, DateOnly checkInDate)
        {
            var vilas = _unitOfWork.Villa.GetAll(includeProperties: new string[] { nameof(Vila.Amenities) });
            foreach (var vila in vilas)
            {
                if (vila.Id % 2 == 0)
                    vila.IsAvailable = false;
            }

            HomeViewModel vmHome = new()
            {
                NumberOfNights = numberOfNights,
                CheckInDate = checkInDate,
                Vilas = vilas,
            };

            return PartialView("_VilaList", vmHome);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
    );
            return LocalRedirect(returnUrl);
        }
    }
}
