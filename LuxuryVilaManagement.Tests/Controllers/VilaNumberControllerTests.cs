using Moq;
using Microsoft.AspNetCore.Mvc;
using LuxuryVilaManagement.Domain.Entities;
using LuxuryVilaManagement.Web.Controllers;
using LuxuryVilaManagement.Web.ViewModels;
using LuxuryVilaManagement.Tests.Fixtures;

namespace LuxuryVilaManagement.Tests.Controllers
{
    public class VilaNumberControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public VilaNumberControllerTests()
        {
            _mockUnitOfWork = MockHelper.CreateMockUnitOfWork();
        }

        [Fact]
        public void Index_ShouldReturnViewWithVilaNumbers()
        {
            // Arrange
            var vilaNumbers = new List<VilaNumber>
            {
                new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Ground Floor" },
                new VilaNumber { Vila_Number = 102, VilaId = 1, SpecialDetails = "First Floor" }
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.GetAll(null, It.IsAny<string[]>()))
                .Returns(vilaNumbers);

            var controller = new VilaNumberController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<VilaNumber>>(viewResult.Model);
            Assert.Equal(2, model.Count());
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

            var controller = new VilaNumberController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<VilaNumberViewModel>(viewResult.Model);
            Assert.Equal(2, model.VilaList.Count());
        }

        [Fact]
        public void Create_PostRequest_ValidVilaNumber_ShouldRedirectToIndex()
        {
            // Arrange
            var viewModel = new VilaNumberViewModel
            {
                VilaNumber = new VilaNumber { Vila_Number = 201, VilaId = 1, SpecialDetails = "New Unit" },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Any(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>()))
                .Returns(false);
            _mockUnitOfWork.Setup(x => x.VilaNumber.Add(It.IsAny<VilaNumber>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new VilaNumberController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Create(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(VilaNumberController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Create_PostRequest_DuplicateNumber_ShouldReturnView()
        {
            // Arrange
            var viewModel = new VilaNumberViewModel
            {
                VilaNumber = new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Existing" },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Any(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>()))
                .Returns(true);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new VilaNumberController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Create(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<VilaNumberViewModel>(viewResult.Model);
        }

        [Fact]
        public void Edit_GetRequest_ValidId_ShouldReturnVilaNumber()
        {
            // Arrange
            var vilaNumber = new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Ground Floor" };
            var vilas = new List<Vila>
            {
                new Vila { Id = 1, Name = "Vila 1", Description = "Desc", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" }
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Get(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>(), null))
                .Returns(vilaNumber);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(vilas);

            var controller = new VilaNumberController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Edit(101);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<VilaNumberViewModel>(viewResult.Model);
            Assert.Equal(101, model.VilaNumber.Vila_Number);
        }

        [Fact]
        public void Edit_GetRequest_InvalidId_ShouldRedirectToError()
        {
            // Arrange
            _mockUnitOfWork.Setup(x => x.VilaNumber.Get(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>(), null))
                .Returns((VilaNumber)null);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new VilaNumberController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Edit(999);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }

        [Fact]
        public void Edit_PostRequest_ValidVilaNumber_ShouldUpdateAndRedirect()
        {
            // Arrange
            var viewModel = new VilaNumberViewModel
            {
                VilaNumber = new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Updated Details" },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Update(It.IsAny<VilaNumber>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new VilaNumberController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Edit(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(VilaNumberController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Delete_GetRequest_ValidId_ShouldReturnVilaNumber()
        {
            // Arrange
            var vilaNumber = new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Ground Floor" };
            var vilas = new List<Vila>
            {
                new Vila { Id = 1, Name = "Vila 1", Description = "Desc", Price = 50000, Sqft = 1000, Occupancy = 4, ImageUrl = "1.jpg" }
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Get(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>(), null))
                .Returns(vilaNumber);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(vilas);

            var controller = new VilaNumberController(_mockUnitOfWork.Object);

            // Act
            var result = controller.Delete(101);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<VilaNumberViewModel>(viewResult.Model);
        }

        [Fact]
        public void Delete_PostRequest_ValidId_ShouldDeleteAndRedirect()
        {
            // Arrange
            var viewModel = new VilaNumberViewModel
            {
                VilaNumber = new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Ground Floor" },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            var numberToDelete = new VilaNumber { Vila_Number = 101, VilaId = 1, SpecialDetails = "Ground Floor" };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Get(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>(), null))
                .Returns(numberToDelete);
            _mockUnitOfWork.Setup(x => x.VilaNumber.Remove(It.IsAny<VilaNumber>()));
            _mockUnitOfWork.Setup(x => x.SaveChanges());
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new VilaNumberController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(VilaNumberController.Index), redirectResult.ActionName);
            _mockUnitOfWork.Verify(x => x.VilaNumber.Remove(It.IsAny<VilaNumber>()), Times.Once);
        }

        [Fact]
        public void Delete_PostRequest_InvalidId_ShouldRedirectToError()
        {
            // Arrange
            var viewModel = new VilaNumberViewModel
            {
                VilaNumber = new VilaNumber { Vila_Number = 999, VilaId = 1, SpecialDetails = "Not Found" },
                VilaList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>()
            };

            _mockUnitOfWork.Setup(x => x.VilaNumber.Get(It.IsAny<System.Linq.Expressions.Expression<Func<VilaNumber, bool>>>(), null))
                .Returns((VilaNumber)null);
            _mockUnitOfWork.Setup(x => x.Villa.GetAll(null, null))
                .Returns(new List<Vila>());

            var controller = new VilaNumberController(_mockUnitOfWork.Object);
            MockHelper.SetupControllerContext(controller);

            // Act
            var result = controller.Delete(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(HomeController.Error), redirectResult.ActionName);
        }
    }
}
