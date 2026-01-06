using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.IO;
using VilaManagement.Application.IO;
using VilaManagement.Application.Services;

namespace VilaManagement.Tests.Fixtures
{
    /// <summary>
    /// Helper for creating mock objects for testing
    /// </summary>
    public static class MockHelper
    {
        public static Mock<IFilePathService> CreateFilePathService()
        {
            var mockFilePathService = new Mock<IFilePathService>();
            var filePathHelper = new FilePathService();

            mockFilePathService.Setup(x => x.CreateRootPath(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((root, path) => filePathHelper.CreateRootPath(root, path));

            mockFilePathService
                .Setup(x => x.GetFullPath(It.IsAny<IFormFile>(),
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns<IFormFile, string, string>((file, root, folder) =>
                    filePathHelper.GetFullPath(file, root, folder));

            mockFilePathService
                .Setup(x => x.GenerateFileName(It.IsAny<IFormFile>()))
                .Returns<IFormFile>(file => filePathHelper.GenerateFileName(file));


            mockFilePathService
                .Setup(x => x.GetFileNameFromFullPath(It.IsAny<string>()))
                .Returns<string>(fullPath => filePathHelper.GetFileNameFromFullPath(fullPath));

            mockFilePathService
                .Setup(x => x.Upload(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Callback<IFormFile, string>((file, path) => filePathHelper.Upload(file, path));

            mockFilePathService.Setup(x => x.Delete(It.IsAny<string>()))
                .Callback<string>(fullPath => filePathHelper.Delete(fullPath));
            return mockFilePathService;
        }

        public static Mock<IUnitOfWork> CreateMockUnitOfWork()
        {
            return new Mock<IUnitOfWork>();
        }

        public static Mock<IVilaRepository> CreateMockVilaRepository()
        {
            return new Mock<IVilaRepository>();
        }

        public static Mock<IAmenityRepository> CreateMockAmenityRepository()
        {
            return new Mock<IAmenityRepository>();
        }

        public static Mock<IVilaNumberRepository> CreateMockVilaNumberRepository()
        {
            return new Mock<IVilaNumberRepository>();
        }

        public static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        public static Mock<SignInManager<ApplicationUser>> CreateMockSignInManager()
        {
            var userManager = CreateMockUserManager();
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<ApplicationUser>>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                null);
        }

        public static Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                store.Object, null, null, null, null);
        }

        /// <summary>
        /// Setup a controller with proper HttpContext and TempData for testing
        /// </summary>
        public static void SetupControllerContext(Controller controller)
        {
            var httpContext = new DefaultHttpContext();

            var tempDataDictionary = new Dictionary<string, object>();
            var tempDataMock = new Mock<ITempDataDictionary>();

            tempDataMock.Setup(x => x[It.IsAny<string>()])
                .Returns<string>(key => tempDataDictionary.ContainsKey(key) ? tempDataDictionary[key] : null);

            tempDataMock.SetupSet(x => x[It.IsAny<string>()] = It.IsAny<object>())
                .Callback<string, object>((key, value) => tempDataDictionary[key] = value);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.TempData = tempDataMock.Object;
        }

        public static Mock<IVilaService> CreateMockVilaService(IMock<IUnitOfWork> mUnitOfWork,
            IMock<IWebHostEnvironment> mWebHostEnvironment,
            IMock<IFilePathService> mFilePathService)
        {
            var mockVilaService = new Mock<IVilaService>();

            var vilaService = new VilaService(mUnitOfWork.Object, mWebHostEnvironment.Object, mFilePathService.Object);
            mockVilaService.Setup(x => x.Update(It.IsAny<Vila>()))
                            .Callback<Vila>(vila => vilaService.Update(vila));

            mockVilaService.Setup(x => x.Add(It.IsAny<Vila>()))
                .Callback<Vila>(vila => vilaService.Add(vila));

            mockVilaService.Setup(x => x.Remove(It.IsAny<Vila>()))
                        .Callback<Vila>(vila => vilaService.Remove(vila));

            mockVilaService.Setup(x => x.SaveImageUrl(It.IsAny<Vila>()))
                    .Callback<Vila>(vila => vilaService.SaveImageUrl(vila));

            mockVilaService.Setup(x => x.DeleteImage(It.IsAny<Vila>()))
                    .Callback<Vila>(vila => vilaService.DeleteImage(vila));

            mockVilaService.Setup(x => x.Get(It.IsAny<int>()))
                  .Callback<int>((vilaId) => vilaService.Get(vilaId));

            mockVilaService.Setup(x => x.GetAll())
                  .Returns(() => vilaService.GetAll());

            mockVilaService.Setup(x => x.UploadImage(It.IsAny<Vila>()))
                .Callback<Vila>(vila => vilaService.UploadImage(vila));

            return mockVilaService;
        }
        
        public static Mock<IVilaNumberService> CreateMockVilaNumberService(IMock<IUnitOfWork> mUnitOfWork)
        {
            var mockVilaNumberService = new Mock<IVilaNumberService>();

            var vilaNumberService = new VilaNumberService(mUnitOfWork.Object);
            mockVilaNumberService.Setup(x => x.Update(It.IsAny<VilaNumber>()))
                            .Callback<VilaNumber>(vilaNumber => vilaNumberService.Update(vilaNumber));

            mockVilaNumberService.Setup(x => x.Add(It.IsAny<VilaNumber>()))
                .Callback<VilaNumber>(vilaNumber => vilaNumberService.Add(vilaNumber));

            mockVilaNumberService.Setup(x => x.Remove(It.IsAny<VilaNumber>()))
                        .Callback<VilaNumber>(vilaNumber => vilaNumberService.Remove(vilaNumber));

            mockVilaNumberService.Setup(x => x.Exists(It.IsAny<int>()))
               .Callback<int>((vilaId) => vilaNumberService.Exists(vilaId));

            mockVilaNumberService.Setup(x => x.Get(It.IsAny<int>()))
                  .Callback<int>((vilaId) => vilaNumberService.Get(vilaId));

            mockVilaNumberService.Setup(x => x.GetAll())
                  .Returns(() => vilaNumberService.GetAll());

            return mockVilaNumberService;
        }

        public static Mock<IWebHostEnvironment> CreateMockWebHostEnviroment()
        {
            var mockWebHostEnviroment = new Mock<IWebHostEnvironment>();
            mockWebHostEnviroment.Setup(x => x.WebRootPath).Returns("wwwroot");

            return mockWebHostEnviroment;
        }
    }
}
