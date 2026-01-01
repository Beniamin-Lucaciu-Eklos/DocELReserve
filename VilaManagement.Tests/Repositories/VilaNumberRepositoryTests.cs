using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Repositories;
using VilaManagement.Tests.Fixtures;

namespace VilaManagement.Tests.Repositories
{
    public class VilaNumberRepositoryTests
    {
        private readonly DbContextFixture _fixture = new();

        [Fact]
        public void Update_ValidVilaNumber_ShouldUpdateSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var vilaNumberRepository = new VilaNumberRepository(context);

            var vila = new Vila
            {
                Name = "Test Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            vilaRepository.Add(vila);
            vilaRepository.SaveChanges();

            var vilaNumber = new VilaNumber
            {
                Vila_Number = 101,
                VilaId = vila.Id,
                SpecialDetails = "Original"
            };
            vilaNumberRepository.Add(vilaNumber);
            vilaNumberRepository.SaveChanges();

            // Act
            vilaNumber.SpecialDetails = "Updated";
            vilaNumberRepository.Update(vilaNumber);
            vilaNumberRepository.SaveChanges();

            // Assert
            var updated = vilaNumberRepository.Get(vn => vn.Vila_Number == 101);
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated.SpecialDetails);
        }

        [Fact]
        public void Add_NewVilaNumber_ShouldPersistToDatabase()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var vilaNumberRepository = new VilaNumberRepository(context);

            var vila = new Vila
            {
                Name = "Test Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            vilaRepository.Add(vila);
            vilaRepository.SaveChanges();

            var vilaNumber = new VilaNumber
            {
                Vila_Number = 202,
                VilaId = vila.Id,
                SpecialDetails = "Ground Floor"
            };

            // Act
            vilaNumberRepository.Add(vilaNumber);
            vilaNumberRepository.SaveChanges();

            // Assert
            var added = vilaNumberRepository.Get(vn => vn.Vila_Number == 202);
            Assert.NotNull(added);
            Assert.Equal(vila.Id, added.VilaId);
        }

        [Fact]
        public void Remove_ValidVilaNumber_ShouldDeleteSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var vilaNumberRepository = new VilaNumberRepository(context);

            var vila = new Vila
            {
                Name = "Test Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            vilaRepository.Add(vila);
            vilaRepository.SaveChanges();

            var vilaNumber = new VilaNumber
            {
                Vila_Number = 303,
                VilaId = vila.Id,
                SpecialDetails = "First Floor"
            };
            vilaNumberRepository.Add(vilaNumber);
            vilaNumberRepository.SaveChanges();

            // Act
            vilaNumberRepository.Remove(vilaNumber);
            vilaNumberRepository.SaveChanges();

            // Assert
            var removed = vilaNumberRepository.Get(vn => vn.Vila_Number == 303);
            Assert.Null(removed);
        }

        [Fact]
        public void GetAll_ShouldReturnAllVilaNumbers()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var vilaNumberRepository = new VilaNumberRepository(context);

            var vila = new Vila
            {
                Name = "Test Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            vilaRepository.Add(vila);
            vilaRepository.SaveChanges();

            var vilaNumbers = new List<VilaNumber>
            {
                new VilaNumber { Vila_Number = 401, VilaId = vila.Id, SpecialDetails = "Details 1" },
                new VilaNumber { Vila_Number = 402, VilaId = vila.Id, SpecialDetails = "Details 2" }
            };
            vilaNumbers.ForEach(vn => vilaNumberRepository.Add(vn));
            vilaNumberRepository.SaveChanges();

            // Act
            var all = vilaNumberRepository.GetAll().ToList();

            // Assert
            Assert.Equal(2, all.Count);
        }
    }
}
