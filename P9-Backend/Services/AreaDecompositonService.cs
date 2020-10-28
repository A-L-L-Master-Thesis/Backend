using Microsoft.AspNetCore.Razor.Language.Intermediate;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Smoothing;

namespace P9_Backend.Services
{
    public class AreaDecompositonService : IAreaDecompositionService
    {

        private readonly IDroneService _droneService;

        public AreaDecompositonService(IDroneService droneService)
        {
            _droneService = droneService;
        }


        public List<DroneZone> ComputeSubAreas(List<Coordinate> searchArea)
        {
            // Zone area paramters
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            foreach (Coordinate coordinate in searchArea)
            {
                if (coordinate.Latitude < minX)
                    minX = coordinate.Latitude;

                if (coordinate.Latitude > maxX)
                    maxX = coordinate.Latitude;

                if (coordinate.Longitude < minY)
                    minY = coordinate.Longitude;

                if (coordinate.Longitude > maxY)
                    maxY = coordinate.Longitude;
            }

            // Calculating grid squares
            List<GridSquare> gridSquares = new List<GridSquare>();

            double[] camera_scope = AddMetersToCoordinates(searchArea[0], 20);

            for (double i = minX; i < maxX; i += camera_scope[0] * 2)
            {
                for (double j = minY; j < maxY; j += camera_scope[1] * 2)
                {
                    Coordinate newCoordinate = new Coordinate(i, j);

                    if (WithinZone(searchArea, newCoordinate)) {
                        GridSquare gridSquare = CalculateGridSquare(newCoordinate, camera_scope);

                        gridSquares.Add(gridSquare);
                    }
                }
            }


            // Assigining grids to drones
            List<DroneZone> droneZones = new List<DroneZone>();

            int NumberOfDrones = _droneService.GetDrones().Result.Value.Count();

            int NumberOfGrids = (int) Math.Floor((double) gridSquares.Count / NumberOfDrones);
            int NumberOfGridsRest = gridSquares.Count % NumberOfDrones;

            for (int i = 0; i < NumberOfDrones; i++)
            {
                DroneZone droneZone = new DroneZone();
                List<Coordinate> squares = new List<Coordinate>();

                List<GridSquare> test = gridSquares.GetRange(i * NumberOfGrids, NumberOfGrids);

                if (i == NumberOfDrones - 1)
                    test.AddRange(gridSquares.GetRange(i * NumberOfGrids + NumberOfGridsRest, NumberOfGridsRest));

                foreach (GridSquare item in test)
                {
                    squares.AddRange(item.Corners);
                }

                Dictionary<Coordinate, int> gridDict = new Dictionary<Coordinate, int>(new CoordinateEqualityComparer());
                // Count number of coordinates
                foreach (Coordinate coordinate in squares)
                {
                    if (!gridDict.ContainsKey(coordinate))
                        gridDict[coordinate] = 1;
                    else
                        gridDict[coordinate] += 1;
                }

                // Discard coordinates
                // If coordinates appear more than 1 time or is different than 3 it should be removed

                /*foreach (KeyValuePair<Coordinate, int> item in gridDict)
                {
                    if (!(item.Value == 1 || item.Value == 3)) {
                        squares.RemoveAll(x => x.Latitude == item.Key.Latitude && x.Longitude == item.Key.Longitude);
                    }
                }*/


                droneZone.Area = squares;
                droneZones.Add(droneZone);
            }


            return droneZones;
        }

        private GridSquare CalculateGridSquare(Coordinate gridCenter, double[] camera_scope)
        {
            List<Coordinate> corners = new List<Coordinate>();

            corners.Add(new Coordinate(gridCenter.Latitude - camera_scope[0], gridCenter.Longitude + camera_scope[1])); // Top left
            corners.Add(new Coordinate(gridCenter.Latitude + camera_scope[0], gridCenter.Longitude + camera_scope[1])); // Top right
            corners.Add(new Coordinate(gridCenter.Latitude + camera_scope[0], gridCenter.Longitude - camera_scope[1])); // Bottom right
            corners.Add(new Coordinate(gridCenter.Latitude - camera_scope[0], gridCenter.Longitude - camera_scope[1])); // Bottom left


            return new GridSquare(gridCenter, corners);
        }

        private bool WithinZone(List<Coordinate> searchArea, Coordinate coordinate)
        {
            bool oddNodes = false;
            for (int k = 0; k < searchArea.Count - 1; k++)
            {
                if (((searchArea[k].Longitude > coordinate.Longitude) != (searchArea[k+1].Longitude > coordinate.Longitude)) &&
                        (coordinate.Latitude < (searchArea[k+1].Latitude - searchArea[k].Latitude) * (coordinate.Longitude - searchArea[k].Longitude) / (searchArea[k+1].Longitude - searchArea[k].Longitude) + searchArea[k].Latitude))
                {
                    oddNodes = !oddNodes;
                }
            }


            if (((searchArea[searchArea.Count - 1].Longitude > coordinate.Longitude) != (searchArea[0].Longitude > coordinate.Longitude)) &&
                        (coordinate.Latitude < (searchArea[0].Latitude - searchArea[searchArea.Count - 1].Latitude) * (coordinate.Longitude - searchArea[searchArea.Count - 1].Longitude) 
                        / (searchArea[0].Longitude - searchArea[searchArea.Count - 1].Longitude) + searchArea[searchArea.Count - 1].Latitude))
            {
                oddNodes = !oddNodes;
            }


            return oddNodes;
        }


        private double[] AddMetersToCoordinates(Coordinate coordinate, int meters)
        {
            double coef = meters * 0.0000089;

            double camera_scope_lat = coef;
            double camera_scope_lng = coef / Math.Cos(coordinate.Latitude * 0.018);

            
            return new[]{ camera_scope_lat, camera_scope_lng };
        }
    }
}
