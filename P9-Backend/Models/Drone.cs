using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public enum DroneStatus { Idle, Charging, Launching, Landing, Returning, Searching, Following, Error = 440}
    public class Drone
    {
        [Key]
        public string UUID { get; set; }
        public DateTime LastUpdate { get; set; }
        public virtual Position CurrentPosition { get; set; }
        public int BatteryPercentage { get; set; }
        public DroneStatus Status { get; set; }
        public string IP { get; set; }
    }
}
