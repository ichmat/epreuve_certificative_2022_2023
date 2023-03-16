using AppCore.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class Objet
    {
        [Key]
        public int ObjetId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        public byte rarete { get; set; }

        public TypeRarete Rarete { get; set; }
    }
}
