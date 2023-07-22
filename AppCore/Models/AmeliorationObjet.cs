using AppCore.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Models
{
    [PrimaryKey(nameof(ObjetId), nameof(ConsInfoId), nameof(Type), nameof(NiveauConcerne))]
    public class AmeliorationObjet
    {
        public int ObjetId { get; set; }

        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        public byte NiveauConcerne { get; set; }

        [ForeignKey("ObjetId")]
        public Objet Objet { get; set; }

        [ForeignKey("ConsInfoId,Type")]
        public ConstructionInfo ConstructionInfo { get; set; }

        [Required]
        public int Nombre { get; set; }
    }
}
