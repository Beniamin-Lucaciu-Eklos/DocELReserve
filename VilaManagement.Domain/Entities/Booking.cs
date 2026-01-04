using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VilaManagement.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string? Phone { get; set; }

        public double TotalCost { get; set; }

        public int Nights { get; set; }

        public string? OrderStatus { get; set; }

        public DateTime BookingDate { get; set; }

        public DateOnly CheckInDate { get; set; }

        public DateOnly CheckOutDate { get; set; }

        public bool IsPaymentSuccessfull { get; set; } = false;
        public DateTime PaymentDate { get; set; }

        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }

        public DateTime ActualCheckInDate { get; set; }
        public DateTime ActualCheckOutDate { get; set; }

        public int VilaNumber { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int VilaId { get; set; }
        public Vila Vila { get; set; }
    }

    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.UserId)
                .IsRequired();

            builder.Property(b => b.Name)
                .IsRequired();

            builder.Property(b => b.Email)
                .IsRequired();

            builder.Property(b => b.TotalCost)
                .IsRequired();

            builder.Property(b => b.BookingDate)
             .IsRequired();

            builder.Property(b => b.CheckInDate)
                .IsRequired();

            builder.Property(b => b.CheckOutDate)
                .IsRequired();
        }
    }
}
