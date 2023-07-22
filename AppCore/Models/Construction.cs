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
    [PrimaryKey(nameof(ConsInfoId), nameof(Type), nameof(VillageId), nameof(ConstructionId))]
    public abstract class Construction
    {
        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        public int VillageId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConstructionId { get; set; }

        [Required]
        public int Vie { get; set; }

        [Required]
        public byte Niveau { get; set; }

        [ForeignKey("VillageId")]
        public Village Village { get; set; }

        [ForeignKey("ConsInfoId,Type")]
        public ConstructionInfo ConstructionInfo { get; set; }
    }
}
