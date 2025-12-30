using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repositories
{
    public class AmenityRepository : Repository<Amenity>, IAmenityRepository
    {
        private readonly ApplicationDBContext _db;

        public AmenityRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(Amenity amenity)
        {
            _db.Update(amenity);
        }
    }
}
