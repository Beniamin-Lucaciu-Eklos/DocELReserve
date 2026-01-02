using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Infrastructure.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Vila> Vilas { get; set; }

        public DbSet<VilaNumber> VilaNumbers { get; set; }

        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new VilaConfiguration());

            modelBuilder.ApplyConfiguration(new VilaNumberConfiguration());

            modelBuilder.ApplyConfiguration(new AmenityConfiguration());
        }
    }
}
