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
    [PrimaryKey(nameof(ObjetId), nameof(ConsInfoId), nameof(Type))]
    public class CreationObjet
    {
        public int ObjetId { get; set; }

        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        [ForeignKey("ObjetId")]
        public Objet Objet { get; set; }

        [ForeignKey("ConsInfoId,Type")]
        public ConstructionInfo ConstructionInfo { get; set; }

        [Required]
        public int Nombre { get; set; }
    }
}
