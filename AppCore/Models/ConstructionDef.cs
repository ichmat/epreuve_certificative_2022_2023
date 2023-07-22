using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class ConstructionDef : Construction
    {
        [Required]
        public int Puissance { get; set; }

        [Required]
        public float MultParNiveau { get; set; }
    }
}
