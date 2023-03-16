using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class Stat
    {
        [Key]
        [ForeignKey("Utilisateur")]
        public Guid UtilisateurId { get; set; }

        public float? VitesseMoyenneKmH { get; set; }

        public int? TotalCalorieDepenseKCal { get; set; }

        public float? VmaKmH { get; set; }
    }
}
