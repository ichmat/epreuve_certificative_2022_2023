using System.ComponentModel.DataAnnotations;

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