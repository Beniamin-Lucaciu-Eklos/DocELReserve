using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Repositories;
using VilaManagement.Tests.Fixtures;

namespace VilaManagement.Tests.Repositories
{
    public class UnitOfWorkTests
    {
        private readonly DbContextFixture _fixture = new();

        [Fact]
        public void Constructor_ShouldInitializeRepositories()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();

            // Act
            var unitOfWork = new UnitOfWork(context);

            // Assert
            Assert.NotNull(unitOfWork.Villa);
            Assert.NotNull(unitOfWork.VilaNumber);
            Assert.NotNull(unitOfWork.Amenity);
        }

        [Fact]
        public void Villa_ShouldBeInstanceOfVilaRepository()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            // Act & Assert
            Assert.IsType<VilaRepository>(unitOfWork.Villa);
        }

        [Fact]
        public void VilaNumber_ShouldBeInstanceOfVilaNumberRepository()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            // Act & Assert
            Assert.IsType<VilaNumberRepository>(unitOfWork.VilaNumber);
        }

        [Fact]
        public void Amenity_ShouldBeInstanceOfAmenityRepository()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            // Act & Assert
            Assert.IsType<AmenityRepository>(unitOfWork.Amenity);
        }

        [Fact]
        public void SaveChanges_ShouldPersistChangesToDatabase()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            var vila = new Vila
            {
                Name = "Test Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            // Act
            unitOfWork.Villa.Add(vila);
            unitOfWork.SaveChanges();

            // Assert
            var retrieved = unitOfWork.Villa.Get(v => v.Name == "Test Vila");
            Assert.NotNull(retrieved);
        }

        [Fact]
        public void Multiple_Repository_Operations_Should_Work()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            var vila = new Vila
            {
                Name = "Multi Test Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            // Act
            unitOfWork.Villa.Add(vila);
            unitOfWork.SaveChanges();

            var amenity = new Amenity
            {
                Name = "WiFi",
                Description = "Free WiFi",
                VilaId = vila.Id
            };
            unitOfWork.Amenity.Add(amenity);
            unitOfWork.SaveChanges();

            var vilaNumber = new VilaNumber
            {
                Vila_Number = 501,
                VilaId = vila.Id,
                SpecialDetails = "Details"
            };
            unitOfWork.VilaNumber.Add(vilaNumber);
            unitOfWork.SaveChanges();

            // Assert
            Assert.NotNull(unitOfWork.Villa.Get(v => v.Name == "Multi Test Vila"));
            Assert.NotNull(unitOfWork.Amenity.Get(a => a.Name == "WiFi"));
            Assert.NotNull(unitOfWork.VilaNumber.Get(vn => vn.Vila_Number == 501));
        }

        [Fact]
        public void Villa_Repository_Should_Support_CRUD()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            var vila = new Vila
            {
                Name = "CRUD Test",
                Description = "Original",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            // Act - Create
            unitOfWork.Villa.Add(vila);
            unitOfWork.SaveChanges();

            // Assert - Read
            var created = unitOfWork.Villa.Get(v => v.Name == "CRUD Test");
            Assert.NotNull(created);

            // Act - Update
            created.Name = "CRUD Test Updated";
            unitOfWork.Villa.Update(created);
            unitOfWork.SaveChanges();

            // Assert - Update
            var updated = unitOfWork.Villa.Get(v => v.Id == created.Id);
            Assert.Equal("CRUD Test Updated", updated.Name);

            // Act - Delete
            unitOfWork.Villa.Remove(updated);
            unitOfWork.SaveChanges();

            // Assert - Delete
            var deleted = unitOfWork.Villa.Get(v => v.Id == created.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public void GetAll_Should_Work_Across_Repositories()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var unitOfWork = new UnitOfWork(context);

            var vilas = new List<Vila>
            {
                new Vila { Name = "V1", Description = "D", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" },
                new Vila { Name = "V2", Description = "D", Price = 60000, Sqft = 1200, Occupancy = 5, ImageUrl = "2.jpg" }
            };
            vilas.ForEach(v => unitOfWork.Villa.Add(v));
            unitOfWork.SaveChanges();

            // Act
            var allVilas = unitOfWork.Villa.GetAll().ToList();

            // Assert
            Assert.Equal(2, allVilas.Count);
        }
    }
}
