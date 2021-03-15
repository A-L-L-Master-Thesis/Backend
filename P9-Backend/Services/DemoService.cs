using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public class DemoService : IDemoService
    {
        private readonly IDroneService _droneService;
        private List<DemoDrone> _demoDrones = new List<DemoDrone>();
        private readonly int _stepDelay = 2000; // Milliseconds
        private readonly double _R = 6371.0; // Earth's Radius
        private readonly double _droneSpeed = 8; // M/s.

        private List<Tuple<double, double>> _startPositions = new List<Tuple<double, double>>
        {
            new Tuple<double, double>(57.05258711442612, 9.919553706795027),
            new Tuple<double, double>(57.05486375316075, 9.927745536938286),
            new Tuple<double, double>(57.049760748008374, 9.9279981484405),
            new Tuple<double, double>(57.053548815057596, 9.935829105009168),
            new Tuple<double, double>(57.049682234759416, 9.942216567279463),
            new Tuple<double, double>(57.05474599940804, 9.942541353496596),
            new Tuple<double, double>(57.05209644124299, 9.950300135350345),
            new Tuple<double, double>(57.05692438338328, 9.949830999703373),
        };

        private double[,,] _droneBearings =
        {
            {{28,374}, {110, 52}, {208, 374}, {110, 52}, {28,374}, {110, 52}, {208, 374}},
            {{208, 494}, {110, 52}, {28, 494}, {110, 52}, {208, 494}, {110, 52}, {28, 494}},
            {{28, 534}, {110, 52}, {208, 534}, {110, 52}, {28, 534}, {110, 52}, {208, 534}},
            {{160, 508} , {53, 52}, {-20, 508}, {53, 52}, {160, 508}, {53, 52}, {-20, 508}},
            {{-20, 515}, {53, 52}, {160, 515}, {53, 52}, {-20, 515}, {53, 52}, {160, 515}},
            {{160, 515} ,{53, 52}, {-20, 515}, {53, 52}, {160, 515}, {53, 52}, {-20, 515}},
            {{-20, 508}, {53, 52}, {160, 508}, {53, 52}, {-20, 508}, {53, 52}, {160, 508}},
            {{160, 434} ,{53, 52}, {-20, 434}, {53, 52}, {160, 434}, {53, 52}, {-20, 434}}
        };

        private Task _demoTask = null;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        public DemoService(IDroneService droneService)
        {
            _droneService = droneService;
            _cancellationToken = _cancellationTokenSource.Token;
            List<Drone> dronesList = _droneService.GetDrones().Result.Value.ToList();

            for (int i = 0; i < 8; i++)
            {
                _demoDrones.Add(new DemoDrone(dronesList[i]));
            }
        }

        /// <summary>
        /// Resets the drones and resets the demo
        /// </summary>
        /// <returns>Bool describing the success</returns>
        public async Task<ActionResult<bool>> ResetDemo()
        {
            try
            {
                if (_demoTask != null)
                    return false;

                await Task.Run(() => {
                    List<Drone> dronesList = _droneService.GetDrones().Result.Value.ToList();
                    ChangeDroneStatusAll(DroneStatus.Charging);

                    for (int i = 0; i < dronesList.Count; i++)
                    {
                        dronesList[i].CurrentPosition.Latitude = _startPositions[i].Item1;
                        dronesList[i].CurrentPosition.Longitude = _startPositions[i].Item2;
                    }

                    foreach (var drone in dronesList)
                    {
                        drone.Status = DroneStatus.Charging;
                        _droneService.UpdateDrone(drone.UUID, drone);
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        _demoDrones[i].DroneObj = dronesList[i];
                        _demoDrones[i].ResetDemoDrone();
                    }
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Starts the demo
        /// </summary>
        /// <returns>Bool describing the success</returns>
        public async Task<ActionResult<bool>> StartDemo()
        {
            return await Task.Run(() => {
                if (_demoTask == null)
                {
                    ChangeDroneStatusAll(DroneStatus.Searching);
                    _demoTask = Task.Factory.StartNew(() =>
                    {
                        DemoLoop();
                    }, _cancellationToken);

                    return true;
                }
                else
                    return false;
            });  
        }

        /// <summary>
        /// Stops the demo
        /// </summary>
        /// <returns>Bool describing the success</returns>
        public async Task<ActionResult<bool>> StopDemo()
        {
            return await Task.Run(async () => {
                if(!_cancellationToken.IsCancellationRequested && _demoTask != null)
                {
                    _cancellationTokenSource.Cancel();

                    await _demoTask;

                    _demoTask = null;
                    _cancellationTokenSource = new CancellationTokenSource();
                    _cancellationToken = _cancellationTokenSource.Token;

                    ChangeDroneStatusAll(DroneStatus.Idle);

                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public async Task<ActionResult<bool>> PauseDrone(string id, bool pause)
        {
            return await Task.Run(() =>
            {
                if (_demoTask == null)
                    return false;

                try
                {
                    _demoDrones.Find(x => x.DroneObj.UUID == id).Paused = pause;
                    return true;
                }
                catch (NullReferenceException)
                {
                    return false;
                }
            });
        }

        private void DemoLoop()
        {
            //Continue looping untill a cancel is requested
            while(!_cancellationToken.IsCancellationRequested)
            {
                int i = 0;
                foreach (DemoDrone drone in _demoDrones)
                {
                    //Skip if paused
                    if (drone.Paused)
                    {
                        i++;
                        continue;
                    }

                    // Advance the drone by one step
                    AdvanceDrone(drone, i++);
                }

                Thread.Sleep(_stepDelay);
            }
        }

        private void ChangeDroneStatusAll(DroneStatus status)
        {
            foreach (DemoDrone drone in _demoDrones)
            {
                drone.DroneObj.Status = status;

                //Update the drone in the DB
                _droneService.UpdateDrone(drone.DroneObj.UUID, drone.DroneObj);
            }
        }

        /// <summary>
        /// Advances the Drones by one step
        /// </summary>
        /// <param name="drone">The DemoDrone to advance</param>
        /// <param name="droneIDX">The index of the DemoDrone</param>
        private void AdvanceDrone(DemoDrone drone, int droneIDX)
        {
            if (drone.Linecoords.Count == 0)
            {
                double nextBearing = drone.Reverse ? _droneBearings[droneIDX, drone.CurrentStep, 0] + 180 : _droneBearings[droneIDX, drone.CurrentStep, 0];

                //Calculate the next line of points
                drone.Linecoords = 
                    GetNextPathLine(drone.DroneObj.CurrentPosition.Latitude, drone.DroneObj.CurrentPosition.Longitude,
                    nextBearing, _droneBearings[droneIDX, drone.CurrentStep, 1]);

                if (drone.Reverse)
                    drone.CurrentStep--;
                else
                    drone.CurrentStep++;

                if (drone.CurrentStep >= 7)
                {
                    drone.CurrentStep = 6;
                    drone.Reverse = true;
                }
                else if (drone.CurrentStep < 0)
                {
                    drone.CurrentStep = 0;
                    drone.Reverse = false;
                }
            }
            var nextCoords = drone.Linecoords.Dequeue();

            int stepIDX;
            int bearing;

            //This if tree is necesarry to align the direction-cones correctly in the front-end
            if (drone.CurrentStep == 0 && !drone.Reverse)
            {
                stepIDX = 6;
                bearing = (int)(_droneBearings[droneIDX, stepIDX, 0]);
            }
            else if (drone.CurrentStep == 6 && drone.Reverse)
            {
                stepIDX = 6;
                bearing = (int)(_droneBearings[droneIDX, stepIDX, 0]);
            }
            else
            {
                stepIDX = drone.Reverse ? drone.CurrentStep + 1 : drone.CurrentStep - 1;
                bearing = (int)(drone.Reverse ? _droneBearings[droneIDX, stepIDX, 0] + 180 : _droneBearings[droneIDX, stepIDX, 0]);
            }

            //Set Coords to the next coords on the path
            drone.DroneObj.CurrentPosition.Latitude = nextCoords.Item1;
            drone.DroneObj.CurrentPosition.Longitude = nextCoords.Item2;
            drone.DroneObj.CurrentPosition.Bearing = bearing;

            //Update the drone in the DB
            _droneService.UpdateDrone(drone.DroneObj.UUID, drone.DroneObj);
        }

        // Coordinate calculations from: http://www.movable-type.co.uk/scripts/latlong.html#destPoint
        private Tuple<double, double> GetNextCoordinate(double φ1, double λ1, double θ, double d)
        {
            θ = ToRad(θ);
            φ1 = ToRad(φ1);
            λ1 = ToRad(λ1);

            double φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(d / _R) + Math.Cos(φ1) * Math.Sin(d / _R) * Math.Cos(θ));
            double λ2 = λ1 + Math.Atan2(Math.Sin(θ) * Math.Sin(d / _R) * Math.Cos(φ1), Math.Cos(d / _R) - Math.Sin(φ1) * Math.Sin(φ2));

            return new Tuple<double, double>(ToDeg(φ2), ToDeg(λ2));
        }

        private double ToRad(double val)
        {
            return (Math.PI / 180) * val;
        }

        private double ToDeg(double val)
        {
            return (180 / Math.PI) * val;
        }

        /// <summary>
        /// Generates a new line of points with a given bearing and length
        /// </summary>
        /// <param name="φ1">Start latitude</param>
        /// <param name="λ1">Start Longitude</param>
        /// <param name="θ">Bearing (degrees)</param>
        /// <param name="d">Distance</param>
        /// <returns>A Queue with tuples of doubles</returns>
        private Queue<Tuple<double, double>> GetNextPathLine(double φ1, double λ1, double θ, double d)
        {
            int pointsPerLine = (int)(d / (_droneSpeed * (_stepDelay/1000)));
            double stepD = (d / pointsPerLine) / 1000;
            Queue<Tuple<double, double>> resList = new Queue<Tuple<double, double>>();

            for (int i = 0; i < pointsPerLine; i++)
            {
                if(i == 0)
                {
                    resList.Enqueue(GetNextCoordinate(φ1, λ1, θ, stepD));
                }
                else
                {
                    resList.Enqueue(GetNextCoordinate(resList.Last().Item1, resList.Last().Item2, θ, stepD));
                }
            }

            return resList;
        }
    }
}
