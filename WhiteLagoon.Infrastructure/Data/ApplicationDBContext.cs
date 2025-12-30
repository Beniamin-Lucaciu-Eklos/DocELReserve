using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Infrastructure.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Vila> Vilas { get; set; }

        public DbSet<VilaNumber> VilaNumbers { get; set; }

        public DbSet<Amenity> Amenities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VilaNumber>().HasData(
                new VilaNumber
                {
                    Vila_Number = 101,
                    VilaId = 1
                },
                new VilaNumber
                {
                    Vila_Number = 102,
                    VilaId = 1
                },
                new VilaNumber
                {
                    Vila_Number = 103,
                    VilaId = 1
                }
                );
        }
    }
}
