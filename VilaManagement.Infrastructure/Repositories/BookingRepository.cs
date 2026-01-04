using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.Common;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;

namespace VilaManagement.Infrastructure.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDBContext _db;

        public BookingRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }

        public void UpdateOrderStatus(int bookingId, string orderStatus)
        {
            var booking = Get(b => b.Id == bookingId);
            if (booking is null)
                return;

            booking.OrderStatus = orderStatus;
            (booking.ActualCheckInDate, booking.ActualCheckOutDate) = orderStatus switch
            {
                BookingOrderStatus.CheckededIn => (DateTime.Now, booking.ActualCheckOutDate),
                BookingOrderStatus.Completed => (booking.ActualCheckInDate, DateTime.Now),

                _ => (booking.ActualCheckInDate, booking.ActualCheckOutDate)
            };
        }

        public void UpdateStripePaymentId(int bookingId, string sessionID, string paymentIntentId)
        {
            var booking = Get(b => b.Id == bookingId);
            if (booking is null)
                return;

            booking.StripeSessionId = !string.IsNullOrWhiteSpace(sessionID) ? sessionID : booking.StripeSessionId;
            (booking.StripePaymentIntentId, booking.PaymentDate, booking.IsPaymentSuccessfull) = paymentIntentId switch
            {
                string paymentIntentIdResult when !string.IsNullOrWhiteSpace(paymentIntentIdResult) => (paymentIntentId, DateTime.Now, true),
                _ => (booking.StripePaymentIntentId, booking.PaymentDate, booking.IsPaymentSuccessfull)
            };
        }
    }
}
