using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace P9_Backend.Models
{
    public class Coordinate 
    {
        public Coordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Coordinate() { }

        [JsonPropertyName("lat")]
        public double Latitude { get; set; }
        [JsonPropertyName("lng")]
        public double Longitude { get; set; }
    }

    public class CoordinateEqualityComparer : IEqualityComparer<Coordinate>
    {
        public bool Equals(Coordinate a, Coordinate b)
        {
            return (a.Latitude == b.Latitude) && (a.Longitude == b.Longitude);
        }

        public int GetHashCode([DisallowNull] Coordinate obj)
        {
            return $"{obj.Latitude}|{obj.Longitude}".GetHashCode();
        }
    }
}