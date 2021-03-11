using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public interface IDemoService
    {
        public abstract Task<ActionResult<bool>> StartDemo();
        public abstract Task<ActionResult<bool>> StopDemo();
        public abstract Task<ActionResult<bool>> PauseDrone(string id, bool pause);
        public abstract Task<ActionResult<bool>> ResetDemo();
    }
}
