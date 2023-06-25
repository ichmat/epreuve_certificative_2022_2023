using AppCore.Property;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Models
{
    [PrimaryKey(nameof(RessourceId), nameof(ConsInfoId), nameof(Type), nameof(NiveauConcerne))]
    public class AmeliorationRessource
    {
        public int RessourceId { get; set; }

        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        public byte NiveauConcerne { get; set; }

        [ForeignKey("RessourceId")]
        public Ressource Ressource { get; set; }

        [ForeignKey("ConsInfoId,Type")]
        public ConstructionInfo ConstructionInfo { get; set; }

        [Required]
        public int Nombre { get; set; }
    }
}
