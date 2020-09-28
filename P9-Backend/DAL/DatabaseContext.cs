using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace P9_Backend.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Drone> Drones { get; set; }
    }
}
