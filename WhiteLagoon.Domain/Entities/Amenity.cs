using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhiteLagoon.Domain.Entities
{
    public class Amenity
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(Vila))]
        public int VilaId { get; set; }
        public Vila Vila { get; set; }
    }
}
