using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VilaManagement.Domain.Entities
{
    public class Amenity
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string Description { get; set; }

        public int VilaId { get; set; }
        public Vila Vila { get; set; }
    }

    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Name).IsRequired();
            builder.Property(v => v.Description);

            builder.HasOne(a => a.Vila)
                   .WithMany(v => v.Amenities)
                   .HasForeignKey(f => f.VilaId)
                   .IsRequired();
        }
    }
}
