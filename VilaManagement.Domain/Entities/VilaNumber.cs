using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace VilaManagement.Domain.Entities
{
    public class VilaNumber
    {
        public int Vila_Number { get; set; }

        public int VilaId { get; set; }
        public Vila Vila { get; set; }

        public string? SpecialDetails { get; set; }
    }

    public class VilaNumberConfiguration : IEntityTypeConfiguration<VilaNumber>
    {
        public void Configure(EntityTypeBuilder<VilaNumber> builder)
        {
            builder.HasKey(v => v.Vila_Number);
            builder.Property(v => v.Vila_Number).ValueGeneratedNever();
            builder.Property(v => v.SpecialDetails);
            builder.HasOne(v => v.Vila)
                      .WithMany()
                      .HasForeignKey(v => v.VilaId);

            builder.HasData(
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
