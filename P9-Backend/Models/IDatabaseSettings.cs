using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Models
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
    }
}
