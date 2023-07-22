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
    [PrimaryKey(nameof(RessourceId), nameof(ConsInfoId), nameof(Type))]
    public class CreationRessource
    {
        public int RessourceId { get; set; }

        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        [ForeignKey("RessourceId")]
        public Ressource Ressource { get; set; }

        [ForeignKey("ConsInfoId,Type")]
        public ConstructionInfo ConstructionInfo { get; set; }

        [Required]
        public int Nombre { get; set; }
    }
}
