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
    public class LogsController : Controller
    {
        private readonly ILogsService _logsService;
        public LogsController(ILogsService logsService)
        {
            _logsService = logsService;
        }
        // GET: api/logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetAllLogs()
        {
            return await _logsService.GetLogs();
        }

        // GET: api/logs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogEntry>> GetSingleLog(int id)
        {
            var log = await _logsService.GetLog(id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // POST: api/logs
        [HttpPost]
        public async Task<ActionResult<QueryResult>> AddNewLog(LogEntry log)
        {
            var result = await _logsService.AddLog(log);

            if (result == QueryResult.ConflictError)
            {
                return Conflict();
            }

            return Ok();
        }
    }
}
