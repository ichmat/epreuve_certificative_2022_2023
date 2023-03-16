using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class Utilisateur
    {
        [Key]
        public Guid UtilisateurId { get; set; }

        [Required]
        [StringLength(100)]
        public string Mail { get; set; }

        [Required]
        [StringLength(100)]
        public string Pseudo { get; set; }

        [Required]
        [StringLength(100)]
        public string MotDePasse { get; set; }

        public float? PoidKg { get; set; }

        public ushort? TailleCm { get; set; }
    }
}
