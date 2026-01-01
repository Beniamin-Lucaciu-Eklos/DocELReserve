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

            modelBuilder.Entity<Vila>(vila =>
            {
                vila.HasKey(v => v.Id);
                vila.Property(v => v.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                vila.Property(v => v.Description);
                vila.Property(v => v.Occupancy);
                vila.Property(v => v.Sqft);
                vila.Ignore(x => x.Image);
                vila.Property(v => v.ImageUrl);

                vila.HasMany(v => v.Amenities)
                    .WithOne(a => a.Vila)
                    .HasForeignKey(f => f.VilaId)
                    .IsRequired();
            });

            modelBuilder.Entity<VilaNumber>(vilaNumber =>
            {
                vilaNumber.HasKey(v => v.Vila_Number);
                vilaNumber.Property(v => v.Vila_Number).ValueGeneratedNever();
                vilaNumber.Property(v => v.SpecialDetails);
                vilaNumber.HasOne(v => v.Vila)
                          .WithMany()
                          .HasForeignKey(v => v.VilaId);

            });

            modelBuilder.Entity<Amenity>(amenity =>
            {
                amenity.HasKey(v => v.Id);
                amenity.Property(v => v.Name).IsRequired();
                amenity.Property(v => v.Description);

                amenity.HasOne(a => a.Vila)
                       .WithMany(v => v.Amenities)
                       .HasForeignKey(f => f.VilaId)
                       .IsRequired();
            });


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
