using Microsoft.EntityFrameworkCore;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.DAL
{
    public interface IDatabaseContext
    {
        public abstract DbSet<Drone> Drones { get; set; }
    }
}
