using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Web.Models;

namespace VilaManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IOptions<RequestLocalizationOptions> _locaOptions;

        public HomeController(IUnitOfWork unitOfWork,
            IStringLocalizer<HomeController> localizer,
            IOptions<RequestLocalizationOptions> locOptions)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _locaOptions = locOptions;
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

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { Expires = DateTimeOffset.UtcNow.AddSeconds(10) }
    );


            return LocalRedirect(returnUrl);
        }
    }
}
