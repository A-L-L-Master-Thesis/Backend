using P9_Backend.Models;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Smoothing;

namespace P9_Backend.Services
{
    public class AreaDecomposition : IAreaDecomposition
    {
        public List<DroneZone> ComputeSubAreas(List<Coordinate> searchArea)
        {
            Polygon polygon = new Polygon();

            foreach (Coordinate coordinate in searchArea)
                polygon.Add(new Vertex(coordinate.Latitude, coordinate.Longitude));

            var options = new ConstraintOptions() { ConformingDelaunay = true };
            var quality = new QualityOptions() { MinimumAngle = 25, MaximumArea = 0};

            var mesh = polygon.Triangulate(options, quality);
            var smoother = new SimpleSmoother();
            smoother.Smooth(mesh);
            smoother.Smooth(mesh);
            mesh.Refine(quality, true);

            List<DroneZone> zones = new List<DroneZone>();
            foreach (var triangle in mesh.Triangles)
            {
                DroneZone zone = new DroneZone();
                zone.Area = new List<Coordinate>();
                zone.Path = new List<Coordinate>();
                zone.DroneUUID = "test";
                for (int i = 0; i < 3; i++)
                {
                    Vertex vertex = triangle.GetVertex(i);
                    zone.Area.Add(new Coordinate(vertex.X, vertex.Y));
                }
                zones.Add(zone);
            }

            return zones;
        }
    }
}
