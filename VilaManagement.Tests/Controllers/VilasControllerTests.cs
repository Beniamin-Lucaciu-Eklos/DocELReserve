using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VilaManagement.Domain.Entities;
using VilaManagement.Web.Controllers;
using VilaManagement.Tests.Fixtures;
using VilaManagement.Infrastructure.IO;
using VilaManagement.Application.IO;
using VilaManagement.Application.Services;
using System.Linq.Expressions;

namespace VilaManagement.Tests.Controllers
{
    public class VilasControllerTests
    {
        private readonly Mock<IFilePathService> _mockfilePathService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IVilaService> _mockVilaService;

        public VilasControllerTests()
        {
            _mockfilePathService = MockHelper.CreateFilePathService();
            _mockUnitOfWork = MockHelper.CreateMockUnitOfWork();
            _mockWebHostEnvironment = MockHelper.CreateMockWebHostEnviroment();
            _mockVilaService = MockHelper.CreateMockVilaService(_mockUnitOfWork, _mockWebHostEnvironment, _mockfilePathService);
        }

        [Fact]
        public void Index_ShouldReturnViewWithVilas()
        {
            // Arrange
            var vilas = new List<Vila>
            {
                new Vila { Id = 1, Name = "Vila 1", Description = "Desc", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" },
                new Vila { Id = 2, Name = "Vila 2", Description = "Desc", Price = 60000, Sqft = 1200, Occupancy = 5, ImageUrl = "2.jpg" }
            };

            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null, false))
                .Returns(vilas);

            var controller = new VilasController(_mockVilaService.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Vila>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_GetRequest_ShouldReturnCreateView()
        {
            // Arrange
            var controller = new VilasController(_mockVilaService.Object);

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_PostRequest_ValidVila_ShouldRedirectToIndex()
        {
            // Arrange
            var vila = new Vila
            {
                Name = "New Vila",
                Description = "Description",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            _mockUnitOfWork.Setup(x => x.Villa.Add(It.IsAny<Vila>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());

            var controller = new VilasController(_mockVilaService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Create(vila);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(VilasController.Index), redirectResult.ActionName);
            _mockUnitOfWork.Verify(x => x.Villa.Add(It.IsAny<Vila>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Create_PostRequest_SameName_ShouldReturnViewWithError()
        {
            // Arrange
            var vila = new Vila
            {
                Name = "Same Name",
                Description = "Same Name", // Same as Name
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            var controller = new VilasController(_mockVilaService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Create(vila);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void Edit_GetRequest_ValidId_ShouldReturnVila()
        {
            // Arrange
            var vila = new Vila
            {
                Id = 1,
                Name = "Test Vila",
                Description = "Desc",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null, true))
                .Returns(vila);
            _mockVilaService.Setup(x => x.Get(It.IsAny<int>()))
              .Returns(vila);

            var controller = new VilasController(_mockVilaService.Object);

            // Act
            var result = controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Vila>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public void Edit_GetRequest_InvalidId_ShouldRedirectToError()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null, false))
                .Returns((Vila)null);

            var controller = new VilasController(_mockVilaService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Edit(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }

        [Fact]
        public void Edit_PostRequest_ValidVila_ShouldUpdateAndRedirect()
        {
            // Arrange
            var vila = new Vila
            {
                Id = 1,
                Name = "Updated Vila",
                Description = "Updated Desc",
                Price = 60000,
                Sqft = 1200,
                Occupancy = 5,
                ImageUrl = "updated.jpg"
            };

            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null, false))
                .Returns(vila);
            _mockUnitOfWork.Setup(x => x.Villa.Update(It.IsAny<Vila>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());

            var controller = new VilasController(_mockVilaService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Edit(vila);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(VilasController.Index), redirectResult.ActionName);
            _mockUnitOfWork.Verify(x => x.Villa.Update(It.IsAny<Vila>()), Times.Once);
        }

        [Fact]
        public void Delete_GetRequest_ValidId_ShouldReturnVila()
        {
            // Arrange
            var vila = new Vila
            {
                Id = 1,
                Name = "Test Vila",
                Description = "Desc",
                Price = 50000,
                Sqft = 1000,
                Occupancy = 4,
                ImageUrl = "test.jpg"
            };

            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null, false))
              .Returns(vila);

            _mockVilaService.Setup(x => x.Get(It.IsAny<int>()))
                .Returns(vila);


            var controller = new VilasController(_mockVilaService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(VilasController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Delete_InvalidId_ShouldRedirectToError()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null, false))
                .Returns((Vila)null);

            var controller = new VilasController(_mockVilaService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }
    }
}
