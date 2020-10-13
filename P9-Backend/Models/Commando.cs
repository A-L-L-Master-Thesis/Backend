using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Commando : IEquatable<Commando>
    {
        [JsonProperty]
        public string command { get; set; }
        [JsonProperty]
        public dynamic data { get; set; }

        public bool Equals([AllowNull] Commando other)
        {
            return (this.command == other.command &&
                    this.data == other.data);
        }

        public override string ToString()
        {
            return $"Command: {command} - Data: {data}";
        }
    }
}
