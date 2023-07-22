using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class Stat
    {
        [JsonInclude]
        [Key]
        public Guid UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }

        [JsonInclude]
        public double? ObjectifTempsSecMax { get; set; }

        [JsonInclude]
        public double? ObjectifPauseSecMax { get; set; }

        [JsonInclude]
        public double? ObjectifDistanceKm { get; set; }

        [JsonInclude]
        public double? ObjectifVitesseMoyenneKmH { get; set; }

        [JsonInclude]
        public double? VitesseMoyenneKmH { get; set; }

        [JsonInclude]
        public double? TotalDistanceKm { get; set; }

        [JsonInclude]
        public uint? TotalCalorieDepenseKCal { get; set; }

        [JsonInclude]
        public double? VmaKmH { get; set; }
    }
}
