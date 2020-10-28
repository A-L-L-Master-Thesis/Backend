using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using P9_Backend.Services;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IAreaDecompositionService _areaDecompositonService;

        public SearchController(IAreaDecompositionService areaDecompositonService)
        {
            _areaDecompositonService = areaDecompositonService;
        }

        [HttpPost]
        public async Task<ActionResult<List<DroneZone>>> PostSearch(List<Coordinate> searchArea)
        {
            List<DroneZone> result = _areaDecompositonService.ComputeSubAreas(searchArea);
            return Ok(result);
        }
    }
}
