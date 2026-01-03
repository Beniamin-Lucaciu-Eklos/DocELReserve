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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDBContext _db;

        public ApplicationUserRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }
    }
}
