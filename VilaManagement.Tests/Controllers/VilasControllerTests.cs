using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VilaManagement.Domain.Entities;
using VilaManagement.Web.Controllers;
using VilaManagement.Tests.Fixtures;
using VilaManagement.Infrastructure.IO;
using VilaManagement.Application.IO;

namespace VilaManagement.Tests.Controllers
{
    public class VilasControllerTests
    {
        private readonly Mock<IFilePathService> _mockfilePathService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        public VilasControllerTests()
        {
            _mockfilePathService = MockHelper.CreateFilePathService();
            SetupMockFilePathService();

            _mockUnitOfWork = MockHelper.CreateMockUnitOfWork();

            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("wwwroot");
        }

        private void SetupMockFilePathService()
        {
            var filePathHelper = new FilePathService();

            _mockfilePathService.Setup(x => x.CreateRootPath(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((root, path) => filePathHelper.CreateRootPath(root, path));

            _mockfilePathService
                .Setup(x => x.GetFullPath(It.IsAny<IFormFile>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns<IFormFile, string, string>((file, root, folder) =>
                    filePathHelper.GetFullPath(file, root, folder));

            _mockfilePathService
                .Setup(x => x.GenerateFileName(It.IsAny<IFormFile>()))
                .Returns<IFormFile>(file => filePathHelper.GenerateFileName(file));


            _mockfilePathService
                .Setup(x => x.GetFileNameFromFullPath(It.IsAny<string>()))
                .Returns<string>(fullPath => filePathHelper.GetFileNameFromFullPath(fullPath));

            _mockfilePathService
                .Setup(x => x.Upload(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Callback<IFormFile, string>((file, path) => filePathHelper.Upload(file, path));

            _mockfilePathService.Setup(x => x.Delete(It.IsAny<string>()))
                .Callback<string>(fullPath => filePathHelper.Delete(fullPath));
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

            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(vilas);

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);

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
            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);

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

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);
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

            var controller = new VilasController(_mockUnitOfWork.Object, _mockWebHostEnvironment.Object, _mockfilePathService.Object);
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

            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null))
                .Returns(vila);

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);

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
            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null))
                .Returns((Vila)null);

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);
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

            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null))
                .Returns(vila);
            _mockUnitOfWork.Setup(x => x.Villa.Update(It.IsAny<Vila>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);
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

            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null))
                .Returns(vila);

            _mockUnitOfWork.Setup(x => x.Villa.Remove(It.IsAny<Vila>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);
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
            _mockUnitOfWork.Setup(x => x.Villa.Get(It.IsAny<System.Linq.Expressions.Expression<Func<Vila, bool>>>(), null))
                .Returns((Vila)null);

            var controller = new VilasController(_mockUnitOfWork.Object,
                _mockWebHostEnvironment.Object, _mockfilePathService.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }
    }
}
