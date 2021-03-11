using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public class DemoDrone
    {
        public Drone DroneObj { get; set; }
        public int CurrentStep { get; set; } = 0;
        public Queue<Tuple<double, double>> Linecoords { get; set; } = new Queue<Tuple<double, double>>();
        public bool Reverse { get; set; } = false;
        public bool Paused { get; set; } = false; 

        public DemoDrone(Drone drone)
        {
            DroneObj = drone;
        }

        public void ResetDemoDrone()
        {
            CurrentStep = 0;
            Reverse = false;
            Paused = false;
            Linecoords.Clear();
        }
    }
}
