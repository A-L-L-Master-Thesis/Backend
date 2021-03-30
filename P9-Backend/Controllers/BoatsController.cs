using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using P9_Backend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoatsController : Controller
    {
        private readonly IBoatService _boatService;

        public BoatsController(IBoatService boatService)
        {
            _boatService = boatService;
        }

        // GET: api/Boats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Boat>>> GetBoats()
        {
            return await _boatService.GetBoats();
        }

        // GET: api/Boats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Boat>> GetBoat(int id)
        {
            var boat = await _boatService.GetBoat(id);

            if (boat == null)
            {
                return NotFound();
            }

            return boat;
        }

        // PUT: api/Boats/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoat(int id, Boat boat)
        {
            if (id != boat.ID)
            {
                return BadRequest();
            }

            var result = await _boatService.UpdateBoat(id, boat);

            if (result == QueryResult.NotFoundError)
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/Boats
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Boat>> PostBoat(Boat boat)
        {
            var result = await _boatService.RegisterBoat(boat);

            if (result == QueryResult.ConflictError)
            {
                return Conflict();
            }

            return CreatedAtAction("GetBoat", new { id = boat.ID }, boat);
        }

        // DELETE: api/Boats/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Boat>> DeleteBoat(int id)
        {
            var result = await _boatService.DeleteBoat(id);
            if (result == QueryResult.NotFoundError)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
