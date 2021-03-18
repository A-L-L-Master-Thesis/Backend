using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P9_Backend.DAL;
using P9_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public class LogsService : ILogsService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public LogsService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Adds a log to the database
        /// </summary>
        /// <param name="content">The content of the log</param>
        /// <returns>A QueryResult indicating if successful</returns>
        public async Task<QueryResult> AddLog(LogEntry log)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                try
                {
                    dbContext.Logs.Add(log);
                    await dbContext.SaveChangesAsync();
                }
                catch(DbUpdateException)
                {
                    return QueryResult.UpdateError;
                }
                
                return QueryResult.OK;
            }
        }

        /// <summary>
        /// Gets a log from the database
        /// </summary>
        /// <param name="id">The ID of the log</param>
        /// <returns>A LogEntry</returns>
        public async Task<ActionResult<LogEntry>> GetLog(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                return await dbContext.Logs.FindAsync(id);
            }
        }

        /// <summary>
        /// Gets all logs from the database
        /// </summary>
        /// <returns>A list of LogEntry</returns>
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogs()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                return await dbContext.Logs.ToListAsync();
            }
        }
    }
}
