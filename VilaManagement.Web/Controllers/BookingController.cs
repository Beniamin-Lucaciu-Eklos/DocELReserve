using Microsoft.AspNetCore.Mvc;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Finalize(int vilaId, DateOnly checkInDate, int numberOfNights)
        {
            Booking booking = new Booking
            {
                VilaId = vilaId,
                Vila = _unitOfWork.Villa.Get(v => v.Id == vilaId, includedProperties: new string[] { "Amenities" }),
                CheckInDate = checkInDate,
                Nights = numberOfNights,
                CheckOutDate = checkInDate.AddDays(numberOfNights),
            };

            return View(booking);
        }
    }
}
