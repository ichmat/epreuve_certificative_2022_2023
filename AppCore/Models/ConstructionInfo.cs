using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCore.Property;
using Microsoft.EntityFrameworkCore;

namespace AppCore.Models
{
    [PrimaryKey(nameof(ConsInfoId), nameof(Type))]
    public class ConstructionInfo
    {
        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        public int VieMax { get; set; }

        [Required]
        public byte NiveauMax { get; set; }

        [Required]
        public long TempsSecConstruction { get; set; }
    }
}
