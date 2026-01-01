using VilaManagement.Domain.Entities;

namespace VilaManagement.Web.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Vila> Vilas { get; set; }

        public DateOnly CheckInDate { get; set; }

        public DateOnly CheckOutDate { get; set; }

        public int NumberOfNights { get; set; }

    }
}
