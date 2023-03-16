using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    public class Ressource
    {
        [Key]
        public int RessourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }
    }
}
