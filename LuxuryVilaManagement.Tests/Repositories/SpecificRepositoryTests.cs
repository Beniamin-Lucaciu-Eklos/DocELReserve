using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Infrastructure.Repositories;
using LuxuryVilaManagement.Tests.Fixtures;

namespace LuxuryVilaManagement.Tests.Repositories
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
