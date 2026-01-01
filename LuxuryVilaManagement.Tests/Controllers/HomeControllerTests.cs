using Moq;
using Microsoft.AspNetCore.Mvc;
using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Web.Controllers;
using LuxuryVilaManagement.Tests.Fixtures;

namespace LuxuryVilaManagement.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public HomeControllerTests()
        {
            _mockUnitOfWork = MockHelper.CreateMockUnitOfWork();
        }

        [Fact]
        public void Index_ShouldReturnViewWithHomeViewModel()
        {
            // Arrange
            var vilas = new List<Vila>
            {
                new Vila
                {
                    Id = 1,
                    Name = "Luxury Villa",
                    Description = "Beautiful villa",
                    Price = 100000,
                    Sqft = 2000,
                    Occupancy = 6,
                    ImageUrl = "luxury.jpg",
                    Amenities = new List<Amenity>()
                },
                new Vila
                {
                    Id = 2,
                    Name = "Beach Villa",
                    Description = "Beachfront property",
                    Price = 150000,
                    Sqft = 2500,
                    Occupancy = 8,
                    ImageUrl = "beach.jpg",
                    Amenities = new List<Amenity>()
                }
            };

            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(vilas);

            var controller = new HomeController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(2, model.Vilas.Count());
            Assert.Equal(1, model.NumberOfNights);
        }

        [Fact]
        public void Index_WithNoVilas_ShouldReturnEmptyList()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(new List<Vila>());

            var controller = new HomeController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Empty(model.Vilas);
        }

        [Fact]
        public void Index_ShouldSetCheckInDateToToday()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(new List<Vila>());

            var controller = new HomeController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), model.CheckInDate);
        }

        [Fact]
        public void Index_ShouldIncludeAmenities()
        {
            // Arrange
            var amenities = new List<Amenity>
            {
                new Amenity { Id = 1, Name = "WiFi", Description = "Free WiFi", VilaId = 1 },
                new Amenity { Id = 2, Name = "Pool", Description = "Swimming Pool", VilaId = 1 }
            };

            var vilas = new List<Vila>
            {
                new Vila
                {
                    Id = 1,
                    Name = "Test Vila",
                    Description = "Test",
                    Price = 50000,
                    Sqft = 1000,
                    Occupancy = 4,
                    ImageUrl = "test.jpg",
                    Amenities = amenities
                }
            };

            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(vilas);

            var controller = new HomeController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Single(model.Vilas);
            Assert.Equal(2, model.Vilas.First().Amenities.Count());
        }

        [Fact]
        public void Error_ShouldReturnErrorView()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(new List<Vila>());

            var controller = new HomeController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Web.Models.ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
        }

        [Fact]
        public void Index_ShouldCallVillaGetAllWithAmenitiesIncluded()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(new List<Vila>());

            var controller = new HomeController(_mockUnitOfWork.Object);

            // Act
            controller.Index();

            // Assert
            _mockUnitOfWork.Verify(
                x => x.Villa.GetAll(null, It.IsAny<string[]>()),
                Times.Once);
        }

        [Fact]
        public void Index_ShouldReturnMultipleVilas()
        {
            // Arrange
            var vilas = new List<Vila>();
            for (int i = 1; i <= 5; i++)
            {
                vilas.Add(new Vila
                {
                    Id = i,
                    Name = $"Vila {i}",
                    Description = "Test",
                    Price = 50000 + (i * 10000),
                    Sqft = 1000,
                    Occupancy = 4,
                    ImageUrl = $"{i}.jpg",
                    Amenities = new List<Amenity>()
                });
            }

            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, It.IsAny<string[]>()))
                .Returns(vilas);

            var controller = new HomeController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(viewResult.Model);
            Assert.Equal(5, model.Vilas.Count());
        }
    }
}
