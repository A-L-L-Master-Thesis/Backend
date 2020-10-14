using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P9_Backend.Models
{
    public class Position : Coordinate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key, ForeignKey("Drone")]
        public int ID { get; set; }
        public double Altitude { get; set; }
    }
}
