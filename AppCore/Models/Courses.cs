using AppCore.Property;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Models
{
    public class Courses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CoursesId { get; set; }

        [Required]
        public float VitesseMoyenKmH { get; set; }

        [Required]
        public float DistanceKm { get; set; }

        [Required]
        public long TempsSec { get; set; }

        [Required]
        public DifficulteCourse NiveauDifficulte { get; set; }

        [Required]
        public DateTime DateDebut { get; set; }

        [Required]
        public int UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }
    }
}