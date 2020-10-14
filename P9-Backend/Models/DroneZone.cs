using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public class DroneZone
    {
        public string DroneUUID { get; set; }
        public string ZoneColor { get; set; }
        public List<Coordinate> Area { get; set; }
        public List<Coordinate> Path { get; set; }
    }
}
