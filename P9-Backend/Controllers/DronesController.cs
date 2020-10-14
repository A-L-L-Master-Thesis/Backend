using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P9_Backend.DAL;
using P9_Backend.Models;
using P9_Backend.Services;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DronesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IDroneService _droneService;

        public DronesController(DatabaseContext context, IDroneService droneService)
        {
            _context = context;
            _context.Database.Migrate();
            _droneService = droneService;
        }

        // GET: api/Drones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drone>>> GetDrones()
        {
            return await _droneService.GetDrones();
        }

        // GET: api/Drones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Drone>> GetDrone(string id)
        {
            var drone = await _droneService.GetDrone(id);

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

            var result = await _droneService.UpdateDrone(id, drone);

            if (result == QueryResult.NotFoundError)
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/Drones
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Drone>> PostDrone(Drone drone)
        {
            drone.IP = HttpContext.Connection.RemoteIpAddress.ToString();

            var result = await _droneService.RegisterDrone(drone);

            if (result == QueryResult.ConflictError)
            {
                return Conflict();
            }

            return CreatedAtAction("GetDrone", new { id = drone.UUID }, drone);
        }

        // DELETE: api/Drones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Drone>> DeleteDrone(string id)
        {
            var result = await _droneService.DeleteDrone(id);
            if (result == QueryResult.NotFoundError)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
