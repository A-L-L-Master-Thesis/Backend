using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P9_Backend.Models
{
    public class Position
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key, ForeignKey("Drone")]
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public int Bearing { get; set; }
    }
}
