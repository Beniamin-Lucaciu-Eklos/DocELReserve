using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;

namespace VilaManagement.Infrastructure.Repositories
{
    public class VilaRepository : Repository<Vila>, IVilaRepository
    {
        private readonly ApplicationDBContext _db;

        public VilaRepository(ApplicationDBContext db)
            : base(db)
        {
            _db = db;
        }
    }
}
