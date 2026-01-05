using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
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

            //var options = new SessionCreateOptions
            //{
            //    LineItems = new List<SessionLineItemOptions>(),
            //    Mode = "payment",
            //    SuccessUrl = $"{domain}booking/confirmation?booking={booking.Id}.html",
            //    CancelUrl = $"{domain}booking/finalize?vilaId={vila.Id}&checkInDate={booking.CheckInDate}&numberOfNights={booking.Nights} .html",
            //};
            //options.LineItems.Add(new SessionLineItemOptions
            //{
            //    PriceData = new SessionLineItemPriceDataOptions
            //    {
            //        UnitAmount = (long)booking.TotalCost * 100,
            //        Currency = "usd",
            //        ProductData = new SessionLineItemPriceDataProductDataOptions
            //        {
            //            Name = vila.Name,
            //            //       Images = new List<string> { domain + vila.ImageUrl }
            //        },
            //    },
            //    Quantity = 1,
            //});

            //var service = new SessionService();
            //Session session = service.Create(options);

            //_unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            //_unitOfWork.SaveChanges();

            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);

            return RedirectToAction(nameof(Confirmation), new { bookingId = booking.Id });

            //var options = _paymentService.CreateStripeSessionOptions(booking, villa, domain);

            //var session = _paymentService.CreateStripeSession(options);

            //_unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);

        }

        [Authorize]
        public IActionResult Confirmation(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId, new string[] { nameof(Booking.Vila), nameof(Booking.User) });
            if (booking is Booking { OrderStatus: BookingOrderStatus.Pending })
            {
                //    var service = new SessionService();

                //    var session = service.Get(booking.StripeSessionId);
                //    if (session.PaymentStatus == "paid")
                //    {
                //        _unitOfWork.Booking.UpdateOrderStatus(bookingId, BookingOrderStatus.Approved);
                //        _unitOfWork.Booking.UpdateStripePaymentId(bookingId, session.Id, session.PaymentIntentId);
                //        _unitOfWork.SaveChanges();
                //    }
            }

            return View(bookingId);
        }

    }
}
