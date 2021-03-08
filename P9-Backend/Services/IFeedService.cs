using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace P9_Backend.Services
{
    public interface IFeedService
    {
        public abstract Task<ActionResult<IEnumerable<string>>> GetFeeds(bool predictive, string host);
    }
}
