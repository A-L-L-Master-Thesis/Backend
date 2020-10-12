using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P9_Backend.DAL;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public enum QueryResult { OK, NotFoundError, ConcurrencyError, ConflictError, UpdateError}
    public class DroneService : IDroneService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DroneService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<QueryResult> DeleteDrone(string uuid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
                var drone = await dbContext.Drones.Include(d => d.CurrentPosition).FirstOrDefaultAsync(d => d.UUID == uuid);
                var pos = drone.CurrentPosition;
                if (drone == null)
                {
                    return QueryResult.NotFoundError;
                }

                dbContext.Drones.Remove(drone);
                dbContext.Position.Remove(pos);
                
                await dbContext.SaveChangesAsync();

                return QueryResult.OK;
            }
        }

        public async Task<ActionResult<Drone>> GetDrone(string uuid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var drones = dbContext.Drones.Include(d => d.CurrentPosition);
                var drone = await drones.FirstOrDefaultAsync(d => d.UUID == uuid);

                return drone;
            }
        }

        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                return await dbContext.Drones.Include(d => d.CurrentPosition).ToListAsync();
            }
        }

        public async Task<QueryResult> RegisterDrone(Drone drone)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Drones.Add(drone);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (DroneExists(drone.UUID))
                    {
                        return QueryResult.ConflictError;
                    }
                    else
                    {
                        return QueryResult.UpdateError;
                    }
                }

                return QueryResult.OK;
            }
        }

        public async Task<QueryResult> UpdateDrone(string uuid, Drone drone)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var dbDrone = await dbContext.Drones.Include(d => d.CurrentPosition).FirstOrDefaultAsync(d => d.UUID == uuid);
                drone.CurrentPosition.ID = dbDrone.CurrentPosition.ID;

                dbContext.Entry(dbDrone).State = EntityState.Detached;
                dbContext.Entry(dbDrone.CurrentPosition).State = EntityState.Detached;
                dbContext.Drones.Update(drone);

                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DroneExists(uuid))
                    {
                        return QueryResult.NotFoundError;
                    }
                    else
                    {
                        return QueryResult.ConcurrencyError;
                    }
                }
                return QueryResult.OK;
            }
        }

        private bool DroneExists(string uuid)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                return dbContext.Drones.Any(e => e.UUID == uuid);
            }
        }
    }
}
