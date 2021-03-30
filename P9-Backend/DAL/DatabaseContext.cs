using P9_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace P9_Backend.DAL
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<Drone> Drones { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<LogEntry> Logs { get; set; }
        public DbSet<Boat> Boats { get; set; }
    }
}
