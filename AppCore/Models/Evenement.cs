using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCore.Property;

namespace AppCore.Models
{
    public class Evenement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EvenId { get; set; }

        [Required]
        public TypeEvent EvenType { get; set; }

        [Required]
        public DateTime ExprirationTime { get; set; }

        [Required]
        public int EvenLieeId { get; set; }

        public string EvenInfo { get; set; }
    }
}
