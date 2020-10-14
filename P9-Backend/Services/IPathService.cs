using Microsoft.EntityFrameworkCore.Migrations.Operations;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public interface IPathService
    {
        public abstract List<DroneZone> ComputeOptimalPaths(string name, List<Coordinate> coords);
        public abstract List<DroneZone> GetOptimalPath(string name);
    }
}
