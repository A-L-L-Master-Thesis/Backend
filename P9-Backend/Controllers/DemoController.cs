using Microsoft.AspNetCore.Mvc;
using P9_Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : Controller
    {
        private readonly IDemoService _demoService;
        public DemoController(IDemoService demoService)
        {
            _demoService = demoService;
        }

        // POST: api/demo/(true/false)
        [HttpPost("{demoRunning}")]
        public async Task<ActionResult<bool>> ToggleDemo(bool demoRunning)
        {
            if (demoRunning)
                return await _demoService.StartDemo();
            else
                return await _demoService.StopDemo();
        }

        [HttpPut("{id}/{pause}")]
        public async Task<ActionResult<bool>> TogglePauseDrone(string id, bool pause)
        {
            return await _demoService.PauseDrone(id, pause);
        }

        // DELETE: api/demo
        [HttpDelete]
        public async Task<ActionResult<bool>> ResetDemo(string id)
        {
            return await _demoService.ResetDemo();
        }
    }
}
