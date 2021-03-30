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
    public class BoatService : IBoatService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public BoatService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<QueryResult> DeleteBoat(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var boat = await dbContext.Boats.Include(b => b.CurrentPosition).FirstOrDefaultAsync(b => b.ID == id);

                if (boat == null)
                {
                    return QueryResult.NotFoundError;
                }

                var pos = boat.CurrentPosition;

                dbContext.Boats.Remove(boat);
                dbContext.Position.Remove(pos);

                await dbContext.SaveChangesAsync();

                return QueryResult.OK;
            }
        }

        public async Task<ActionResult<Boat>> GetBoat(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var boats = dbContext.Boats.Include(b => b.CurrentPosition);
                var boat = await boats.FirstOrDefaultAsync(b => b.ID == id);

                return boat;
            }
        }

        public async Task<ActionResult<IEnumerable<Boat>>> GetBoats()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                return await dbContext.Boats.Include(b => b.CurrentPosition).ToListAsync();
            }
        }

        public async Task<QueryResult> RegisterBoat(Boat boat)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                dbContext.Boats.Add(boat);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    if (BoatExists(boat.ID))
                    {
                        return QueryResult.ConflictError;
                    }
                    else
                    {
                        return QueryResult.UpdateError;
                    }
                }

                return QueryResult.OK;
            }
        }

        public async Task<QueryResult> UpdateBoat(int id, Boat boat)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var dbBoat = await dbContext.Boats.Include(b => b.CurrentPosition).FirstOrDefaultAsync(b => b.ID == id);
                boat.CurrentPosition.ID = dbBoat.CurrentPosition.ID;

                dbContext.Entry(dbBoat).State = EntityState.Detached;
                dbContext.Entry(dbBoat.CurrentPosition).State = EntityState.Detached;
                dbContext.Boats.Update(boat);

                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoatExists(id))
                    {
                        return QueryResult.NotFoundError;
                    }
                    else
                    {
                        return QueryResult.ConcurrencyError;
                    }
                }
                return QueryResult.OK;
            }
        }

        private bool BoatExists(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                return dbContext.Boats.Any(e => e.ID == id);
            }
        }
    }
}
