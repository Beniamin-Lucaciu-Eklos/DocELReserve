using Microsoft.EntityFrameworkCore;
using LuxuryVilaManagement.Infrastructure.Data;

namespace LuxuryVilaManagement.Tests.Fixtures
{
    /// <summary>
    /// Fixture for creating in-memory database contexts for testing
    /// </summary>
    public class DbContextFixture
    {
        public ApplicationDBContext CreateApplicationDBContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               //for SQL Test .UseSqlServer(connectionString:"")
                .Options;

            var context = new ApplicationDBContext(options);
            return context;
        }
    }
}
