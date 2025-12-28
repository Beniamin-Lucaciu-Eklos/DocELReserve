using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _db;

        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Villa = new VilaRepository(db);
            VilaNumber = new VilaNumberRepository(db);
        }

        public IVilaRepository Villa { get; private set; }

        public IVilaNumberRepository VilaNumber { get; private set; }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
