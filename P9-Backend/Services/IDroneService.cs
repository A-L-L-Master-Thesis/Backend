using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public interface IDroneService
    {
        public abstract Task<QueryResult> RegisterDrone(Drone drone);
        public abstract Task<QueryResult> UpdateDrone(string uuid, Drone drone);
        public abstract Task<ActionResult<Drone>> GetDrone(string uuid);
        public abstract Task<ActionResult<IEnumerable<Drone>>> GetDrones();
        public abstract Task<QueryResult> DeleteDrone(string uuid);
    }
}
