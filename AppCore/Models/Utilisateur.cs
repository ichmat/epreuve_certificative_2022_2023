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
        [StringLength(255)]
        public string Mail { get; set; }

        [Required]
        [StringLength(255)]
        public string Pseudo { get; set; }

        [Required]
        [StringLength(255)]
        public string MotDePasse { get; set; }

        [Required]
        [StringLength(255)]
        public string Sel { get; set; }

        public float? PoidKg { get; set; }

        public ushort? TailleCm { get; set; }
    }
}
