using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class Vila
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        [Range(10000, 20000000)]
        public double Price { get; set; }

        public int Sqft { get; set; }

        [Range(1, 10)]
        public int Occupancy { get; set; }

        public IFormFile? Image { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [ValidateNever]
        public IEnumerable<Amenity> Amenities { get; set; } = new List<Amenity>();

        public bool IsAvailable { get; set; } = true;
    }


    public class VilaConfiguration : IEntityTypeConfiguration<Vila>
    {
        public void Configure(EntityTypeBuilder<Vila> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Description);
            builder.Property(v => v.Occupancy);
            builder.Property(v => v.Sqft);
            builder.Ignore(x => x.Image);
            builder.Property(v => v.ImageUrl);

            builder.HasMany(v => v.Amenities)
                .WithOne(a => a.Vila)
                .HasForeignKey(f => f.VilaId)
                .IsRequired();

            builder.Ignore(v => v.IsAvailable);
        }
    }
}
