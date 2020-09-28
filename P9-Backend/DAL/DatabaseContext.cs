using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.DAL
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("Server=jensoft.dk:27020;Database=p9;Uid=P9ACC;Pwd=p9accpw;")
        {
        }

        public DbSet<Drone> Drones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
