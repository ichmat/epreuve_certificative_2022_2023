using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{ 
    [PrimaryKey( nameof(VillageId), nameof(RessourceId))]
    public class RessourcePossede
    {
        public int VillageId { get; set; }
        public int RessourceId { get; set; }

        [ForeignKey("VillageId")]
        public Village Village { get; set; }

        [ForeignKey("RessourceId")]
        public Ressource Ressource { get; set; }

        [Required]
        public int Nombre { get; set; }
    }
}
