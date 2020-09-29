using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace P9_Backend.Models
{
    public class Position
    {
        [Key]
        public string UUID { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public double Altitude { get; set; }
    }
}
