﻿using AppCore.Property;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppCore.Models
{
    [PrimaryKey(nameof(VillageId), nameof(ConsInfoId), nameof(Type), nameof(ConstructionId))]
    public class Coordonnee
    {
        public int VillageId { get; set; }

        public int ConsInfoId { get; set; }

        public TypeConstruction Type { get; set; }

        public int ConstructionId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        [ForeignKey("ConsInfoId,Type")]
        public ConstructionInfo ConstructionInfo { get; set; }

        [ForeignKey("VillageId")]
        public Village Village { get; set; }

        [JsonIgnore]
        [ForeignKey("ConsInfoId,Type,VillageId,ConstructionId")]
        public Construction Construction { get; set; }
    }
}
