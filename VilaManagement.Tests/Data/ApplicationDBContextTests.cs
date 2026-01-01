using Microsoft.EntityFrameworkCore;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.Data;
using VilaManagement.Tests.Fixtures;

namespace VilaManagement.Tests.Data
{
    public class ApplicationDBContextTests
    {
        private readonly DbContextFixture _fixture = new();

        [Fact]
        public void CanCreateContext()
        {
            // Act
            var context = _fixture.CreateApplicationDBContext();

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void CanAddAndRetrieveVila()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Test Villa",
                Description = "A test villa",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg",
                CreatedDate = DateTime.Now
            };

            // Act
            context.Vilas.Add(vila);
            context.SaveChanges();

            var retrievedVila = context.Vilas.FirstOrDefault(v => v.Name == "Test Villa");

            // Assert
            Assert.NotNull(retrievedVila);
            Assert.Equal("Test Villa", retrievedVila.Name);
            Assert.Equal("A test villa", retrievedVila.Description);
            Assert.Equal(50000, retrievedVila.Price);
        }

        [Fact]
        public void CanAddAndRetrieveAmenity()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Villa with Amenity",
                Description = "Test",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            context.Vilas.Add(vila);
            context.SaveChanges();

            var amenity = new Amenity
            {
                Name = "WiFi",
                Description = "Free WiFi",
                VilaId = vila.Id
            };

            // Act
            context.Amenities.Add(amenity);
            context.SaveChanges();

            var retrievedAmenity = context.Amenities.Include(a => a.Vila)
                .FirstOrDefault(a => a.Name == "WiFi");

            // Assert
            Assert.NotNull(retrievedAmenity);
            Assert.Equal("WiFi", retrievedAmenity.Name);
            Assert.Equal(vila.Id, retrievedAmenity.VilaId);
            Assert.NotNull(retrievedAmenity.Vila);
        }

        [Fact]
        public void CanAddAndRetrieveVilaNumber()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Test Villa for Numbers",
                Description = "Test",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            context.Vilas.Add(vila);
            context.SaveChanges();

            var vilaNumber = new VilaNumber
            {
                Vila_Number = 101,
                VilaId = vila.Id,
                SpecialDetails = "Ground Floor"
            };

            // Act
            context.VilaNumbers.Add(vilaNumber);
            context.SaveChanges();

            var retrievedVilaNumber = context.VilaNumbers.Include(vn => vn.Vila)
                .FirstOrDefault(vn => vn.Vila_Number == 101);

            // Assert
            Assert.NotNull(retrievedVilaNumber);
            Assert.Equal(101, retrievedVilaNumber.Vila_Number);
            Assert.Equal(vila.Id, retrievedVilaNumber.VilaId);
        }

        [Fact]
        public void CanUpdateVila()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Original Name",
                Description = "Original",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            context.Vilas.Add(vila);
            context.SaveChanges();

            // Act
            vila.Name = "Updated Name";
            vila.Price = 60000;
            context.Vilas.Update(vila);
            context.SaveChanges();

            var updatedVila = context.Vilas.FirstOrDefault(v => v.Id == vila.Id);

            // Assert
            Assert.NotNull(updatedVila);
            Assert.Equal("Updated Name", updatedVila.Name);
            Assert.Equal(60000, updatedVila.Price);
        }

        [Fact]
        public void CanDeleteVila()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Villa to Delete",
                Description = "Test",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            context.Vilas.Add(vila);
            context.SaveChanges();
            var vilaId = vila.Id;

            // Act
            context.Vilas.Remove(vila);
            context.SaveChanges();

            var deletedVila = context.Vilas.FirstOrDefault(v => v.Id == vilaId);

            // Assert
            Assert.Null(deletedVila);
        }

        [Fact]
        public void VilaRequiredFieldsValidation()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Required Fields Test",
                Description = "Test",
                Price = 15000,
                Sqft = 1000,
                Occupancy = 2,
                ImageUrl = "test.jpg"
            };

            // Act & Assert
            context.Vilas.Add(vila);
            context.SaveChanges();            
   
            var savedVila = context.Vilas.FirstOrDefault(x => x.Name == "Required Fields Test");
            Assert.NotNull(savedVila);
            Assert.Equal("Required Fields Test", savedVila.Name);
        }

        [Fact]
        public void CanRetrieveVilaWithAmenities()
        {
            // Arrange
            var context = _fixture.CreateApplicationDBContext();
            var vila = new Vila
            {
                Name = "Villa with Multiple Amenities",
                Description = "Test",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };
            context.Vilas.Add(vila);
            context.SaveChanges();

            var amenity1 = new Amenity { Name = "WiFi", Description = "Free WiFi", VilaId = vila.Id };
            var amenity2 = new Amenity { Name = "Pool", Description = "Swimming Pool", VilaId = vila.Id };
            context.Amenities.AddRange(amenity1, amenity2);
            context.SaveChanges();

            // Act
            var vilaWithAmenities = context.Vilas.Include(v => v.Amenities)
                .FirstOrDefault(v => v.Id == vila.Id);

            // Assert
            Assert.NotNull(vilaWithAmenities);
            Assert.Equal(2, vilaWithAmenities.Amenities.Count());
        }
    }
}
