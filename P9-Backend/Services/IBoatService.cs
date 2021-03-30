using Microsoft.AspNetCore.Mvc;
using P9_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P9_Backend.Services
{
    public interface IBoatService
    {
        public abstract Task<QueryResult> RegisterBoat(Boat boat);
        public abstract Task<QueryResult> UpdateBoat(int id, Boat boat);
        public abstract Task<ActionResult<Boat>> GetBoat(int id);
        public abstract Task<ActionResult<IEnumerable<Boat>>> GetBoats();
        public abstract Task<QueryResult> DeleteBoat(int id);
    }
}
