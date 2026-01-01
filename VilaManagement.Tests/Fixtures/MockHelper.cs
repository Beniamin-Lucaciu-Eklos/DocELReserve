using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;
using VilaManagement.Infrastructure.IO;
using VilaManagement.Application.IO;

namespace VilaManagement.Tests.Fixtures
{
    /// <summary>
    /// Helper for creating mock objects for testing
    /// </summary>
    public static class MockHelper
    {
        public static Mock<IFilePathService> CreateFilePathService()
        {
            return new Mock<IFilePathService>();
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

            // Create a simple mock temp data dictionary that just stores values in memory
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
    }
}
