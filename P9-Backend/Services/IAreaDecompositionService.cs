using P9_Backend.Models;
using System.Collections.Generic;
using TriangleNet.Topology;

namespace P9_Backend.Services
{
    public interface IAreaDecompositionService
    {
        public List<DroneZone> ComputeSubAreas(List<Coordinate> searchArea);
    }
}
