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
        private Queue<Tuple<double, double>> _lineCoords = new Queue<Tuple<double, double>>();
        public Queue<Tuple<double, double>> Linecoords { get { return _lineCoords; } set { _lineCoords = value; } }

        public DemoDrone(Drone drone)
        {
            DroneObj = drone;
        }

        public void ResetDemoDrone()
        {
            CurrentStep = 0;
            _lineCoords.Clear();
        }
    }
}
