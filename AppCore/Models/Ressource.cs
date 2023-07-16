using AppCore.Services;
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

        public static bool operator ==(Ressource lhs, Ressource rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Ressource lhs, Ressource rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return Equals(obj as Ressource);
        }

        public bool Equals(Ressource? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return RessourceId.Equals(other.RessourceId);
        }

        public override int GetHashCode()
        {
            return RessourceId.GetHashCode();
        }
    }
}