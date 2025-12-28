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
    public class VilaNumberRepository : Repository<VilaNumber>, IVilaNumberRepository
    {
        private readonly ApplicationDBContext _db;

        public VilaNumberRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }

        public void Update(VilaNumber vilaNumber)
        {
            _db.Update(vilaNumber);
        }
    }
}
