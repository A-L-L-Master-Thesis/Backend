using Microsoft.AspNetCore.Mvc;
using P9_Backend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P9_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : Controller
    {
        private readonly IFeedService _feedService;
        public FeedController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        // GET: api/feed/(true/false)
        [HttpGet("{predictive}")]
        public async Task<ActionResult<IEnumerable<string>>> RandomFeeds(bool predictive)
        {
            string host = HttpContext.Request.Host.ToString();

            return await _feedService.GetFeeds(predictive, host);
        }
    }
}
