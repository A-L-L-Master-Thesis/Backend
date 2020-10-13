using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using P9_Backend.Services;
using System.Data.Common;

namespace P9_Backend_Tests.Services
{
    public class DroneServiceTests : ItemsControllerTest
    {
        private readonly DbConnection _connection;
        private readonly IServiceScopeFactory _scopeFactory;

        protected DroneServiceTests(DbContextOptions<DbContext> contextOptions) : base(
            new DbContextOptionsBuilder<DbContext>()
                .UseSqlite(CreateInMemoryDatabase())
                .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();


        [TestCase("test", ExpectedResult = QueryResult.OK)]
        public QueryResult Test_DroneDelete(string uuid)
        {
            using (var context = new DbContext(ContextOptions))
            {
                DroneService service = new DroneService(_scopeFactory);

                return service.DeleteDrone(uuid).Result;
            }
        }
    }
}
