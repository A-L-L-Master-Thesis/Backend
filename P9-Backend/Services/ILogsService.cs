using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public interface ILogsService
    {
        public abstract Task<ActionResult<IEnumerable<LogEntry>>> GetLogs();
        public abstract Task<ActionResult<LogEntry>> GetLog(int id);
        public abstract Task<QueryResult> AddLog(LogEntry log);
    }
}
