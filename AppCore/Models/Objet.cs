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
        public TypeRarete Rarete { get; set; }

        public static bool operator ==(Objet? lhs, Objet? rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Objet? lhs, Objet? rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return false;
            }

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

            return Equals(obj as Objet);
        }

        public bool Equals(Objet? other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return ObjetId.Equals(other.ObjetId);
        }

        public override int GetHashCode()
        {
            return ObjetId.GetHashCode();
        }
    }
}
