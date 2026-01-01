using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxuryVilaManagement.Domain.Entities
{
    public class VilaNumber
    {
        public int Vila_Number { get; set; } 

        public int VilaId { get; set; }
        public Vila Vila { get; set; }

        public string? SpecialDetails { get; set; }
    }
}
