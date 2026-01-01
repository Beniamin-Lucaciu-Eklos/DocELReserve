using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Repositories;
using VilaManagement.Tests.Fixtures;

namespace VilaManagement.Tests.Repositories
{
    public class VilaRepositoryTests
    {
        private readonly DbContextFixture _fixture = new();

        [Fact]
        public void Update_ValidVila_ShouldUpdateSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new VilaRepository(context);
            var vila = new Vila
            {
                Name = "Original",
                Description = "Original Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "original.jpg"
            };
            repository.Add(vila);
            repository.SaveChanges();

            // Act
            vila.Name = "Updated";
            vila.Price = 60000;
            repository.Update(vila);
            repository.SaveChanges();

            // Assert
            var updatedVila = repository.Get(v => v.Id == vila.Id);
            Assert.NotNull(updatedVila);
            Assert.Equal("Updated", updatedVila.Name);
            Assert.Equal(60000, updatedVila.Price);
        }

        [Fact]
        public void Add_NewVila_ShouldPersistToDatabase()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new VilaRepository(context);
            var vila = new Vila
            {
                Name = "New Villa",
                Description = "New Description",
                Price = 75000,
                Sqft = 1200,
                Occupancy = 5,
                ImageUrl = "new.jpg",
                CreatedDate = DateTime.Now
            };

            // Act
            repository.Add(vila);
            repository.SaveChanges();

            // Assert
            var addedVila = repository.Get(v => v.Name == "New Villa");
            Assert.NotNull(addedVila);
            Assert.NotEqual(0, addedVila.Id);
        }

        [Fact]
        public void GetAll_ShouldReturnAllVilas()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new VilaRepository(context);

            var vilas = new List<Vila>
            {
                new Vila { Name = "V1", Description = "D1", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" },
                new Vila { Name = "V2", Description = "D2", Price = 60000, Sqft = 1200, Occupancy = 5, ImageUrl = "2.jpg" }
            };

            vilas.ForEach(v => repository.Add(v));
            repository.SaveChanges();

            // Act
            var allVilas = repository.GetAll().ToList();

            // Assert
            Assert.Equal(2, allVilas.Count);
        }

        [Fact]
        public void Remove_ValidVila_ShouldDeleteSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new VilaRepository(context);
            var vila = new Vila
            {
                Name = "To Remove",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            repository.Add(vila);
            repository.SaveChanges();
            var vilaId = vila.Id;

            // Act
            repository.Remove(vila);
            repository.SaveChanges();

            // Assert
            var removed = repository.Get(v => v.Id == vilaId);
            Assert.Null(removed);
        }
    }
}
