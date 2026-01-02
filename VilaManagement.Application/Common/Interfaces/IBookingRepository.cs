using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {       
    }
}
