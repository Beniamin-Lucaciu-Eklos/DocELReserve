using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        [Authorize]
        public IActionResult Finalize(int vilaId, DateOnly checkInDate, int numberOfNights)
        {
            var userId = User.Identity switch
            {
                ClaimsIdentity { IsAuthenticated: true } ci => ci.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                _ => throw new KeyNotFoundException()
            };

            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            Booking booking = new Booking
            {
                VilaId = vilaId,
                Vila = _unitOfWork.Villa.Get(v => v.Id == vilaId, includedProperties: new string[] { "Amenities" }),
                CheckInDate = checkInDate,
                Nights = numberOfNights,
                CheckOutDate = checkInDate.AddDays(numberOfNights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name
            };
            booking.TotalCost = booking.Vila.Price * numberOfNights;
            return View(booking);
        }
    }
}
