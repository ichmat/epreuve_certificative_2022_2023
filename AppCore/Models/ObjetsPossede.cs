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
    [PrimaryKey( nameof(VillageId), nameof(ObjetId))]
    public class ObjetsPossede
    {
        public int VillageId { get; set; }

        public int ObjetId { get; set; }

        [ForeignKey("ObjetId")]
        public Objet Objet { get; set; }

        [ForeignKey("VillageId")]
        public Village Village { get; set; }

        [Required]
        public int Nombre { get; set; }
    }
}
