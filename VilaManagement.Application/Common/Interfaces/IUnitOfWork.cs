using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VilaManagement.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IVilaRepository Villa { get; }

        IVilaNumberRepository VilaNumber { get; }

        IAmenityRepository Amenity { get; }

        IBookingRepository Booking { get; }

        IApplicationUserRepository ApplicationUser { get; }

        void SaveChanges();
    }
}
