using Microsoft.EntityFrameworkCore;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P9_Backend_Tests
{
    public class ItemsControllerTest
    {
        protected ItemsControllerTest(DbContextOptions<DbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<DbContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new DbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Position pos = new Position { ID = 1, Altitude = 22, Latitude = 55, Longitude = 56 };
                Drone newDrone = new Drone { UUID= "test", Status= DroneStatus.Charging, BatteryPercentage=33, CurrentPosition=pos, LastUpdate=DateTime.Now, IP="1.1.1.1"};
     
                context.Add(newDrone);
                context.SaveChanges();
            }
        }
    }
}
