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
        private readonly int _pointsPerLine = 20;
        private readonly double _lineLength = 0.5;
        private List<DemoDrone> _demoDrones = new List<DemoDrone>();
        private readonly int _stepDelay = 2000;
        private readonly double _R = 6371.0; // Earth's Radius

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

        private double[,] _droneBearings =
        {
            {28, 110, 208, 110, 28, 110, 208},
            {208, 110, 28, 110, 208, 110, 28},
            {28, 110, 208, 110, 28, 110, 208},
            {160 ,53, -20, 53, 160, 53, -20},
            {-20, 53, 160, 53, -20, 53, 160},
            {160 ,53, -20, 53, 160, 53, -20},
            {-20, 53, 160, 53, -20, 53, 160},
            {160 ,53, -20, 53, 160, 53, -20}
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
                await Task.Run(() => {
                    List<Drone> dronesList = _droneService.GetDrones().Result.Value.ToList();

                    for (int i = 0; i < dronesList.Count; i++)
                    {
                        dronesList[i].CurrentPosition.Latitude = _startPositions[i].Item1;
                        dronesList[i].CurrentPosition.Longitude = _startPositions[i].Item2;
                    }

                    foreach (var drone in dronesList)
                    {
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
                if(!_cancellationToken.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();

                    await _demoTask;

                    _demoTask = null;

                    return true;
                }
                else
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
                    // Advance the drone by one step
                    AdvanceDrone(drone, i++);
                }

                Thread.Sleep(_stepDelay);
            }
        }

        private void AdvanceDrone(DemoDrone drone, int droneIDX)
        {
            if(drone.Linecoords.Count == 0)
            {
                //Calculate the next line of points
                drone.Linecoords = 
                    GetNextPathLine(drone.DroneObj.CurrentPosition.Latitude, drone.DroneObj.CurrentPosition.Longitude, _droneBearings[droneIDX, drone.CurrentStep++], _lineLength);
                if(drone.CurrentStep >= 8)
                {
                    drone.CurrentStep = 0;
                }
            }
            var nextCoords = drone.Linecoords.Dequeue();

            //Set Coords to the next coords on the path
            drone.DroneObj.CurrentPosition.Latitude = nextCoords.Item1;
            drone.DroneObj.CurrentPosition.Longitude = nextCoords.Item2;

            //Update the drone in the DB
            _droneService.UpdateDrone(drone.DroneObj.UUID, drone.DroneObj);
        }

        // Coordinate calculations from: http://www.movable-type.co.uk/scripts/latlong.html#destPoint
        private Tuple<double, double> GetNextCoordinate(double φ1, double λ1, double θ, double d)
        {
            System.Diagnostics.Debug.WriteLine("1: " + φ1 + " " + λ1);
            θ = ToRad(θ);
            φ1 = ToRad(φ1);
            λ1 = ToRad(λ1);

            double φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(d / _R) + Math.Cos(φ1) * Math.Sin(d / _R) * Math.Cos(θ));
            double λ2 = λ1 + Math.Atan2(Math.Sin(θ) * Math.Sin(d / _R) * Math.Cos(φ1), Math.Cos(d / _R) - Math.Sin(φ1) * Math.Sin(φ2));

            System.Diagnostics.Debug.WriteLine("2: " + ToDeg(φ2) + " " + ToDeg(λ2));

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
            System.Diagnostics.Debug.WriteLine(φ1 + " " + λ1);
            double stepD = d / _pointsPerLine;
            Queue<Tuple<double, double>> resList = new Queue<Tuple<double, double>>();

            for (int i = 0; i < _pointsPerLine; i++)
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
