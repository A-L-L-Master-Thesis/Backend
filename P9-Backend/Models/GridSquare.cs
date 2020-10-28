using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public class GridSquare
    {
        public GridSquare(Coordinate centerCoordinate, List<Coordinate> corners)
        {
            CenterCoordinate = centerCoordinate;
            Corners = corners;
        }

        public Coordinate CenterCoordinate { get; set; }
        public List<Coordinate> Corners { get; set; }
    }
}
