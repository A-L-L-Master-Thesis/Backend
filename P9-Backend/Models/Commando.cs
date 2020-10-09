using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Commando
    {
        [JsonProperty]
        public string command { get; set; }
        [JsonProperty]
        public dynamic data { get; set; }

        public override string ToString()
        {
            return $"Command: {command} - Data: {data}";
        }
    }
}
