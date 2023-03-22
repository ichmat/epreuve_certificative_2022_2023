using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class ConstructionProd : Construction
    {
        [Required]
        public int Production { get; set; }

        [Required]
        public float MultParNiveau { get; set; }

        [Required]
        public int RessourceId { get; set; }

        [ForeignKey("RessourceId")]
        public Ressource Ressource { get; set; }
    }
}
