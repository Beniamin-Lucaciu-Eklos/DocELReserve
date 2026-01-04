using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VilaManagement.Application.Common;
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


        [Authorize]
        [HttpPost]
        public IActionResult Finalize(Booking booking)
        {
            var vila = _unitOfWork.Villa.Get(v => v.Id == booking.VilaId, includedProperties: new string[] { "Amenities" });
            booking.TotalCost = vila.Price * booking.Nights;

            booking.OrderStatus = BookingOrderStatus.Pending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.SaveChanges();

            //if (!_villaService.IsVillaAvailableByDate(villa.Id, booking.Nights, booking.CheckInDate))
            //{
            //    TempData["error"] = "Room has been sold out!";
            //    //no rooms available
            //    return RedirectToAction(nameof(FinalizeBooking), new
            //    {
            //        villaId = booking.VillaId,
            //        checkInDate = booking.CheckInDate,
            //        nights = booking.Nights
            //    });
            //}




            //_bookingService.CreateBooking(booking);

            //var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            //var options = _paymentService.CreateStripeSessionOptions(booking, villa, domain);

            //var session = _paymentService.CreateStripeSession(options);

            //_unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);

            return RedirectToAction(nameof(Confirmation), new { bookingId = booking.Id });
        }

        [Authorize]
        public IActionResult Confirmation(int bookingId)
        {
            return View(bookingId);
        }

    }
}
