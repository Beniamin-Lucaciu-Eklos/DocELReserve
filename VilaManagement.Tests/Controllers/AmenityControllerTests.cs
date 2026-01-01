using Moq;
using Microsoft.AspNetCore.Mvc;
using VilaManagement.Domain.Entities;
using VilaManagement.Web.Controllers;
using VilaManagement.Web.ViewModels;
using VilaManagement.Tests.Fixtures;

namespace VilaManagement.Tests.Controllers
{
    public class AmenityControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public AmenityControllerTests()
        {
            _mockUnitOfWork = MockHelper.CreateMockUnitOfWork();
        }

        [Fact]
        public void Index_ShouldReturnViewWithAmenities()
        {
            // Arrange
            var amenities = new List<Amenity>
            {
                new Amenity { Id = 1, Name = "WiFi", Description = "Free WiFi", VilaId = 1 },
                new Amenity { Id = 2, Name = "Pool", Description = "Swimming Pool", VilaId = 1 }
            };

            _mockUnitOfWork.Setup(x => x.Amenity.GetAll(null, It.IsAny<string[]>()))
                .Returns(amenities);

            var controller = new AmenityController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Amenity>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_GetRequest_ShouldReturnViewWithVilaList()
        {
            // Arrange
            var vilas = new List<Vila>
            {
                new Vila { Id = 1, Name = "Vila 1", Description = "Desc", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" },
                new Vila { Id = 2, Name = "Vila 2", Description = "Desc", Price = 60000, Sqft = 1200, Occupancy = 5, ImageUrl = "2.jpg" }
            };

            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(vilas);

            var controller = new AmenityController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AmenityViewModel>(viewResult.Model);
            Assert.Equal(2, model.VilaList.Count());
        }

        [Fact]
        public void Create_PostRequest_ValidAmenity_ShouldRedirectToIndex()
        {
            // Arrange
            var viewModel = new AmenityViewModel
            {
                Amenity = new Amenity { Name = "New Amenity", Description = "Desc", VilaId = 1 },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.Amenity.Any(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>()))
                .Returns(false);
            _mockUnitOfWork.Setup(x => x.Amenity.Add(It.IsAny<Amenity>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new AmenityController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AmenityController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Create_PostRequest_DuplicateAmenity_ShouldReturnView()
        {
            // Arrange
            var viewModel = new AmenityViewModel
            {
                Amenity = new Amenity { Name = "Existing", Description = "Desc", VilaId = 1 },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.Amenity.Any(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>()))
                .Returns(true);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new AmenityController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Create(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AmenityViewModel>(viewResult.Model);
        }

        [Fact]
        public void Edit_GetRequest_ValidId_ShouldReturnAmenity()
        {
            // Arrange
            var amenity = new Amenity { Id = 1, Name = "WiFi", Description = "Free WiFi", VilaId = 1 };
            var vilas = new List<Vila>
            {
                new Vila { Id = 1, Name = "Vila 1", Description = "Desc", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" }
            };

            _mockUnitOfWork.Setup(x => x.Amenity.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>(), null))
                .Returns(amenity);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(vilas);

            var controller = new AmenityController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AmenityViewModel>(viewResult.Model);
            Assert.Equal(1, model.Amenity.Id);
        }

        [Fact]
        public void Edit_GetRequest_InvalidId_ShouldRedirectToError()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Amenity.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>(), null))
                .Returns((Amenity)null);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new AmenityController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Edit(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }

        [Fact]
        public void Edit_PostRequest_ValidAmenity_ShouldUpdateAndRedirect()
        {
            // Arrange
            var viewModel = new AmenityViewModel
            {
                Amenity = new Amenity { Id = 1, Name = "Updated WiFi", Description = "Updated", VilaId = 1 },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.Amenity.Update(It.IsAny<Amenity>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new AmenityController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Edit(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AmenityController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Delete_GetRequest_ValidId_ShouldReturnAmenity()
        {
            // Arrange
            var amenity = new Amenity { Id = 1, Name = "WiFi", Description = "Free WiFi", VilaId = 1 };
            var vilas = new List<Vila>
            {
                new Vila { Id = 1, Name = "Vila 1", Description = "Desc", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" }
            };

            _mockUnitOfWork.Setup(x => x.Amenity.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>(), null))
                .Returns(amenity);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(vilas);

            var controller = new AmenityController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AmenityViewModel>(viewResult.Model);
        }

        [Fact]
        public void Delete_PostRequest_ValidId_ShouldDeleteAndRedirect()
        {
            // Arrange
            var viewModel = new AmenityViewModel
            {
                Amenity = new Amenity { Id = 1, Name = "WiFi", Description = "Free WiFi", VilaId = 1 },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            var amenityToDelete = new Amenity { Id = 1, Name = "WiFi", Description = "Free WiFi", VilaId = 1 };

            _mockUnitOfWork.Setup(x => x.Amenity.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>(), null))
                .Returns(amenityToDelete);
            _mockUnitOfWork.Setup(x => x.Amenity.Remove(It.IsAny<Amenity>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new AmenityController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(AmenityController.Index), redirectResult.ActionName);
            _mockUnitOfWork.Verify(x => x.Amenity.Remove(It.IsAny<Amenity>()), Times.Once);
        }

        [Fact]
        public void Delete_PostRequest_InvalidId_ShouldRedirectToError()
        {
            // Arrange
            var viewModel = new AmenityViewModel
            {
                Amenity = new Amenity { Id = 999, Name = "Not Found", Description = "Desc", VilaId = 1 },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.Amenity.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Amenity, bool>>>(), null))
                .Returns((Amenity)null);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new AmenityController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }
    }
}
