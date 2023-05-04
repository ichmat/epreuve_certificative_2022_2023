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

        public double? ObjectifTempsSecMax { get; set; }

        public double? ObjectifPauseSecMax { get; set; }

        public double? ObjectifDistanceKm { get; set; }

        public double? ObjectifVitesseMoyenneKmH { get; set; }

        public double? VitesseMoyenneKmH { get; set; }

        public double? TotalDistanceKm { get; set; }

        public uint? TotalCalorieDepenseKCal { get; set; }

        public double? VmaKmH { get; set; }
    }
}
