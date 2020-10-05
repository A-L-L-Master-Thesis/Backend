using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P9_Backend.DAL;
using P9_Backend.Models;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DronesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DronesController(DatabaseContext context)
        {
            _context = context;
            _context.Database.Migrate();
        }

        // GET: api/Drones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            var drones = _context.Drones.Include(d => d.CurrentPosition);
            return await _context.Drones.ToListAsync();
        }

        // GET: api/Drones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Drone>> GetDrone(string id)
        {
            var drones = _context.Drones.Include(d => d.CurrentPosition);
            var drone = await drones.FirstOrDefaultAsync(d => d.UUID == id);

            if (drone == null)
            {
                return NotFound();
            }

            return drone;
        }

        // PUT: api/Drones/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDrone(string id, Drone drone)
        {
            drone.IP = HttpContext.Connection.RemoteIpAddress.ToString();

            if (id != drone.UUID)
            {
                return BadRequest();
            }

            _context.Entry(drone).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DroneExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Drones
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Drone>> PostDrone(Drone drone)
        {
            drone.IP = HttpContext.Connection.RemoteIpAddress.ToString();

            _context.Drones.Add(drone);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DroneExists(drone.UUID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDrone", new { id = drone.UUID }, drone);
        }

        // DELETE: api/Drones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Drone>> DeleteDrone(string id)
        {
            var drone = await _context.Drones.FindAsync(id);
            if (drone == null)
            {
                return NotFound();
            }

            _context.Drones.Remove(drone);
            await _context.SaveChangesAsync();

            return drone;
        }

        private bool DroneExists(string id)
        {
            return _context.Drones.Any(e => e.UUID == id);
        }
    }
}
