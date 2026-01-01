using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxuryVilaManagement.Application.Common.Interfaces;
using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Infrastructure.Data;

namespace LuxuryVilaManagement.Infrastructure.Repositories
{
    public class AmenityRepository : Repository<Amenity>, IAmenityRepository
    {
        private readonly ApplicationDBContext _db;

        public AmenityRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }
    }
}
