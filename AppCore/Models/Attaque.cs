using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class Attaque
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttaqueId { get; set; }

        [Required]
        public DateTime DateApparition { get; set; }

        [Required]
        public int Puissance { get; set; }

        [Required]
        public int VillageId { get; set; }

        [ForeignKey("VillageId")]
        public Village Village { get; set;}
    }
}
