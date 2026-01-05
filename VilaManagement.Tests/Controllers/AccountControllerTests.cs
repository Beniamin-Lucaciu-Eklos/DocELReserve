using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VilaManagement.Application.Common;
using VilaManagement.Domain.Entities;
using VilaManagement.Tests.Fixtures;
using VilaManagement.Web.Controllers;
using VilaManagement.Web.ViewModels;

namespace VilaManagement.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public AccountControllerTests()
        {
            _mockUnitOfWork = MockHelper.CreateMockUnitOfWork();
        }

        [Fact]
        public void Login_ShouldRedirectToDashboard()
        {
        }
    }
}