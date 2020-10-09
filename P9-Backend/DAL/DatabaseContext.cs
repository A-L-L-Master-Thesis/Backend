using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace P9_Backend.DAL
{
    public class DatabaseContext : DbContext, IDatabaseContext, IServiceScopeFactory
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Drone> Drones { get; set; }

        public IServiceScope CreateScope()
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
