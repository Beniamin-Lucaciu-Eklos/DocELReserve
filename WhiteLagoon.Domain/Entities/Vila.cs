using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entities
{
    public class Vila
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Range(10000, 20000000)]
        public double Price { get; set; }

        public int Sqft { get; set; }

        [Range(1,10)]
        public int Occupancy { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
