using Microsoft.EntityFrameworkCore;
using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Infrastructure.Data;
using LuxuryVilaManagement.Infrastructure.Repositories;
using LuxuryVilaManagement.Tests.Fixtures;

namespace LuxuryVilaManagement.Tests.Repositories
{
    public class RepositoryTests
    {
        private readonly DbContextFixture _fixture = new();

        [Fact]
        public void Add_ValidEntity_ShouldAddSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);
            var vila = new Vila
            {
                Name = "Test Villa",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            // Act
            repository.Add(vila);
            repository.SaveChanges();

            // Assert
            var addedVila = context.Vilas.FirstOrDefault(v => v.Name == "Test Villa");
            Assert.NotNull(addedVila);
            Assert.Equal("Test Villa", addedVila.Name);
        }

        [Fact]
        public void Get_WithValidFilter_ShouldReturnEntity()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);
            var vila = new Vila
            {
                Name = "Find Me",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            repository.Add(vila);
            repository.SaveChanges();

            // Act
            var result = repository.Get(v => v.Name == "Find Me");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Find Me", result.Name);
        }

        [Fact]
        public void Get_WithInvalidFilter_ShouldReturnNull()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);

            // Act
            var result = repository.Get(v => v.Name == "Non-Existent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_NoFilter_ShouldReturnAllEntities()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);
            var vilas = new List<Vila>
            {
                new Vila { Name = "Villa 1", Description = "Desc 1", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" },
                new Vila { Name = "Villa 2", Description = "Desc 2", Price = 60000, Sqft = 1200, Occupancy = 5, ImageUrl = "2.jpg" },
                new Vila { Name = "Villa 3", Description = "Desc 3", Price = 70000, Sqft = 1500, Occupancy = 6, ImageUrl = "3.jpg" }
            };
            vilas.ForEach(v => repository.Add(v));
            repository.SaveChanges();

            // Act
            var results = repository.GetAll().ToList();

            // Assert
            Assert.Equal(3, results.Count);
        }

        [Fact]
        public void GetAll_WithFilter_ShouldReturnFilteredEntities()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);
            var vilas = new List<Vila>
            {
                new Vila { Name = "Expensive", Description = "Desc", Price = 150000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" },
                new Vila { Name = "Cheap", Description = "Desc", Price = 30000, Sqft = 500, Occupancy = 2, ImageUrl = "2.jpg" }
            };
            vilas.ForEach(v => repository.Add(v));
            repository.SaveChanges();

            // Act
            var results = repository.GetAll(v => v.Price > 100000).ToList();

            // Assert
            Assert.Single(results);
            Assert.Equal("Expensive", results[0].Name);
        }

        [Fact]
        public void Remove_ValidEntity_ShouldRemoveSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);
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

            // Act
            repository.Remove(vila);
            repository.SaveChanges();

            // Assert
            var removed = repository.Get(v => v.Name == "To Remove");
            Assert.Null(removed);
        }

        [Fact]
        public void Any_WithExistingEntity_ShouldReturnTrue()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);
            var vila = new Vila
            {
                Name = "Existing",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            repository.Add(vila);
            repository.SaveChanges();

            // Act
            var exists = repository.Any(v => v.Name == "Existing");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public void Any_WithNonExistentEntity_ShouldReturnFalse()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var repository = new Repository<Vila>(context);

            // Act
            var exists = repository.Any(v => v.Name == "Non-Existent");

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public void GetAll_WithIncludeProperties_ShouldLoadRelatedData()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new Repository<Vila>(context);
            var amenityRepository = new Repository<Amenity>(context);

            var vila = new Vila
            {
                Name = "Villa with Amenities",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            vilaRepository.Add(vila);
            vilaRepository.SaveChanges();

            var amenity = new Amenity { Name = "WiFi", Description = "Free WiFi", VilaId = vila.Id };
            amenityRepository.Add(amenity);
            amenityRepository.SaveChanges();

            // Act
            var vilas = vilaRepository.GetAll(includeProperties: new string[] { "Amenities" }).ToList();

            // Assert
            Assert.Single(vilas);
            Assert.Single(vilas[0].Amenities);
        }
    }
}
