using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxuryVilaManagement.Application.Common.Interfaces;
using LuxuryVilaManagement.Infrastructure.Data;

namespace LuxuryVilaManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _db;

        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Villa = new VilaRepository(db);
            VilaNumber = new VilaNumberRepository(db);
            Amenity = new AmenityRepository(db);
        }

        public IVilaRepository Villa { get; private set; }

        public IVilaNumberRepository VilaNumber { get; private set; }

        public IAmenityRepository Amenity { get; private set; }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
