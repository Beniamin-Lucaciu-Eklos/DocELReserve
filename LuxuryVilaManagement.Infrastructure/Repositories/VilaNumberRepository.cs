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
    public class VilaNumberRepository : Repository<VilaNumber>, IVilaNumberRepository
    {
        private readonly ApplicationDBContext _db;

        public VilaNumberRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }
    }
}
