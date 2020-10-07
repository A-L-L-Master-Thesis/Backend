using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public class Commando
    {
        public string command { get; set; }
        public string data { get; set; }

        public override string ToString()
        {
            return $"Command: {command} - Data: {data}";
        }
    }
}
