using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Repositories;
using VilaManagement.Tests.Fixtures;

namespace VilaManagement.Tests.Repositories
{
    public class AmenityRepositoryTests
    {
        private readonly DbContextFixture _fixture = new();

        [Fact]
        public void Update_ValidAmenity_ShouldUpdateSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var amenityRepository = new AmenityRepository(context);

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

            var amenity = new Amenity
            {
                Name = "WiFi",
                Description = "Free WiFi",
                VilaId = vila.Id
            };
            amenityRepository.Add(amenity);
            amenityRepository.SaveChanges();

            // Act
            amenity.Name = "Updated WiFi";
            amenity.Description = "High-speed WiFi";
            amenityRepository.Update(amenity);
            amenityRepository.SaveChanges();

            // Assert
            var updated = amenityRepository.Get(a => a.Id == amenity.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated WiFi", updated.Name);
        }

        [Fact]
        public void Add_NewAmenity_ShouldPersistToDatabase()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var amenityRepository = new AmenityRepository(context);

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

            var amenity = new Amenity
            {
                Name = "Pool",
                Description = "Swimming Pool",
                VilaId = vila.Id
            };

            // Act
            amenityRepository.Add(amenity);
            amenityRepository.SaveChanges();

            // Assert
            var added = amenityRepository.Get(a => a.Name == "Pool");
            Assert.NotNull(added);
            Assert.Equal(vila.Id, added.VilaId);
        }

        [Fact]
        public void Remove_ValidAmenity_ShouldDeleteSuccessfully()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var amenityRepository = new AmenityRepository(context);

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

            var amenity = new Amenity
            {
                Name = "Gym",
                Description = "Fitness Center",
                VilaId = vila.Id
            };
            amenityRepository.Add(amenity);
            amenityRepository.SaveChanges();

            // Act
            amenityRepository.Remove(amenity);
            amenityRepository.SaveChanges();

            // Assert
            var removed = amenityRepository.Get(a => a.Name == "Gym");
            Assert.Null(removed);
        }

        [Fact]
        public void GetAll_WithFilter_ShouldReturnFilteredAmenities()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vilaRepository = new VilaRepository(context);
            var amenityRepository = new AmenityRepository(context);

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

            var amenities = new List<Amenity>
            {
                new Amenity { Name = "WiFi", Description = "Free", VilaId = vila.Id },
                new Amenity { Name = "Pool", Description = "Swimming", VilaId = vila.Id }
            };
            amenities.ForEach(a => amenityRepository.Add(a));
            amenityRepository.SaveChanges();

            // Act
            var result = amenityRepository.GetAll(a => a.Name == "WiFi").ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("WiFi", result[0].Name);
        }
    }
}
