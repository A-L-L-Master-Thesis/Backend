using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public enum DroneStatus { Charging, Launching, Landing, Returning, Searching, Following}
    public class Drone
    {
        public string UUID { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public Position CurrentPosition { get; private set; }
        public int BatteryPercentage { get; private set; }
        public DroneStatus Status { get; private set; }
        public string IP { get; private set; }

        public Drone(string uuid, string ip)
        {
            UUID = uuid;
            IP = ip;
        }
    }
}
