using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly DatabaseContext _context;

        public DroneService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<QueryResult> DeleteDrone(string uuid)
        {
            var drone = await _context.Drones.FindAsync(uuid);
            if (drone == null)
            {
                return QueryResult.NotFoundError;
            }

            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();

            return QueryResult.OK;
        }

        public async Task<ActionResult<Drone>> GetDrone(string uuid)
        {
            var drones = _context.Drones.Include(d => d.CurrentPosition);
            var drone = await drones.FirstOrDefaultAsync(d => d.UUID == uuid);

            return drone;
        }

        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            var drones = _context.Drones.Include(d => d.CurrentPosition);
            return await _context.Drones.ToListAsync();
        }

        public async Task<QueryResult> RegisterDrone(Drone drone)
        {
            _context.Drones.Add(drone);
            try
            {
                await _context.SaveChangesAsync();
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

        public async Task<QueryResult> UpdateDrone(string uuid, Drone drone)
        {
            _context.Entry(drone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
        private bool DroneExists(string uuid)
        {
            return _context.Drones.Any(e => e.UUID == uuid);
        }
    }
}
