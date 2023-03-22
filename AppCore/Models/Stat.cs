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
        public Guid UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }

        public float? VitesseMoyenneKmH { get; set; }

        public int? TotalCalorieDepenseKCal { get; set; }

        public float? VmaKmH { get; set; }
    }
}
