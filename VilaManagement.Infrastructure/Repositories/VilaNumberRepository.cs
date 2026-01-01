using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;

namespace VilaManagement.Infrastructure.Repositories
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
