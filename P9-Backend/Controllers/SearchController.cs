using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using P9_Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISocketService _socketService;
        private readonly IDroneService _droneService;
        public SearchController(ISocketService socketService, IDroneService droneService)
        {
            _socketService = socketService;
            _droneService = droneService;
        }

        [HttpPost("launch")]
        public async Task<IActionResult> LaunchDrones()
        {
            var dronesEnumerable = await _droneService.GetDrones();

            List<Drone> drones = dronesEnumerable.Value.ToList();

            if (drones.Count == 0)
            {
                var resp = Content("No drones registered in system");
                resp.StatusCode = 204;
                return resp;
            }

            if (drones.Any(d => d.Status == DroneStatus.Searching))
            {
                var resp = Content("An operation is already ongoing");
                resp.StatusCode = 406;
                return resp;
            }

            _socketService.SendAll("launch", "");

            return Ok();
        }

        [HttpPost("abort")]
        public async Task<IActionResult> AbortDrones()
        {
            var dronesEnumerable = await _droneService.GetDrones();

            List<Drone> drones = dronesEnumerable.Value.ToList();

            if (drones.Count == 0)
            {
                var resp = Content("No drones registered in system");
                resp.StatusCode = 204;
                return resp;
            }

            if (!(drones.Any(d => d.Status == DroneStatus.Searching || d.Status == DroneStatus.Following || d.Status == DroneStatus.Launching)))
            {
                var resp = Content("No drones in the air");
                resp.StatusCode = 406;
                return resp;
            }

            _socketService.SendAll("abort", "");

            return Ok();
        }
    }
}
